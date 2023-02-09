using System;
using Engine.Engine_Actors;
using UnityEngine;
using UnityEngine.AI;


namespace Engine.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIEntity : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] internal bool isActive = true;
        [SerializeField] internal Transform target;
        
        [Header("Serialized Objects")] 
        [SerializeField] private State currentState;
        [SerializeField] internal Entity stats;



        internal NavMeshAgent agent;

        [HideInInspector] public float StateTimeElapsed { get; private set; }
        
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            if (TryGetComponent(out BaseHealth health)) health.maxHealth = stats.maxHealth;
        }

        private void Start()
        {
            currentState.OnEnter(this);
        }

        private void Update()
        {
            if (!isActive) return;
            currentState.UpdateState(this);
            agent.destination = target.position;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = currentState.gizmoColor;
            Gizmos.DrawWireSphere(transform.position, .5f);
        }

        public void TransitionToState(State nextState)
        {
            currentState.OnExit(this);
            currentState = nextState;
            currentState.OnEnter(this);
            OnExitState();
        }

        public bool CheckIfCountDownElapsed(float duration)
        {
            StateTimeElapsed += UnityEngine.Time.deltaTime;
            return (StateTimeElapsed >= duration);
        }

        private void OnExitState()
        {
            StateTimeElapsed = 0;
        }
    }
}