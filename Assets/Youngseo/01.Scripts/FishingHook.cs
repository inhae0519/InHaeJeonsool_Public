using LeeInHae;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class FishingHook : Poolable
    {
        [SerializeField] private float _upDownValue = 1;
        [SerializeField] private float _speed = 1f;
        private float _startValue;

        private SpriteRenderer _spriteRenderer;
        private Transform _bobberTrm;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _bobberTrm = transform.Find("Bobber");
        }

        private void Start()
        {
            _startValue = _spriteRenderer.size.y;
            StartCoroutine(nameof(UpAndDown));
        }

        private void LateUpdate() // 바늘 위치도 위 아래로
        {
            _bobberTrm.localPosition = new Vector3(0, 0.25f - _spriteRenderer.size.y);
        }

        public void Reroad()
        {
            StopCoroutine(nameof(UpAndDown));
            StartCoroutine(Catch());
        }

        private IEnumerator UpAndDown() // 낚시 바늘 위아래로 왔다갔다
        {
            while (true)
            {
                float time = _startValue / 1.75f / _speed;
                float currentTime = 0, percent = 0;
                while (percent < 1)
                {
                    currentTime += Time.deltaTime;
                    percent = currentTime / time;
                    _spriteRenderer.size = new Vector2(_spriteRenderer.size.x,
                        _startValue + Mathf.Lerp(-_upDownValue, _upDownValue, -(Mathf.Cos(Mathf.PI * percent) - 1) / 2));
                    yield return null;
                }

                currentTime = percent = 0;
                while (percent < 1)
                {
                    currentTime += Time.deltaTime;
                    percent = currentTime / time;
                    _spriteRenderer.size = new Vector2(_spriteRenderer.size.x,
                        _startValue + Mathf.Lerp(_upDownValue, -_upDownValue, -(Mathf.Cos(Mathf.PI * percent) - 1) / 2));
                    yield return null;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other) // 플레이어 닿으면 왔다갔다 멈추고 플레이어 끌어올림
        {
            if (other.CompareTag("Player"))
            {
                StopCoroutine(nameof(UpAndDown));
                StartCoroutine(Catch(other.transform));
            }
        }

        private IEnumerator Catch(Transform playerTrm = null)
        {
            float time = 1.5f;
            float currentTime = 0, percent = 0;
            float startValue = _spriteRenderer.size.y;
            
            while (percent < 1)
            {
                currentTime += Time.deltaTime;
                percent = currentTime / time;
                _spriteRenderer.size = new Vector3(_spriteRenderer.size.x,
                    Mathf.Lerp(startValue, 0, -(Mathf.Cos(Mathf.PI * percent) - 1) / 2));
                if (playerTrm is not null) playerTrm.position = _bobberTrm.position;
                yield return null;
            }

            if (playerTrm is not null) UIManager.Instance.RestartPanelOn();

            yield return new WaitForSeconds(4f);
            currentTime = percent = 0;
            time = 2;
            while (percent < 1) // 닊싯대 다시 내리기
            {
                currentTime += Time.deltaTime;
                percent = currentTime / time;
                _spriteRenderer.size = new Vector3(_spriteRenderer.size.x,
                    Mathf.Lerp(0, _startValue - _upDownValue, -(Mathf.Cos(Mathf.PI * percent) - 1) / 2));
                if (playerTrm is not null) playerTrm.position = _bobberTrm.position;
                yield return null;
            }

            StopCoroutine(nameof(UpAndDown));
            StartCoroutine(nameof(UpAndDown));
        }

        public override void Initialize()
        {
            StopCoroutine(nameof(UpAndDown));
            StartCoroutine(nameof(UpAndDown));
        }
    }
}
