    using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YSCore;

namespace LeeInHae
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [SerializeField] private Transform canvas;
            
        private GameObject pausePanel;
        private GameObject settingPanel;
        private GameObject exitCheckPanel;
        private GameObject currentPanel;
        private GameObject restartPanel;
        private GameObject restartButtonPanel;
        private Transform blackPanel;
        
        private Slider bgmSlider;
        private Slider effectSlider;

        private TextMeshProUGUI bgmPercent;
        private TextMeshProUGUI effectPercent;

        private Transform fishStateImageTrm;
        private Transform currentStateImageTrm;
        
        private bool pausePanelOn;
        private bool panelOn;
        private bool isStarting;

        public Action buttonRayCastOff;
        public Action buttonRayCastOn;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            Init();
            SoundManager.Instance.Init();
        }

        private void Start()
        {
            StartCoroutine(startWaitRoutine());
            currentStateImageTrm = fishStateImageTrm.Find(FishAbilityState.None.ToString());
            bgmSlider.value = SoundManager.Instance.BgmSourceValue;
            effectSlider.value = SoundManager.Instance.EffectSourceValue;
        }

        private void Init()
        {
            pausePanel = canvas.transform.Find("PauseObj").gameObject;
            settingPanel = pausePanel.transform.Find("SettingPanel").gameObject;
            exitCheckPanel = pausePanel.transform.Find("ExitCheckPanel").gameObject;
            bgmSlider = settingPanel.transform.Find("BackGroundSoundSetting/BackSoundSlider").GetComponent<Slider>();
            bgmPercent = settingPanel.transform.Find("BackGroundSoundSetting/PercentText")
                .GetComponent<TextMeshProUGUI>();
            effectSlider = settingPanel.transform.Find("EffectSoundSetting/EffectSoundSlider").GetComponent<Slider>();
            effectPercent = settingPanel.transform.Find("EffectSoundSetting/PercentText")
                .GetComponent<TextMeshProUGUI>();
            fishStateImageTrm = canvas.transform.Find("FishStateImages");
            restartPanel = canvas.transform.Find("RestartPanel").gameObject;
            restartButtonPanel = canvas.transform.Find("RestartPanel2").gameObject;
            blackPanel = canvas.transform.Find("BlackPanel");
        }
        
        private void Update()
        {
            SoundManager.Instance.RegulateSound(Sound.Bgm, (SoundManager.Instance.bgmValue+1) * bgmSlider.value);
            SoundManager.Instance.RegulateSound(Sound.Effect, (SoundManager.Instance.effectValue+1) * effectSlider.value);
            bgmPercent.SetText(((SoundManager.Instance.bgmValue+1) *10 * bgmSlider.value).ToString("F0") + "%");
            effectPercent.SetText(((SoundManager.Instance.effectValue+1)*10 * effectSlider.value).ToString("F0") + "%");

            if (!isStarting)
                return;
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!pausePanelOn)
                {
                    pausePanelOn = true;
                    pausePanel.SetActive(true);
                }
                else if(panelOn)
                {
                    CurrentPanelOff();
                }
                else
                {
                    pausePanelOn = false;
                    pausePanel.SetActive(false);
                }
            }
        }

        public void PauseButton()
        {
            if (!pausePanelOn)
            {
                pausePanelOn = true;
                pausePanel.SetActive(true);
            }
            else
            {
                pausePanelOn = false;
                pausePanel.SetActive(false);
            }
        }

        public void ResumeButton()
        {
            pausePanel.SetActive(false);
        }

        public void SettingPanelOn()
        {
            if (!panelOn)
            { 
                panelOn = true;
                currentPanel = settingPanel;
                buttonRayCastOff?.Invoke();
                settingPanel.SetActive(true);
                settingPanel.transform.DOScale(Vector3.one, 0.5f).SetUpdate(true);
            }
        }
        
        public void ExitCheckPanelOn()
        {
            if (!panelOn)
            {
                panelOn = true;
                currentPanel = exitCheckPanel;
                buttonRayCastOff?.Invoke();
                exitCheckPanel.SetActive(true);
                exitCheckPanel.transform.DOScale(Vector3.one, 0.5f).SetUpdate(true);
            }
        }
        public void CurrentPanelOff()
        {
            currentPanel.transform.DOScale(Vector3.zero, 0.5f).SetUpdate(true).OnComplete(()=>
            {
                buttonRayCastOn?.Invoke();
                currentPanel.SetActive(false);
                panelOn = false;
            });
        }

        public void GoTitleYes()
        {
            SceneManagerment.Instance.SceneChange("StartScene");
        }

        public void SetImage(FishAbilityState fishAbilityState)
        {
            currentStateImageTrm.gameObject.SetActive(false);
            currentStateImageTrm = fishStateImageTrm.Find(fishAbilityState.ToString());
            currentStateImageTrm.gameObject.SetActive(true);
        }

        public void RestartPanelOn()
        {
            restartPanel.SetActive(true);
            restartPanel.transform.DOScale(Vector3.one, 0.5f).SetUpdate(true).OnComplete(
                () =>
                {
                    SoundManager.Instance.Clear();
                    SoundManager.Instance.Play("Effect/Fail");
                });
        }

        public void RestartButton()
        {
            restartButtonPanel.SetActive(true);
            restartButtonPanel.transform.DOScale(Vector3.one, 0.5f).SetUpdate(true);
        }

        public void RestartButtonPanelNo()
        {
            restartButtonPanel.transform.DOScale(Vector3.zero, 0.5f).SetUpdate(true).OnComplete(()=>restartButtonPanel.SetActive(false));

        }
        
        public void RestartYes()
        {
            blackPanel.gameObject.SetActive(true);
            blackPanel.transform.DOScale(Vector3.one, 0.5f).SetUpdate(true).OnComplete(()=>
            {
                StageManager.Instance.Retry();
            });
        }

        public void RestartNo()
        {
            blackPanel.gameObject.SetActive(true);
            blackPanel.transform.DOScale(Vector3.one, 0.5f).SetUpdate(true).OnComplete(() =>
            {
                SoundManager.Instance.Clear();
                SceneManagerment.Instance.SceneChange(1);
            });
        }

        private IEnumerator startWaitRoutine()
        {
            yield return new WaitForSeconds(1f);
            blackPanel.transform.DOScale(Vector3.zero, 0.5f).SetUpdate(true).OnComplete(() => isStarting = true);
        }
    }
}
