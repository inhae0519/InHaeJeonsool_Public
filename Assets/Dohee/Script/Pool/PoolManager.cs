using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dohee
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Singleton;

        private Dictionary<PoolType, Queue<Poolable>> pools = new();
        [SerializeField] private Pair[] poolables;

        private void Awake()
        {
            if (Singleton != null) Destroy(gameObject);
            Singleton = this;

            foreach(var p in poolables)
            {
                pools[p.type] = new Queue<Poolable>();
            }
        }

        public void Push(PoolType type, Poolable poolable)
        {
            poolable.gameObject.SetActive(false);
            poolable.transform.parent = transform;

            pools[type].Enqueue(poolable);
        }

        public Poolable Pool(PoolType type)
        {
            Poolable poolable;

            if (pools[type].Count > 0)
            {
                poolable = pools[type].Dequeue();

                poolable.gameObject.SetActive(true);
                poolable.transform.parent = null;
            }
            else poolable = Instantiate(Find(type), null).GetComponent<Poolable>();

            poolable.Initialize();
            return poolable;
        }

        public Poolable Pool(PoolType type, Transform parent)
        {
            Poolable poolable;

            if (pools[type].Count > 0)
            {
                poolable = pools[type].Dequeue();

                poolable.gameObject.SetActive(true);
                poolable.transform.parent = parent;
            }
            else poolable = Instantiate(Find(type), parent).GetComponent<Poolable>();

            poolable.Initialize();
            return poolable;
        }

        private GameObject Find(PoolType type)
        {
            foreach(Pair pair in poolables)
            {
                if (pair.type == type)
                    return pair.poolable;
            }

            return null;
        }
    }

    [System.Serializable]
    public class Pair
    {
        public PoolType type;
        public GameObject poolable;
    }
}
