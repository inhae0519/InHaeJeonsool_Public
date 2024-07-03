using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace LeeInHae
{
    public class StartSceneManager : MonoBehaviour
    {
        public static StartSceneManager Instance;

        [SerializeField] private Transform canvas;
        private GameObject titlePanel;
        private GameObject settingPanel;
        private GameObject exitCheckPanel;
        private GameObject currentPanel;
        private GameObject helpPanel;
        private GameObject nextHelpButton;
        private GameObject backHelpButton;
        private GameObject currentHelpText;

        private Transform blackPanel;

        private Slider bgmSlider;
        private Slider effectSlider;

        private TextMeshProUGUI bgmPercent;
        private TextMeshProUGUI effectPercent;

        private bool panelOn;
        private bool titleOn;
        private float titlePanelFadeValue = 750;
        private int currentHelp = 1;

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

            SoundManager.Instance.Init();
            SoundManager.Instance.Clear();
            SoundManager.Instance.Play("BGM/TitleBGM", Sound.Bgm);

            Init();
        }

        private void Start()
        {
            bgmSlider.value = SoundManager.Instance.BgmSourceValue;
            effectSlider.value = SoundManager.Instance.EffectSourceValue;
        }

        private void Update()
        {
            SoundManager.Instance.RegulateSound(Sound.Bgm, (SoundManager.Instance.bgmValue + 1) * bgmSlider.value);
            SoundManager.Instance.RegulateSound(Sound.Effect, (SoundManager.Instance.effectValue + 1) * effectSlider.value);
            bgmPercent.SetText(((SoundManager.Instance.bgmValue + 1) * 10 * bgmSlider.value).ToString("F0") + "%");
            effectPercent.SetText(((SoundManager.Instance.effectValue + 1) * 10 * effectSlider.value).ToString("F0") + "%");
        }

        private void Init()
        {
            titlePanel = canvas.Find("TitlePanel").gameObject;
            settingPanel = canvas.Find("SettingPanel").gameObject;
            exitCheckPanel = canvas.Find("ExitCheckPanel").gameObject;
            bgmSlider = settingPanel.transform.Find("BackGroundSoundSetting/BackSoundSlider").GetComponent<Slider>();
            bgmPercent = settingPanel.transform.Find("BackGroundSoundSetting/PercentText")
                .GetComponent<TextMeshProUGUI>();
            effectSlider = settingPanel.transform.Find("EffectSoundSetting/EffectSoundSlider").GetComponent<Slider>();
            effectPercent = settingPanel.transform.Find("EffectSoundSetting/PercentText")
                .GetComponent<TextMeshProUGUI>();
            blackPanel = canvas.Find("BlackPanel");
            helpPanel = canvas.Find("HelpPanel").gameObject;
            nextHelpButton = helpPanel.transform.Find("NextButton").gameObject;
            backHelpButton = helpPanel.transform.Find("BackButton").gameObject;
            currentHelpText = helpPanel.transform.Find("Help1").gameObject;
        }

        public void StartButton()
        {
            SoundManager.Instance.Play("Effect/ButtonClick");
            if (!panelOn)
            {
                blackPanel.GetComponent<Image>().DOColor(new Color(0, 0, 0, 1), 1).OnComplete(() =>
                {
                    SoundManager.Instance.Clear();
                    SceneManagerment.Instance.SceneChange(6);
                });
                SoundManager.Instance.EffectSourceValue = effectSlider.value;
                SoundManager.Instance.BgmSourceValue = bgmSlider.value;
            }
        }

        public void SettingPanelOn()
        {
            SoundManager.Instance.Play("Effect/ButtonClick");
            if (!panelOn)
            {
                panelOn = true;
                currentPanel = settingPanel;
                TitlePanelFadeOut();
                settingPanel.SetActive(true);
                settingPanel.transform.DOScale(Vector3.one, 0.5f);
            }
        }

        public void HelpPanelOn()
        {
            SoundManager.Instance.Play("Effect/ButtonClick");
            if (!panelOn)
            {
                panelOn = true;
                currentPanel = helpPanel;
                TitlePanelFadeOut();
                helpPanel.SetActive(true);
                helpPanel.transform.DOScale(Vector3.one, 0.5f);
                currentHelpText.SetActive(false);
                currentHelp = 1;
                HelpChange(currentHelp);
                nextHelpButton.SetActive(true);
            }
        }

        public void HelpNext()
        {
            SoundManager.Instance.Play("Effect/ButtonClick");
            currentHelpText.SetActive(false);
            currentHelp++;
            backHelpButton.SetActive(true);
            HelpChange(currentHelp);
        }
        
        public void HelpBack()
        {
            SoundManager.Instance.Play("Effect/ButtonClick");
            currentHelpText.SetActive(false);
            currentHelp--;
            nextHelpButton.SetActive(true);
            HelpChange(currentHelp);
        }

        private void HelpChange(int index)
        {
            currentHelp = Mathf.Clamp(currentHelp, 1, 5);
            if(currentHelp == 5)
                nextHelpButton.SetActive(false);
            else if(currentHelp == 1)
                backHelpButton.SetActive(false);
            currentHelpText = helpPanel.transform.Find("Help" + index).gameObject;
            currentHelpText.SetActive(true);
        }

        public void ExitCheckPanelOn()
        {
            SoundManager.Instance.Play("Effect/ButtonClick");
            if (!panelOn)
            {
                panelOn = true;
                currentPanel = exitCheckPanel;
                TitlePanelFadeOut();
                exitCheckPanel.SetActive(true);
                exitCheckPanel.transform.DOScale(Vector3.one, 0.5f);
            }
        }

        public void CurrentPanelOff()
        {
            SoundManager.Instance.Play("Effect/ButtonClick");
            if (!titleOn)
            {
                currentPanel.transform.DOScale(Vector3.zero, 0.45f).OnComplete(() =>
                {
                    currentPanel.SetActive(false);
                    panelOn = false;
                });
                TitlePanelFadeIn();
            }
        }

        public void ExitCheckYes()
        {
            SoundManager.Instance.Play("Effect/ButtonClick");
            Application.Quit();
        }

        private void TitlePanelFadeOut()
        {
            RectTransform rectTransform = titlePanel.GetComponent<RectTransform>();
            rectTransform.DOAnchorPos(
                new Vector2(rectTransform.anchoredPosition.x - titlePanelFadeValue, rectTransform.anchoredPosition.y), 0.5f).
                OnComplete(() =>
                {
                    buttonRayCastOff?.Invoke();
                });
        }

        private void TitlePanelFadeIn()
        {
            RectTransform rectTransform = titlePanel.GetComponent<RectTransform>();
            rectTransform.DOAnchorPos(
                new Vector2(rectTransform.anchoredPosition.x + titlePanelFadeValue, rectTransform.anchoredPosition.y), 0.45f).
                OnComplete(() =>
                {
                    buttonRayCastOn?.Invoke();
                    titleOn = false;
                });
        }
    }
}
