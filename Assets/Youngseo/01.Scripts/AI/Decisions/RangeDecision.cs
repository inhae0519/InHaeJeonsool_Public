using Dohee;
using UnityEngine;

namespace FSM
{
    public class RangeDecision : AIDecision
    {
        [SerializeField] private bool _world;
        private float _range;

        private FishScale _player;

        public override void SetUp(Transform agent)
        {
            base.SetUp(agent);
            _player = _brain.playerTrm.GetComponent<FishScale>();
        }

        public override bool MakeADecision()
        {
            _range = _world ? transform.lossyScale.x : transform.localScale.x;
            return Physics2D.OverlapCircle(transform.position, _world ? _range * _player.Scale : _range, 1 << 3);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}