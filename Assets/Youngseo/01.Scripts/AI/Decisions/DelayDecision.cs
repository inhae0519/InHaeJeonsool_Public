using UnityEngine;

namespace FSM
{
    public class DelayDecision : AIDecision
    {
        [SerializeField] private float _duration = 3;
        private float _currentTime = 0;

        public override bool MakeADecision()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > _duration)
            {
                _currentTime = 0;
                return true;
            }

            return false;
        }
    }
}