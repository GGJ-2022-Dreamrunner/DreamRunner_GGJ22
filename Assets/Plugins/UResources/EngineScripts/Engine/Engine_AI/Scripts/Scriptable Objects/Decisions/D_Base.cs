using UnityEngine;

namespace Engine.AI
{
    
    public abstract class D_Base : ScriptableObject
    {
        public abstract bool Decide(AIEntity controller);
    }
}
