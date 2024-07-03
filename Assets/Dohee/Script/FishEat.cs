using LeeInHae;
using UnityEngine;

namespace Dohee
{
    public class FishEat : MonoBehaviour
    {
        [SerializeField] private float EdibleScale = 0.2f;

        private FishScale fishScale;
        private FishAbility fishAbility;

        private void Awake()
        {
            fishScale = GetComponent<FishScale>();
            fishAbility = GetComponent<FishAbility>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out FishBody part))
            {
                if (part.head.TryGetComponent(out FishScale scale))
                {
                    if (fishScale.Scale - 0.2f <= scale.Scale) return;
                }

                if (part.head.TryGetComponent(out FishEdible edible))
                {
                    if (fishScale != null)
                    {
                        fishScale.Scale += edible.GivenSize * 1.5f;
                        
                    }
                    if (fishAbility != null && edible.AbilityState != FishAbilityState.None)
                    {
                        fishAbility.State = edible.AbilityState;
                        
                        UIManager.Instance.SetImage(fishAbility.State);
                    }

                    if (part.head.TryGetComponent(out Poolable poolable))
                    {
                        SoundManager.Instance.Play("Effect/Eat");
                        PoolManager.Singleton.Push(poolable.Type, poolable);
                        return;
                    }

                    foreach (Transform p in part.head.bodys)
                    {
                        Destroy(p.gameObject);
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out FishBody part))
            {
                if (part.head.TryGetComponent(out FishScale scale))
                {
                    if (fishScale.Scale * EdibleScale <= scale.Scale) return;
                }

                if (part.head.TryGetComponent(out FishEdible edible))
                {
                    if (fishScale != null)
                        fishScale.Scale += edible.GivenSize;
                    if (fishAbility != null && edible.AbilityState != FishAbilityState.None)
                        fishAbility.State = edible.AbilityState;

                    if (part.head.TryGetComponent(out Poolable poolable))
                    {
                        SoundManager.Instance.Play("Effect/Moss");
                        PoolManager.Singleton.Push(poolable.Type, poolable);
                        return;
                    }

                    foreach (Transform p in part.head.bodys)
                    {
                        Destroy(p.gameObject);
                    }
                }
            }
        }
    }
}
