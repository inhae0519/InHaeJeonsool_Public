using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YSCore
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager Instance;

        private GameObject[,] _stages = new GameObject[3, 6];
        
        public readonly bool[,] StageMap = new bool[3, 6] // 존재하는 스테이지는 true 아닌 것은 false
        {
            { true , true , true , true , true , true },
            { true , true , true , true , false, false },
            { true , false, false, false, false, false }
        };

        public int CurrentStage
        {
            get => _currentStage;
            set
            {
                int a = value / 10 - 1, b = value % 10 - 1;
                if (a < 0) a = 0; if (b < 0) b = 0;
                if (b >= 6 || StageMap[a, b] == false) // 들어온 값이 존재하지 않는 스테이지라면
                {
                    _currentStage = int.Parse($"{a + 2}{1}"); // 다음 챕터 1스테이지로
                }
                else  _currentStage = value; // 아님 그냥 그대로
            }
        }

        private int _currentStage;
        private GameObject _currentMap;
        
        private void Awake()
        {
            Instance ??= this;
            if (PlayerPrefs.GetInt("11") == 0) UnlockStage(11); // 1-1 스테이지가 잠겨있다면 풀어줌

            int cnt = 0;
            GameObject[] stages = Resources.LoadAll<GameObject>(@"Prefabs/Stages");
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 6; j++)
                    if (StageMap[i, j])
                        _stages[i, j] = stages[cnt++];
        }

        private void Start()
        {
            CurrentStage = PlayerPrefs.GetInt("Stage", 11);
        }

        public void UnlockStage(int idx) // 스테이지 Unlock!!
        {
            if (GetPoint(idx) != 0) return;
            int a = idx / 10, b = idx % 10;
            PlayerPrefs.SetInt($"{a}{b}", -1);
        }

        public void MoveStage(int idx) // 챕터화면에서 스테이지 선택
        {
            SoundManager.Instance.Play("Effect/ButtonClick");
            if (idx == 31)
            {
                CurrentStage = 31;
                PlayerPrefs.SetInt("Stage", CurrentStage);
                EndingStage();
                return;
            }
            int a = idx / 10, b = idx % 10;
            if (PlayerPrefs.GetInt($"{a}{b}") != 0)
            {
                YSUIManager.Instance.Fade(true, () =>
                {
                    PlayerPrefs.SetInt("Stage", idx);
                    CurrentStage = idx;
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    SceneManager.LoadScene("Stage");
                });
            }
        }

        private void EndingStage()
        {
            int cnt = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 6; j++)
                    if (StageMap[i, j])
                    {
                        int value = PlayerPrefs.GetInt($"{i + 1}{j + 1}");
                        cnt += value switch
                        {
                            -1 => 0,
                            _ => value
                        };
                    }

            if (cnt >= 30)
            {
                YSUIManager.Instance.Fade(true, () =>
                {
                    PlayerPrefs.SetInt("Stage", 31);
                    CurrentStage = 31;
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    SceneManager.LoadScene("Stage");
                });
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            int stage = PlayerPrefs.GetInt("Stage");
            _currentMap = Instantiate(_stages[stage / 10 - 1, stage % 10 - 1]);
            FishSingleton.Singleton.FindCameraBound();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        public void ToChapterScene() // 챕터 씬으로
        {
            Time.timeScale = 1;
            if (_currentMap is not null) Destroy(_currentMap);
            SoundManager.Instance.Clear();
            YSUIManager.Instance.Fade(true, () =>
            {
                SceneManager.LoadScene("Chapter");
            });
        }

        public void Retry() // 다시시도
        {
            Time.timeScale = 1;
            if (_currentMap is not null) Destroy(_currentMap);
            MoveStage(PlayerPrefs.GetInt("Stage"));
        }

        public void NextStage() // 다음 스테이지로
        {
            Time.timeScale = 1;
            if (_currentMap is not null) Destroy(_currentMap);
            CurrentStage++;
            UnlockStage(CurrentStage);
            MoveStage(CurrentStage);
        }

        public int GetPoint(int stage) // 현재 스테이지의 점수 Get!
        {
            int a = stage / 10 - 1, b = stage % 10 - 1;
            if (a < 0) a = 0; if (b < 0) b = 0;
            if (b >= 6 || StageMap[a, b] == false)
            {
                stage = int.Parse($"{a + 2}{1}");
            }
            
            return PlayerPrefs.GetInt(stage.ToString());
        }

        public void SetPoint(int point) // 현재 스테이지의 점수 Set!
        {
            point = point switch
            {
                0 => -1,
                -1 => 0,
                _ => point
            };
            int stage = PlayerPrefs.GetInt("Stage");
            if (PlayerPrefs.GetInt(stage.ToString()) >= point) return;
            PlayerPrefs.SetInt($"{stage / 10}{stage % 10}", point);
        }
    }
}