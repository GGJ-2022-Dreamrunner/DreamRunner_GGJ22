using UnityEngine;


namespace Engine.AI
{
    public abstract class A_Base : ScriptableObject
    {
        public abstract void Act(AIEntity controller);

        public abstract void OnEnter(AIEntity controller);

        public abstract void OnExit(AIEntity controller);
    }
}