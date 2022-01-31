using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Engine.Engine_Timer;
using UnityEngine.InputSystem;

namespace ThirdPersonPlayer
{
	/// <summary>
	/// This is a struct containing some information about the camera. Notable its x and y rotation
	/// </summary>
	public struct CameraHelper
	{
		public float rotX;
		public float rotY;
		/// <summary>
		/// A function which takes in a delta and adds it to the rotX and rotY variables, clamping the 
		/// rotY variable so you can't look behind you.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void Rotate(float x, float y)
		{
			//Reset these values to 0 since we're using local rotation in the controller
			//This is important for moving platforms to work.
			//TODO: Create some system of offsets for this so we can do stuff like recoil
			rotX = rotY = 0;
			rotX += x;
			rotY -= y;
			rotY = Mathf.Clamp(rotY, -90f, 90f);
		}
		/// <summary>
		/// Current position of the camera
		/// </summary>
		public Vector3 Position;
		/// <summary>
		/// Initial Position of the Camera
		/// </summary>
		public Vector3 InitialPosition;
	}

	


	/// <summary>
	/// This is simply a struct used to hold information about our input. Used purely for organizational purposes.
	/// </summary>
	public struct InputHelper
	{
		public float ForwardMove;
		public float RightMove;
		public float UpMove;
	}

	public class NetscapeFPSController : MonoBehaviour
	{
		public Animator animator;
		#region Public Helpers
		/// <summary>
		/// Reference to the player camera
		/// </summary>
		public Camera Cam;

		/// <summary>
		/// A Vector3 representing the players velocity.
		/// </summary>
		[HideInInspector]
		public Vector3 Velocity = Vector3.zero;
		#endregion

		#region Constants
		/// <summary>
		/// The amount of gravity applied
		/// </summary>
		[Header("Constants")]
		public float gravity = 20.0f;
		/// <summary>
		/// global amount of friction
		/// </summary>
		public float friction = 6;
		/// <summary>
		/// A movement mask used for traces that need to detect if we can do things like uncrouch.
		/// </summary>
		public LayerMask MovementMask;
		#endregion

		#region Movement
		/// <summary>
		/// The movement sped
		/// </summary>
		[Header("Movement Values")]
		public float MoveSpeed = 7.0f;
		/// <summary>
		/// The acceleration when running on the ground
		/// </summary>
		public float RunAcceleration = 14.0f;
		/// <summary>
		/// The deacceleration that occurs when running on the ground
		/// </summary>
		public float RunDeacceleration = 10.0f;
		/// <summary>
		/// How fast we accelerate when we're in the air
		/// </summary>
		public float AirAcceleration = 2.0f;
		/// <summary>
		/// Deacceleration experienced when ooposite strafing
		/// </summary>
		public float AirDecceleration = 2.0f;
		/// <summary>
		/// How precise the air control is.
		/// </summary>
		public float AirControl = 0.3f;
		/// <summary>
		/// How fast acceleration is when strafing
		/// </summary>
		public float SideStrafeAcceleration = 50.0f;
		/// <summary>
		/// Max strafe speed
		/// </summary>
		public float SideStrafeSpeed = 1.0f;
		/// <summary>
		/// A force that gets applied to the player in transit that makes going up and down hills easier
		/// </summary>
		public float StickToGroundForce = 10f;
		/// <summary>
		/// A varaiable representing the amount of friction we get on slopes.
		/// </summary>
		public float SlopeFriction = 0.3f;

		/// <summary>
		/// The jump speed
		/// </summary>
		[Header("Jumps")]
		public float JumpSpeed = 8.0f;

		/// <summary>
		/// The maximum number of jumps we're allowed to have
		/// </summary>
		public int MaxJumps = 2;

		/// <summary>
		/// Our capsules height when not crouching
		/// </summary>
		[Header("Crouch")]
		public float UncrouchedHeight = 1.8f;
		/// <summary>
		/// Our unrouched width.
		/// </summary>
		public float UncrouchedRadius = 0.5f;
		/// <summary>
		/// The height of the capsule when we're crouched
		/// </summary>
		public float CrouchedHeight = 1.0f;
		/// <summary>
		/// The radius of the capsule when we're crouched
		/// </summary>
		public float CrouchedRadius = 0.3f;
		/// <summary>
		/// A variable representing how slow we move when crouched
		/// </summary>
		public float CrouchSpeed = 3.5f;
		/// <summary>
		/// The amount to move the camera down when crouching
		/// </summary>
		public float CrouchCameraMod = 0.1f;

