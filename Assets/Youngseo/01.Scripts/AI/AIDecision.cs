using UnityEngine;

namespace FSM
{
    [RequireComponent(typeof(AITransition))]
    public abstract class AIDecision : MonoBehaviour
    {
        protected AIBrain _brain;
        public bool IsReverse;

        public virtual void SetUp(Transform agent)
        {
            _brain = agent.GetComponent<AIBrain>();
        }

        public abstract bool MakeADecision();
    }
}