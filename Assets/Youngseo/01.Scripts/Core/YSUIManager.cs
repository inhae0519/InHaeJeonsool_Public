using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LeeInHae;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace YSCore
{
    public class YSUIManager : MonoBehaviour
    {
        public static YSUIManager Instance;
        
        private RectTransform _chaptersTrm;
        private Transform _stageClearPanel;
        private TextMeshProUGUI _stageText;
        private Image[] _stars;
        private Image _nextButton;
        private Image _fadePanel;
        private List<Image> _stageButtons = new();
        private List<TextMeshProUGUI> _stageNums = new();
        private List<List<Image>> _stageStars = new();
        [SerializeField] private Sprite[] _starImage;

        private bool _isActive;
        private bool _isTween;
        
        private void Awake()
        {
            Instance = this;
            SoundManager.Instance.Init();
            if (Equals(SceneManager.GetActiveScene().name, "Chapter")) SoundManager.Instance.Play("BGM/TitleBGM", Sound.Bgm);
            Transform canvasTrm = GameObject.Find("Canvas").transform;

            _chaptersTrm = canvasTrm.Find("Chapters").GetComponent<RectTransform>();
            _stageClearPanel = canvasTrm.Find("StageClearPanel");
            _stageText = _stageClearPanel.Find("StageName").GetComponent<TextMeshProUGUI>();
            _stars = _stageClearPanel.Find("Stars").GetComponentsInChildren<Image>();
            _nextButton = _stageClearPanel.Find("NextButton").GetComponent<Image>();
            _fadePanel = canvasTrm.Find("FadePanel").GetComponent<Image>();
            foreach (var grid in canvasTrm.GetComponentsInChildren<GridLayoutGroup>())
            {
                foreach (var button in grid.GetComponentsInChildren<Button>()
                            .ToList().ConvertAll(b => b.GetComponent<Image>()))
                {
                    _stageButtons.Add(button);
                    _stageNums.Add(button.GetComponentInChildren<TextMeshProUGUI>());
                    List<Image> images = button.GetComponentsInChildren<Image>().ToList();
                    images.Remove(button);
                    _stageStars.Add(images);
                }
            }
            
            InitStage();
            Fade(false);
        }

        private void InitStage() // 챕터 화면에서 잠긴 스테이지랑 점수 표현하는 거
        {
            if (_stageButtons.Count == 0) return;

            StageManager.Instance.CurrentStage = 11;
            for (int i = 0; StageManager.Instance.CurrentStage <= 31; i++, StageManager.Instance.CurrentStage++)
            {
                int point = StageManager.Instance.GetPoint(StageManager.Instance.CurrentStage);
                if (point != 0)
                {
                    _stageButtons[i].color = Color.white;
                    _stageNums[i].color = new Color(0.1f, 0.2f, 0.35f);
                }
                else
                {
                    _stageButtons[i].color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
                    _stageNums[i].color = new Color(0.2f, 0.25f, 0.3f);
                    for (int j = 0; j < 3; j++)
                        _stageStars[i][j].color = point > j ? Color.white : new Color(1, 1, 1, 0.75f);
                }
                
                for (int j = 0; j < 3; j++)
                    _stageStars[i][j].sprite = point > j ? _starImage[0] : _starImage[1];
            }
            
            int cnt = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 6; j++)
                    if (StageManager.Instance.StageMap[i, j])
                    {
                        int value = PlayerPrefs.GetInt($"{i + 1}{j + 1}");
                        cnt += value switch
                        {
                            -1 => 0,
                            _ => value
                        };
                    }
            
            _stageButtons[^1].color = cnt < 30 ? new Color(0.8f, 0.8f, 0.8f, 0.8f) : Color.white;
        }

        public void StageClear(int point) // 스테이지 클리어하면 점수 매개변수로 넣고 호출하면됨!
        {
            if (_isActive) return;
            UIManager.Instance.gameObject.SetActive(false);
            _isActive = true;
            StageManager.Instance.SetPoint(point);
            
            int stage = PlayerPrefs.GetInt("Stage");
            _stageText.text = $"{stage / 10} - {stage % 10} 스테이지";

            _nextButton.color = point > 0 ? Color.white : new Color(0.25f, 0.25f, 0.25f, 0.5f);
            
            StartCoroutine(DOScale());
            for (int i = 0; i < 3; i++)
                _stars[i].sprite = i < point ? _starImage[0] : _starImage[1];
            
            StageManager.Instance.UnlockStage(PlayerPrefs.GetInt("Stage") + 1);
        }

        private IEnumerator DOScale() // 스테이지 클리어 UI DOScale 코루틴
        {
            const float c4 = 1.5f * Mathf.PI / 3;
            float currentTime = 0, percent = 0, time = 0.75f;

            while (percent < 1)
            {
                currentTime += Time.deltaTime;
                percent = currentTime / time;

                float x = Mathf.Pow(2, -10 * percent) * Mathf.Sin((percent * 10 - .75f) * c4) + 1;
                _stageClearPanel.localScale = Vector3.one * x;
                yield return null;
            }

            _stageClearPanel.localScale = Vector3.one;
            Time.timeScale = 0;
        }

        public void ChapterMove(bool prev) // 챕터 왔다갔다 하는 거
        {
            if (_isTween) return;
            _isTween = true;
            
            float origin = _chaptersTrm.anchoredPosition.x;
            if (prev) _chaptersTrm.DOAnchorPosX(origin - 1920, 0.3f).OnComplete(() => _isTween = false)
                .SetEase(Ease.OutQuart);
            else _chaptersTrm.DOAnchorPosX(origin + 1920, 0.3f).OnComplete(() => _isTween = false)
                .SetEase(Ease.OutQuart);
        }

        public void Fade(bool value, Action callBack = null)
        {
            _fadePanel.DOFade(value ? 1 : 0, 1).OnComplete(() => callBack?.Invoke());
        }

        public void ToTitle()
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}