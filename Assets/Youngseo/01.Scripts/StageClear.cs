using System;
using Dohee;
using UnityEngine;
using YSCore;

namespace YS
{
    public class StageClear : MonoBehaviour
    {
        [Header("초단위 입니다")] [SerializeField] private float _seconds;

        [Header("크기 조건입니다")] [SerializeField] private float _goalScale;

        [Header("크기 제한입니다")] [SerializeField] private float _maxSize = 5;
        
        private FishScale _playerFish;
        private float _startTime;
        
        private void Start()
        {
            _startTime = Time.time;
            _playerFish = FishSingleton.Singleton.GetComponent<FishScale>();
        }

        private void LateUpdate()
        {
            _playerFish.Scale = Mathf.Clamp(_playerFish.Scale, 0, _maxSize);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                int point = 1;
                if (Time.time - _startTime < _seconds) point++;
                if (_playerFish.Scale > _goalScale) point++;
                YSUIManager.Instance.StageClear(point);
                SoundManager.Instance.Play("Effect/Clear");
            }
        }
    }
}