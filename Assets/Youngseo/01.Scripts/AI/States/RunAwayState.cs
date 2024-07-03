using System.Linq;
using UnityEngine;

namespace FSM
{
    public class RunAwayState : AIState
    {
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotationSpeed = 10f;

        private Vector3 _dir;
        private Quaternion _targetRotation;
        private readonly int _layer = 1 << 8 | 1 << 16;
        
        public override void OnEnterState()
        {
            
        }

        public override void OnExitState()
        {
            
        }

        public override void UpdateState() // 플레이어 반대 방향으로 도망
        {
            base.UpdateState();
            
            _dir = GetCalculatedDirection((transform.position - _brain.playerTrm.position).normalized);
            _targetRotation = Quaternion.Euler(0, 0, Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg);

            _rigid.AddForce(_dir * _moveSpeed);
            _rigid.velocity = Vector2.ClampMagnitude(_rigid.velocity, _moveSpeed);
            _brain.transform.rotation = Quaternion.Slerp(_brain.transform.rotation, _targetRotation,
                _rotationSpeed * Time.deltaTime);

            _brain.transform.localScale = (_dir.x < 0 ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1)) *
                                          _brain.transform.localScale.x;
        }

        private Vector3 GetCalculatedDirection(Vector3 dir)
        {
            RaycastHit2D[] hit = 
                Physics2D.RaycastAll
                (
                    origin: _brain.transform.position,
                    direction: dir, 
                    distance: 3,
                    layerMask: _layer
                ).OrderBy(h => h.distance).ToArray();

            if (hit.Length > 1)
            {
                dir = Vector3.Reflect(dir, hit[1].normal);
            }

            return dir;
        }
    }
}