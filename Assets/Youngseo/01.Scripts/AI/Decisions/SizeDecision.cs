using Dohee;
using UnityEngine;

namespace FSM
{
    public class SizeDecision : AIDecision
    {
        private FishScale _myScale;
        private FishScale _playerScale;
        [SerializeField] private float _difference = 0.2f;
        
        public override void SetUp(Transform agent)
        {
            base.SetUp(agent);
            _myScale = agent.GetComponent<FishScale>();
            _playerScale = _brain.playerTrm.GetComponent<FishScale>();
        }

        public override bool MakeADecision()
        {
            return _myScale.Scale - _difference > _playerScale.Scale;
        }
    }
}