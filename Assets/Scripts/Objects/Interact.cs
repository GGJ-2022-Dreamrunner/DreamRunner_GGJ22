using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Interact : MonoBehaviour
{
     public CurrentVolume c = null;
    ParticleSystem sys;

    [SerializeField] Material promptGamepad, promptKB;

    private void Awake()
    {
        sys = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    void OnInteract(InputValue val) 
    {
        if (c != null) c.Interact();
    }

    private void Update()
    {
        if (GetComponent<PlayerInput>().currentControlScheme == "Gamepad")
        {
            GetComponentInChildren<ParticleSystemRenderer>().material = promptGamepad;
        }
        else if(GetComponent<PlayerInput>().currentControlScheme == "Keyboard & Mouse")
        {
            GetComponentInChildren<ParticleSystemRenderer>().material = promptKB;
        }


        if (c != null)
        {
            sys.Emit(1);
        }
        else
        {
            sys.Clear();
        }
    }
}