		#endregion

		#region Private Members
		/// <summary>
		/// Reference to a character controller component
		/// </summary>
		private CharacterController Controller;

		/// <summary>
		/// The camera helper which is used to help control mouse input.
		/// </summary>
		private CameraHelper CamHelper;
		/// <summary>
		/// The input helper which helps organize input
		/// </summary>
		private InputHelper InHelper;
		/// <summary>
		/// The normalized value of our desired move direction.
		/// </summary>
		private Vector3 MoveDirectionNorm = Vector3.zero;
		/// <summary>
		/// Whether we wish to jump in a given frame
		/// </summary>
		private bool WishJump;

		/// <summary>
		/// The number of jumps we currently have
		/// </summary>
		private int Jumps;

		/// <summary>
		/// The time since we last jumped. Used to make sure we can't immediately double jump.
		/// </summary>
		private NATTime.TimeSince TimeSinceJump;

		/// <summary>
		/// Our collision flags, useful for telling if we've hit a certain side of our controller
		/// </summary>
		private CollisionFlags ColFlags;
		/// <summary>
		/// This is a variable for our ground normal, this way we can do sliding sown slopes if they're too steep.
		/// </summary>
		private Vector3 GroundNormal;
		/// <summary>
		/// A variable representing how our movement is affected by being crouched
		/// </summary>
		private float CrouchMovementCoef;
		/// <summary>
		/// The vertical offset by which the camera is set when crouching
		/// </summary>
		private float VerticalCamOffset;

		/// <summary>
		/// A list of our movement componenents, things we control that modify movement outside of this file.
		/// Stuff like the grappling hook and the netscape tube traveller. 
		/// This is done so that we can have things that modify the movement and still have the controller be agnostic of having to know about them.
		/// *Extremely* useful if we wanna use this controller in other projects, but maybe don't want all the Netscape bells and whistles.
		/// </summary>
		//private List<MovementComponent> MovementComponents;

		/// <summary>
		/// A boolean for whether or not we want to apply the stick to ground force.
		/// </summary>
		private bool StickToGround;

		#endregion

		#region Debug
		[Header("Debug")]
		public GUIStyle style;
		#endregion


		/// <summary>
		/// Initialization
		/// </summary>
		private void Start()
		{
			//Get a reference to our controller
			Controller = GetComponent<CharacterController>();
			//Instantiate the CamHelper
			CamHelper = new CameraHelper();
			CamHelper.InitialPosition = Cam.transform.localPosition;
			//Insantiate the InHelper;
			InHelper = new InputHelper();
			////Get our Movement components
			//MovementComponents = GetComponents<MovementComponent>().ToList();

			//Set the controller for our movement components to this
			//foreach (MovementComponent m in MovementComponents)
			//{
			//	m.Controller = this;
			//}
		}

		private void Update()
		{
			HandleRotation();
			HandleController();
			ResetProperties();
			
			animator.SetBool("isJumping", !Controller.isGrounded);
		}

		/// <summary>
		/// Handles the input for if we want to jump or not
		/// </summary>
		//private void QueueJump()
		//{
		//	if (Input.GetButtonDown("Jump") && !WishJump)
		//		WishJump = true;
		//	else
		//		WishJump = false;
		//}

		void OnJump(InputValue val)
        {
			WishJump = true;
        }


		/// <summary>
		/// The method responsible for handling the actual movement. This is where our isgrounded checks and a lot of the stuff we do go. 
		/// </summary>
		public void HandleController()
		{
			//Check if we wanna jump
			//QueueJump();
			//Handle our grappling
			//HandleMovementComponents();
			//Pick the control method based on whether or not the controller is grounded.
			if (Controller.isGrounded)
			{
				GroundMove();
				HandleSlopes();
			}
			else if (!Controller.isGrounded)
			{
				AirMove();
			}

			//This bit of code should probably be moved to Air Move, but basically if our collision flags are "above" set the vertical
			//component of our velocity to 0. This basically prevents us from "sticking" to the ceiling, a weird by product of how unity handles jumping.
			if (ColFlags == CollisionFlags.Above)
			{
				Velocity = Velocity.WithY(-1);
			}

			//Now we can move the character controller
			ColFlags = Controller.Move(Velocity * Time.deltaTime);
		}

