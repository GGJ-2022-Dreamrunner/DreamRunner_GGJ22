using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netscape
{
	public class Tripod : MonoBehaviour
	{

		/// <summary>
		/// A reference to our camera modifier struct
		/// </summary>
		public CameraModifier Camera;

		protected virtual void Start()
		{
			//Instantiate the camera modifier
			Camera = new CameraModifier();
			Camera.Position = Vector3.zero;
			Camera.Rotation = Quaternion.identity;
			Camera.InitialPosition = transform.localPosition;
		}

		protected virtual void Update()
		{
			Simulate();
		}

		/// <summary>
		/// Simulate the camera
		/// </summary>
		void Simulate()
		{
			PostCameraSetup(ref Camera);
			this.transform.localPosition = Camera.Position;
			this.transform.localRotation = Camera.Rotation;
		}

		public virtual void PostCameraSetup(ref CameraModifier camera)
		{
			//Do any behavior necessary for modifing the camera here
		}
	}

	public struct CameraModifier
	{
		public Quaternion Rotation;
		public Vector3 Position;
		public Vector3 InitialPosition;
	}
}