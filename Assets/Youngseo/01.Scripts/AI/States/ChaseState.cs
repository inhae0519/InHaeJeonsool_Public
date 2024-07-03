using UnityEngine;
using FSM;

namespace FSM
{
    public class ChaseState : AIState
    {
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotationSpeed = 10f;
        
        public override void OnEnterState()
        {
            
        }

        public override void OnExitState()
        {
            
        }

        public override void UpdateState() // 플레이어 방향으로 쫓아감
        {
            base.UpdateState();
            Debug.Log(0);
            
            Vector3 dir = (_brain.playerTrm.position - transform.position).normalized * (_moveSpeed * Time.deltaTime);
            float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Vector2 vel = _brain.transform.right * _moveSpeed;
            _rigid.AddForce(vel.y > 0 ? new Vector2(vel.x, vel.y * 2) : vel);
            _rigid.velocity = Vector2.ClampMagnitude(_rigid.velocity, _moveSpeed);
            _brain.transform.rotation = Quaternion.Slerp(_brain.transform.rotation, Quaternion.Euler(0, 0, z),
                _rotationSpeed * Time.deltaTime);
            
            _brain.transform.localScale = (dir.x < 0 ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1)) *
                                          _brain.transform.localScale.x;
        }
    }
}