		#region Movement Components
		///// <summary>
		///// Simulates our individual movement componenets
		///// </summary>
		//public void HandleMovementComponents()
		//{
		//	foreach (MovementComponent m in MovementComponents)
		//	{
		//		m.Simulate();
		//	}
		//}

		/// <summary>
		/// Returns the gravity coefficient calculated from our movement components.
		/// </summary>
		/// <returns></returns>
		public float CalculateGravityCoefficient()
		{
			float grav = 1;
			//for (int i = 0; i < MovementComponents.Count; i++)
			//{
			//	grav *= MovementComponents[i].GravCoef();
			//}
			return grav;
		}
		#endregion

		/// <summary>
		/// The ground movement component of the movement controller. This will handle stuff like friction and what not.
		/// </summary>
		public void GroundMove()
		{

			//Our desired direction
			Vector3 WishDir;

			//Only apply friction if we're not about to jump
			if (!WishJump)
			{
				ApplyFriction(1.0f);
			}
			else
			{
				ApplyFriction(0);
			}

			//Set the movement dir
			//SetMovementDir();

			//We're going to reset the movement dir if the slope is above our slope limit, that way we can't hold W against a slope to try and climb it
			if (CheckSlope())
			{
				InHelper.RightMove = 0f;
				InHelper.ForwardMove = 0f;
			}

			//Set the wish dir to the values from InHelper, set in the above SetMovementDir function
			WishDir = new Vector3(InHelper.RightMove, 0, InHelper.ForwardMove);
			WishDir = InHelper.RightMove * Cam.transform.right + InHelper.ForwardMove * new Vector3(Cam.transform.forward.x, 0, Cam.transform.forward.z);
			//Convert the wish direction to a worldspace vector
			WishDir = transform.TransformDirection(WishDir);
			//Normalize this direction
			WishDir.Normalize();
			//Store the wish direction in the Normal
			MoveDirectionNorm = WishDir;

			//Store the magnitude of our desired direction
			var wishSpeed = WishDir.magnitude;
			//Multiply the wish Speed by the move speed or crouch speed every frame.
			wishSpeed *= MoveSpeed;


			//Accelerate up to the given speed in this given direction
			Accelerate(WishDir, wishSpeed, RunAcceleration);

			//Apply gravity to our PCs velocity
			Velocity.y = -gravity * Time.deltaTime;

			//Don't apply the stick to ground force if we aren't allowed to stick to the ground. Prevents some issues when grappling
			if (StickToGround)
			{
				Velocity.y -= StickToGroundForce;
			}

			//This function only runs when grounded, meaning we can reset our max number of jumps.
			Jumps = MaxJumps;

			//And finally if we wanna jump, then jump
			if (WishJump)
			{
				//Subtract from the jumps so that in the editor, if we want a double jump we set max jumps to true.
				Jumps--;
				Velocity.y = JumpSpeed;
				TimeSinceJump = 0f;
				WishJump = false;
			}
		}

		/// <summary>
		/// This is basically going to constantly check our ground normal and we will apply a bit of velocity in the opposite direction of the normal
		/// so that we can go off the slope.
		/// </summary>
		public void HandleSlopes()
		{
			if (Controller.isGrounded)
			{
				if (CheckSlope())
				{
					Velocity.x += (1f - GroundNormal.y) * GroundNormal.x * (1f - SlopeFriction);
					Velocity.z += (1f - GroundNormal.y) * GroundNormal.z * (1f - SlopeFriction);
				}
			}
		}


		/// <summary>
		/// A method which returns whether or not the angle of the surface we're standing on is greater than our given slope limit
		/// </summary>
		public bool CheckSlope()
		{
			return Vector3.Angle(Vector3.up, GroundNormal) > Controller.slopeLimit;
		}

