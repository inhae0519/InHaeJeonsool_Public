using System.Collections;
using Dohee;
using UnityEngine;

namespace FSM
{
    public enum AttackType
    {
        Base,
        Octopus
    }
    
    public class AttackState : AIState
    {
        private FishScale _myScale;
        private Vector3 _offset;
        
        [SerializeField] private AttackType _attackType;
        [SerializeField] private LayerMask _playerLayer;

        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _atkDelay = 1f;
        private float _lastAtkTime = -9999f;

        private Collider2D _coll;

        public override void SetUp(Transform agent)
        {
            base.SetUp(agent);
            _myScale = agent.GetComponent<FishScale>();
            _coll = _brain.GetComponent<Collider2D>();
        }

        public override void OnEnterState()
        {
            if (_attackType == AttackType.Octopus)
            {
                _coll.enabled = false;
                StopCoroutine(nameof(BlowOut));
            }
            _offset = (transform.position - _brain.playerTrm.position).normalized;
        }

        public override void OnExitState()
        {
            if (_attackType == AttackType.Octopus)
            {
                _coll.enabled = true; 
                StartCoroutine(nameof(BlowOut));
            }
        }

        public override void UpdateState()
        {
            base.UpdateState();

            _rigid.velocity = Vector2.zero;
            Vector3 dir = (_brain.playerTrm.position - _brain.transform.position).normalized;
            float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _brain.transform.rotation = Quaternion.Slerp(_brain.transform.rotation, Quaternion.Euler(0, 0, z),
                10 * Time.deltaTime);

            if (dir.x < 0)
            {
                _brain.transform.localScale = new Vector3(1, -1, 1) * _myScale.Scale;
            }
            else
            {
                _brain.transform.localScale = new Vector3(1, 1, 1) * _myScale.Scale;
            }
            
            if (_attackType == AttackType.Base)
            {
                if (Physics2D.OverlapCircle(transform.position, _attackRange, _playerLayer) &&
                    _brain.playerTrm.TryGetComponent(out FishScale playerScale))
                {
                    if (Time.time - _lastAtkTime < _atkDelay) return;
                    _lastAtkTime = Time.time;

                    playerScale.Scale -= _myScale.Scale / 10;
                    if (playerScale.Scale < _myScale.Scale / 2)
                    {
                        Destroy(_brain.playerTrm.gameObject);
                    }
                }
                else _brain.transform.Translate(Vector3.right * (1.5f * Time.deltaTime));
            }
            else // Octopus
            {
                if (Physics2D.OverlapCircle(transform.position, _attackRange, _playerLayer) &&
                    _brain.playerTrm.TryGetComponent(out FishScale fishScale))
                {
                    fishScale.Scale -= Time.deltaTime / _atkDelay;
                    _myScale.Scale += Time.deltaTime / _atkDelay;
                    _brain.transform.position = _brain.playerTrm.position + _offset * _myScale.Scale / 2;
                }
            }
        }

        private IEnumerator BlowOut()
        {
            yield return new WaitForSeconds(5f);
            while (_myScale.Scale > 0.5f)
            {
                _myScale.Scale -= Time.deltaTime / 10;
                yield return null;
            }
        }
    }
}
