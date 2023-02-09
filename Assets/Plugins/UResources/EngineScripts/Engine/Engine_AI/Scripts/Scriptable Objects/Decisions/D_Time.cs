using System.Timers;
using Engine.Engine_Timer;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.AI
{
    [CreateAssetMenu(menuName = "AI/Decision/Timer")]
    public class D_Time : D_Base
    {
        [SerializeField] private float time = 5;
        private NATTime.TimeSince _ts = new NATTime.TimeSince();
        public override bool Decide(AIEntity controller)
        {
            return _ts >= time;
        }
    }
}