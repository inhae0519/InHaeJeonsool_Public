using System.Collections;
using UnityEngine;

namespace YSUI
{
    public class DummyFish : MonoBehaviour
    {
        private RectTransform _transform;
        [SerializeField] private float _startDelay = 0;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _distance = 2250f;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            StartCoroutine(nameof(FishMove));
        }

        private IEnumerator FishMove()
        {
            yield return new WaitForSeconds(_startDelay);
            
            float x = 660;
            while (true)
            {
                x += Time.deltaTime * 100 * _speed;
                _transform.localPosition = new Vector3(Mathf.Repeat(x, _distance + 960) - 960, _transform.localPosition.y, _transform.localPosition.z);
                yield return null;
            }
        }
    }
}