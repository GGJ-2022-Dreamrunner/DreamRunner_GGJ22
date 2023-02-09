using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Engine_Timer 
{
    public static class NATTime
    {
        // THIS IS THE ONLY THING YOU SHOULD INTERFACE WITH
        /// <summary>
        /// Creates a new timer instance.
        /// </summary>
        /// <param name="goal">The goal time for the timer. Will tick from start.</param>
        /// <param name="func">The function that the timer will execute</param>
        /// <param name="start">the start time override for the timer.</param>
        public static Timer CreateNewTimer(float goal, Action func, float start = 0f)
        {
            var t = new Timer(goal, start);  
            t.OnTimerFinished += func;
            _timers.Add(t);

            //Return t for extended implementation (I.E, cancelling timers)
            return t;
        }


        //Private
        private static List<Timer> _timers = new List<Timer>();

        static NATTime()
        {
            Application.onBeforeRender += Update;
        }

        private static void Update()
        {
            foreach (Timer timer in _timers)
            {
                if (timer.Update())
                {
                    _timers.Remove(timer);
                    Update();
                    return;
                }
            }
        }

        [Serializable]
        public class Timer
        {
            internal event Action OnTimerFinished;


            [SerializeField] private float goalTime;
            private float _currentTime;

            internal bool Update()
            {
                _currentTime += UnityEngine.Time.deltaTime;

                if (_currentTime >= goalTime)
                {
                    OnTimerFinished?.Invoke( );
                    return true;
                }
                return false;
            }

            internal Timer(float goal, float start = 0f)
            {
                goalTime = goal;
                _currentTime = start;
            }
        }

        public struct TimeSince
        {
            private float _time;

            public static implicit operator float( TimeSince ts )
            {
                return UnityEngine.Time.time - ts._time;
            }

            public static implicit operator TimeSince( float ts )
            {
                return new TimeSince { _time = UnityEngine.Time.time - ts };
            }
        }
    }

}
