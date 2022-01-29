using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonPlayer
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField] float maxUp = 60f, minUp = 5f;
        
        Vector2 input = Vector2.zero;
        [SerializeField] float sensitivity = 3f;

        private void Start()
        {
            transform.eulerAngles = new Vector3(0, 40, 0);
        }

        private void Update()
        {

            transform.Rotate(Vector3.right, input.y * sensitivity * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.up, input.x * sensitivity * Time.deltaTime, Space.World);

            var eulerAngles = transform.eulerAngles;
            eulerAngles = new Vector3(Mathf.Clamp(eulerAngles.x, minUp, maxUp),
                eulerAngles.y,
                eulerAngles.z);
            transform.eulerAngles = eulerAngles;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
        }

        void OnLook(InputValue val)
        {
            input = val.Get<Vector2>();
        }
    }
}
