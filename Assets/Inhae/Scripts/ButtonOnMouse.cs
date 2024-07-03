using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeeInHae
{
    public class ButtonOnMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image _image;
        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            if (UIManager.Instance)
            {
                UIManager.Instance.buttonRayCastOff += () => _image.raycastTarget = false;
                UIManager.Instance.buttonRayCastOn += () => _image.raycastTarget = true;
            }
            else if (StartSceneManager.Instance)
            {
                StartSceneManager.Instance.buttonRayCastOff += () => _image.raycastTarget = false;
                StartSceneManager.Instance.buttonRayCastOn += () => _image.raycastTarget = true;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(Vector3.one, 0.3f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(new Vector3(0.8f,0.8f, 1f), 0.3f);
        }
    }
}
