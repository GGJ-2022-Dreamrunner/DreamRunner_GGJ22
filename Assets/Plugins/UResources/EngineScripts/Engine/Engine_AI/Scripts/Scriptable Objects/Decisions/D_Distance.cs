using UnityEngine;

namespace Engine.AI
{
    [CreateAssetMenu(menuName = "AI/Decision/Distance")]
    public class D_Distance : D_Base
    {
        
        [SerializeField] private float radius;
        
        public override bool Decide(AIEntity controller)
        {
            return radius >= Vector3.Distance(controller.target.transform.position,
                controller.transform.position);
        }
    }
}