using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Engine.AI
{
    [System.Serializable]
    public class Transition
    {
        [SerializeField] internal D_Base decision;
        [SerializeField] internal State trueState;
        [SerializeField] internal State falseState;
    }
}