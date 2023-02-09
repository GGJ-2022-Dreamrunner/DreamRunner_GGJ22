using System;
using UnityEngine;

namespace Engine.AI
{
    [CreateAssetMenu(menuName = "AI/State")]
    public class State : ScriptableObject
    {
        public A_Base[] actions;
        public Transition[] transitions;
        public Color gizmoColor = Color.grey;

        public void UpdateState(AIEntity controller)
        {
            DoActions(controller);
            CheckTransitions(controller);
        }

        
        
        private void DoActions(AIEntity controller)
        {
            foreach (var action in actions)
            {
                action.Act(controller);
            }
        }

        private void CheckTransitions(AIEntity controller)
        {
            foreach (var transition in transitions)
            {
                bool decisionSucceeded = transition.decision.Decide(controller);

                controller.TransitionToState(decisionSucceeded ? transition.trueState : transition.falseState);
            }
        }

        public void OnEnter(AIEntity controller)
        {
            foreach (var action in actions)
            {
                action.OnEnter(controller);
            }
        }

        public void OnExit(AIEntity controller)
        {
            foreach (var action in actions)
            {
                action.OnExit(controller);
            }
        }
    }
}