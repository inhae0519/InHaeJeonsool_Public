using System.Collections;
using System.Linq;
using Dohee;
using UnityEngine;

namespace FSM
{
    public class PatrolState : AIState
    {
        [SerializeField] private float _moveSpeed = 5;
        [SerializeField] private float _rotationSpeed = 10f;
        private Vector3 _origin;
        private Vector3 _dir;
        
        private readonly int _layer = 1 << 8 | 1 << 16;

        private FishScale _myFish;
        
        public override void SetUp(Transform agent)
        {
            base.SetUp(agent);
            _myFish = _brain.GetComponent<FishScale>();
        }

        public override void OnEnterState()
        {
            _dir = Vector3.zero;
            _origin = _brain.transform.position;
        }

        public override void OnExitState()
        {
            
        }

        public override void UpdateState() // Delay마다 얻은 dir로 이동
        {
            base.UpdateState();
            
            GetRandomDirection();
            _dir = GetCalculatedDirection(_dir);
            float z = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;

            _rigid.AddForce(_dir * 20);
            _rigid.velocity = Vector2.ClampMagnitude(_rigid.velocity, _moveSpeed);
            
            _brain.transform.rotation = 
                Quaternion.Slerp
                (
                    _brain.transform.rotation,
                    Quaternion.Euler(0, 0, z),
                    _rotationSpeed * Time.deltaTime
                );
            
            _brain.transform.localScale = (_rigid.velocity.x < 0 ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1)) *
                                          _brain.transform.localScale.x;
        }

        private float _lastGetTime = -9999f;
        
        private void GetRandomDirection()
        {
            if (Equals(_dir, Vector3.zero) || Vector3.Distance(_brain.transform.position, _origin) > 1)
            {
                if (Time.time - _lastGetTime < 2) return;
                _lastGetTime = Time.time;
                float rdRad = Random.Range(-Mathf.PI, Mathf.PI);
                _dir = (_origin + new Vector3(Mathf.Cos(rdRad), Mathf.Sin(rdRad)) - _brain.transform.position)
                    .normalized;
            }
        }
        
        private Vector3 GetCalculatedDirection(Vector3 dir)
        {
            RaycastHit2D[] hit = 
                Physics2D.RaycastAll
                (
                    origin: _brain.transform.position,
                    direction: _brain.transform.right, 
                    distance: _myFish.Scale,
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