		/// <summary>
		/// The bread and butter of this movement controller. The creme de le creme. This takes in some information and accelerates the player's velocity in a given
		/// direction. 
		/// </summary>
		/// <param name="wishDir">The direction we wish to go</param>
		/// <param name="wishSpeed">The speed in which we want to move in our given wish direction</param>
		/// <param name="accel">How fast we accelerate to that wish direction</param>
		public void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
		{
			//The speed we're going toadd
			float addSpeed;
			//The speed of our acceleration
			float accelSpeed;
			//The current speed
			float currentSpeed;

			//The dot product of our wish direction and the current velocity of our player
			currentSpeed = Vector3.Dot(Velocity, wishDir);
			//How much speed we want to add from our  desired speed to our current speed;
			addSpeed = wishSpeed - currentSpeed;

			//If we have no more speed to add, just return because we're already at our desired wish speed, so we don't need to accelerate any more.
			if (addSpeed <= 0)
			{
				return;
			}
			//Essentially, we're accelerating towards the add speed at the rate of our desired speed
			accelSpeed = accel * Time.deltaTime * wishSpeed;
			//Basically clamp us to the addSpeed if our acceleration exceeds it.
			if (accelSpeed > addSpeed)
			{
				accelSpeed = addSpeed;
			}

			//Finally, modify our velocity accordingly
			Velocity.x += accelSpeed * wishDir.x;
			Velocity.z += accelSpeed * wishDir.z;

		}

		/// <summary>
		/// The air movement component of the move controller. This will handle air control and occurs when the player is in the air.
		/// </summary>
		public void AirMove()
		{
			//The desired direction we wish to travel in.
			Vector3 WishDir;
			//Setting our wish velocity to the air acceleration value
			float wishVelocity = AirAcceleration;
			//Our given acceleration
			float accel;

			//Get the input
			//SetMovementDir();

			//Set our wish direction based on the given input
			//WishDir = new Vector3(InHelper.RightMove, 0, InHelper.ForwardMove);
			WishDir = InHelper.RightMove * Cam.transform.right + InHelper.ForwardMove * Cam.transform.forward;
			//Transform it to world space
			WishDir = transform.TransformDirection(WishDir);

			///Set our wish speed to the wish directions magnitude then multiply that by our move speed
			float wishSpeed = WishDir.magnitude;
			wishSpeed *= MoveSpeed;

			//This next portion of code handles air control
			float airWishSpeed = wishSpeed;
			//Basically check if we're moving in the direction of our velocity
			if (Vector3.Dot(Velocity, WishDir) < 0)
			{
				accel = AirDecceleration;
			}
			else
			{
				accel = AirAcceleration;
			}

			////This next bit of code only runs if we're only strafing in the air.
			//if (InHelper.ForwardMove == 0 && InHelper.RightMove != 0)
			//{
			//	//If our wish speed is greater than the side strafe speed
			//	//Basically don't let us strafe faster than our strafe speed.
			//	if (wishSpeed > SideStrafeSpeed)
			//	{
			//		wishSpeed = SideStrafeSpeed;
			//	}
			//	accel = SideStrafeAcceleration;
			//}

			//Accelerate in the given direction
			Accelerate(WishDir, wishSpeed, accel);

			//Basically, if we have air control enabled, do it
			if (AirControl > 0)
			{
				DoAirControl(WishDir, airWishSpeed);
			}

			//Since we're in the air, we check if we want to jump, see if we have enough jumps left, and then jump;
			if (WishJump)
			{
				if (Jumps > 0 && TimeSinceJump >= 0.1f)
				{
					Jumps--;
					Velocity.y = JumpSpeed;
					TimeSinceJump = 0f;
				}
				//Set wish jump to false regardless so it resets and we don't jump again when we land
				WishJump = false;
			}

			//Finally, apply gravity
			Velocity.y -= gravity * CalculateGravityCoefficient() * Time.deltaTime;
		}

