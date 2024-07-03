using System.Collections.Generic;
using Dohee;
using UnityEngine;

namespace FSM
{
    public abstract class AIState : MonoBehaviour, IState
    {
        protected List<AITransition> _transitions;
        protected AIBrain _brain;
        protected Rigidbody2D _rigid;

        public virtual void SetUp(Transform agent)
        {
            _brain = agent.GetComponent<AIBrain>();
            _rigid = agent.GetComponent<Rigidbody2D>();

            _transitions = new();
            GetComponentsInChildren(_transitions);

            _transitions.ForEach(transition => transition.SetUp(agent));
        }

        public abstract void OnEnterState();

        public abstract void OnExitState();

        public virtual void UpdateState()
        {
            foreach (var transition in _transitions)
            {
                if (transition.MakeATransition())
                {
                    _brain.ChangeState(transition.NextState);
                }
            }
        }
    }
}