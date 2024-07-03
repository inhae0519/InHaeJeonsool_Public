using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressSwich : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    [SerializeField] private Transform Door;

    private Vector2 ClosePos;
    private float Value = 1;

    private void Awake()
    {
        ClosePos = Door.localPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "ButtonTrigger")
        {
            Value *= -1;
            Door.DOLocalMove(new Vector3(ClosePos.x, ClosePos.y * Value), 1f);
        }
    }
}
