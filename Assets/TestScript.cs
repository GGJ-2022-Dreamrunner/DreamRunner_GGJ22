using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
public class TestScript : MonoBehaviour
{
    DialogueSystemTrigger trigger;


    private void Awake()
    {
        trigger = GetComponent<DialogueSystemTrigger>();
    }


    private void Start()
    {
        trigger.Fire(transform);
    }
}