		private void DoAirControl(Vector3 wishdir, float wishspeed)
		{
			//Our zspeed
			float zspeed;
			//Speed variable
			float speed;
			//Dot product
			float dot;
			//Some K value
			float k;

			// Can't control movement if not moving forward or backward
			if (Mathf.Abs(InHelper.ForwardMove) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
				return;
			//Store the vertical component of our velocity then 0 it out
			zspeed = Velocity.y;
			Velocity.y = 0;

			//Set the speed to our velocities magnitude then normalize it
			speed = Velocity.magnitude;
			Velocity.Normalize();

			//Get the dot product of our Velocity and our desired direction
			dot = Vector3.Dot(Velocity, wishdir);
			//Start at 32
			k = 32;
			//No clue what this math does, but presumably acts as a damping value
			k *= AirControl * dot * dot * Time.deltaTime;

			// Change direction while slowing down
			if (dot > 0)
			{
				Velocity.x = Velocity.x * speed + wishdir.x * k;
				Velocity.y = Velocity.y * speed + wishdir.y * k;
				Velocity.z = Velocity.z * speed + wishdir.z * k;

				Velocity.Normalize();
				MoveDirectionNorm = Velocity;
			}

			Velocity.x *= speed;
			Velocity.y = zspeed;
			Velocity.z *= speed;
		}

		/// <summary>
		/// This function sets the input direction on the input helper
		/// </summary>
		//public void SetMovementDir()
		//{
		//	InHelper.ForwardMove = Input.GetAxisRaw("Vertical");
		//	InHelper.RightMove = Input.GetAxisRaw("Horizontal");
		//}	

		private void OnMove(InputValue val)
        {
			InHelper.ForwardMove = val.Get<Vector2>().y;
			InHelper.RightMove = val.Get<Vector2>().x;

			animator.SetFloat("Speed", Mathf.Abs(InHelper.ForwardMove) + Mathf.Abs(InHelper.RightMove));
		}

		/// <summary>
		/// This applies a given friction to our current speed, slowing it down.
		/// </summary>
		/// <param name="t">The amount of friction to apply.</param>
		private void ApplyFriction(float t)
		{
			//Story the current velocity
			Vector3 vec = Velocity;
			//Define the current speed
			float speed;
			//Define the new speed
			float newspeed;
			//Define control?
			float control;
			//Define drop?
			float drop;

			//For some reason set the velocity copy's Y to zero.
			vec.y = 0.0f;
			//Set the speed to the magnitude of our velocity on the XZ plane.
			speed = vec.magnitude;
			//Set drop to 0f
			drop = 0.0f;

			//Apply friction if the controller is grounded.
			if (Controller.isGrounded)
			{
				//Control is a value which
				control = (speed < RunDeacceleration) ? RunDeacceleration : speed;
				//Drop appears to be the amount of deacceleration to be applied. In this case we're taking the control variable from above, and multiplying it by the friction value
				drop = control * friction * Time.deltaTime * t;
			}

			//The new speed is decreased by the "drop" velocity
			newspeed = speed - drop;

			//Clamp the newspeed to 0 if its less than 0;
			if (newspeed < 0)
				newspeed = 0;
			//And if our speed is less than 0, divide the new speed by speed
			if (speed > 0)
				newspeed /= speed;

			//Apply this new speed to our velocity
			Velocity.x *= newspeed;
			Velocity.z *= newspeed;
		}

		/// <summary>
		/// Since we'll be moving the camera rotation logic to inside the controller, this is where that will be handled.
		/// In addition to rotating the camera, this also rotates our movement collider.
		/// </summary>
		public void HandleRotation()
		{
            //float x = Input.GetAxisRaw("Mouse X") * 2f;
            //float y = Input.GetAxisRaw("Mouse Y") * 2f;
            //CamHelper.Rotate(x, y);

            ////Set the rotation of the player
            //transform.localRotation *= Quaternion.Euler(0, CamHelper.rotX, 0);

            ////Updating the cursor lock state to be locked
        }

		/// <summary>
		/// Sets variables that prevent us from sticking to the ground momentarily.
		/// </summary>
		public void ClearGround()
		{
			StickToGround = false;
		}

		/// <summary>
		/// Called at the end of the frame to reset any variables that supress things like the stick to ground force or the ability to jump
		/// </summary>
		public void ResetProperties()
		{
			StickToGround = true;
		}

		/// <summary>
		/// Adds to our controller's current velocity.
		/// </summary>
		/// <param name="add"></param>
		public void AddVelocity(Vector3 add)
		{
			Velocity += add;
		}

		/// <summary>
		/// Sets our controller's current velocity.
		/// </summary>
		/// <param name="vel"></param>
		public void SetVelocity(Vector3 vel)
		{
			Velocity = vel;
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			GroundNormal = hit.normal;

			//This is weird. Basically cancels horizontal component of velocity if we hit a wall.
			if (ColFlags == CollisionFlags.Sides)
			{
				var dot = Vector3.Dot(Velocity, hit.normal);
				Debug.Log(dot);
				if (dot < 0f)
				{
					Velocity -= hit.normal * dot;
				}
			}
		}
	}
}