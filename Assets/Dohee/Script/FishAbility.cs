using DG.Tweening;
using Enemy;
using LeeInHae;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dohee
{
    public class FishAbility : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer Render;
        [SerializeField] private GameObject Range;
        private SpriteRenderer headRender;

        public FishAbilityState State = FishAbilityState.None;

        public Dictionary<FishAbilityState, Action> Skills = new();
        public Dictionary<FishAbilityState, Action> States = new();

        private bool activeAbility = false;

        private FishScale scale;
        private FishMovement move;

        [Header("Salmon")]
        [SerializeField] Sprite SalmonSprite;
        [SerializeField] GameObject SalmonEffect;
        [SerializeField] float SalmonTime = 5f;

        [Header("BlowFish")]
        [SerializeField] Sprite BlowfishSprite;
        [SerializeField] GameObject BlowFishEffect;
        [SerializeField] float BlowTime = 1.5f;

        [Header("Flyingfish")]
        [SerializeField] Sprite FlyingfishSprite;
        [SerializeField] GameObject FlyingFishEffect;
        [SerializeField] float FlyingTime = 5f;

        [Header("ElectricEel")]
        [SerializeField] Sprite ElectricEelSprite;
        [SerializeField] GameObject ElectricEelEffect;
        [SerializeField] float StrongElectricDistance;
        [SerializeField] float StunTime = 1f;

        [Header("BabyElectricEel")]
        [SerializeField] Sprite BabyElectricEelSprite;
        [SerializeField] GameObject BabyElectricEelEffect;
        [SerializeField] float WeakElectricDistance;

        private void Awake()
        {
            scale = GetComponent<FishScale>();
            move = GetComponent<FishMovement>();
            headRender = GetComponent<SpriteRenderer>();

            Skills[FishAbilityState.None] = None;
            Skills[FishAbilityState.Salmon] = Salmon;
            Skills[FishAbilityState.Blowfish] = Blowfish;
            Skills[FishAbilityState.Flyingfish] = Flyingfish;
            Skills[FishAbilityState.ElectricEel] = ElectricEel;
            Skills[FishAbilityState.BabyElectricEel] = BabyElectricEel;

            States[FishAbilityState.None] = NoneState;
            States[FishAbilityState.ElectricEel] = ElectricEelState;
            States[FishAbilityState.BabyElectricEel] = BabyElectricEelState;
        }

        private void NoneState()
        {
            Range.SetActive(false);
        }

        private void ElectricEelState()
        {
            Range.SetActive(true);
            Range.transform.localScale = Vector3.one * StrongElectricDistance * (1f / transform.localScale.x) * 2;
        }

        private void BabyElectricEelState()
        {
            Range.SetActive(true);
            Range.transform.localScale = Vector3.one * WeakElectricDistance * (1f / transform.localScale.x) * 2;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !activeAbility)
            {
                Skills[State]?.Invoke();
                State = FishAbilityState.None;
            }

            if (States.ContainsKey(State))
            {
                States[State]?.Invoke();
                UIManager.Instance.SetImage(State);
            }

            Render.flipY = headRender.flipY;
        }

        public void None()
        {

        }

        public void Salmon()
        {
            move.Torrent = true;
            activeAbility = true;

            Render.transform.localScale = Vector3.one * 0.5f;
            Render.flipX = true;
            Render.sprite = SalmonSprite;
            Render.color = new Color(1f, 1f, 1f, 0.8f);
            Render.DOFade(0f, SalmonTime);
            
            SoundManager.Instance.Play("Effect/Salmon");

            StartCoroutine(Wait(SalmonTime-0.15f, () =>
            {
                move.Torrent = false;
                activeAbility = false;

                Render.flipX = false;
                Render.sprite = null;

                Render.transform.localScale = Vector3.one;
            }));
        }

        public void Blowfish()
        {
            activeAbility = true;

            Render.transform.localScale = Vector3.one;
            Render.sprite = BlowfishSprite;
            Render.color = new Color(1f, 1f, 1f, 1f);
            Render.DOFade(0f, BlowTime);

            BlowFishEffect.SetActive(true);
            BlowFishEffect.transform.localScale = Vector3.one * Mathf.Clamp(scale.Scale * 5, 0, 10);

            scale.CanModify = false;

            SoundManager.Instance.Play("Effect/Shield");

            StartCoroutine(Wait(BlowTime, () =>
            {
                activeAbility = false;

                BlowFishEffect.SetActive(false);

                scale.CanModify = true;

                Render.sprite = null;
            }));
        }

        public void Flyingfish()
        {
            activeAbility = true;

            Render.transform.localScale = Vector3.one * 0.5f;
            Render.sprite = FlyingfishSprite;
            Render.color = new Color(1f, 1f, 1f, 0.8f);
            Render.DOFade(0f, FlyingTime);

            move.Flying = true;

            StartCoroutine(Wait(FlyingTime-0.15f, () =>
            {
                activeAbility = false;
                move.Flying = false;

                Render.sprite = null;
                Render.transform.localScale = Vector3.one;
            }));
        }

        public void ElectricEel()
        {
            activeAbility = true;

            Render.sprite = ElectricEelSprite;
            Render.color = new Color(1f, 1f, 1f, 1f);
            Render.DOFade(0, 0.3f);

            Instantiate(ElectricEelEffect, transform.position, transform.rotation);

            Collider2D[] enemys = Physics2D.OverlapCircleAll(transform.position, StrongElectricDistance);

            foreach (Collider2D col in enemys)
            {
                if (col.TryGetComponent(out AIBrain ai))
                {
                    ai.Stun(StunTime);
                }
                else if (col.transform.parent.TryGetComponent(out FishingHook hook))
                {
                    hook.Reroad();
                }
            }

            SoundManager.Instance.Play("Effect/SpecialFish/Electricity");

            StartCoroutine(Wait(0.15f, () =>
            {
                activeAbility = false;

                Render.sprite = null;
                Render.transform.localScale = Vector3.one;
            }));
        }

        public void BabyElectricEel()
        {
            activeAbility = true;

            Render.transform.localScale = Vector3.one * 0.5f;
            Render.sprite = BabyElectricEelSprite;
            Render.color = new Color(1f, 1f, 1f, 1f);
            Render.DOFade(0, 0.3f);

            Instantiate(BabyElectricEelEffect, transform.position, transform.rotation);

            Collider2D[] interactives = Physics2D.OverlapCircleAll(transform.position, WeakElectricDistance);

            foreach (Collider2D col in interactives)
            {
                if (col.TryGetComponent(out IInteractive interactive))
                {
                    Debug.Log("Interactive");
                    interactive.Interactive();
                }
            }

            SoundManager.Instance.Play("Effect/Electricity");

            StartCoroutine(Wait(0.3f, () =>
            {
                activeAbility = false;

                Render.sprite = null;
                Render.transform.localScale = Vector3.one;
            }));
        }

        private IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);

            callback.Invoke();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
