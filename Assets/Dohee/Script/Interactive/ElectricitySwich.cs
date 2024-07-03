using DG.Tweening;
using UnityEngine;

public class ElectricitySwich : MonoBehaviour, IInteractive
{
    [SerializeField] private Transform Door;

    private bool isOpening;
    private Vector2 ClosePos;
    private float Value = -8.5f;

    private void Awake()
    {
        ClosePos = Door.localPosition;
    }

    public void Interactive()
    {
        if(isOpening)
            return;
        
        isOpening = true;            
        Value *= -1;
        Door.DOLocalMove(new Vector3(ClosePos.x, ClosePos.y + Value), 1f)
            .OnComplete(() =>
            {
                ClosePos = Door.localPosition;
                isOpening = false;
            });
    }
}
