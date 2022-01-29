using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMovementDirection : MonoBehaviour
{
    Vector3 lastPosition;

    // Start is called before the first frame update
    void Awake  ()
    {
        lastPosition = transform.position.WithY(transform.position.y) + transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != lastPosition && transform.parent.GetComponent<CharacterController>().isGrounded)
        {
            transform.LookAt(transform.position.WithY(transform.position.y) + (transform.position.WithY(transform.position.y) - lastPosition.WithY(transform.position.y)));

            
        }

        lastPosition = transform.position.WithY(transform.position.y);
    }   

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position.WithY(transform.position.y) + ((transform.position.WithY(transform.position.y) - lastPosition) * 4), 2f);
    }
}
