using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class CurrentVolume : MonoBehaviour
{
    public UnityEvent onInteract;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Interact>(out var i))
        {
            i.c = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Interact>(out var i))
        {
            i.c = null;
        }
    }

    public void Interact()
    {
        onInteract.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(GetComponent<Collider>().bounds.center, "goal.png");
    }

    public void OnConversationEnd()
    {
        FindObjectOfType<PlayerInput>().ActivateInput();
    }
}
