using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    Vector2 input = Vector2.zero;
    float sensitivity = 3f;

    private void Start()
    {
        transform.eulerAngles = new Vector3(0, 40, 0);
    }

    private void Update()
    {
        transform.Rotate(Vector3.right, input.y * sensitivity * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.up, input.x * sensitivity * Time.deltaTime, Space.World);

        transform.eulerAngles =  new Vector3(Mathf.Clamp(transform.eulerAngles.x, 4, 30), transform.eulerAngles.y, transform.eulerAngles.z);



    }

    void OnLook(InputValue val)
    {
        input = val.Get<Vector2>();
    }
}
