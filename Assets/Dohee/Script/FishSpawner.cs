using Dohee;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private PoolType type;

    [Header("Value")]
    [SerializeField] float MaxChildCount = 1;
    [Space]
    [SerializeField] float SpawnMinScale = 0.5f;
    [SerializeField] float SpawnMaxScale = 3f;

    [Header("Spawn")]

    [SerializeField] float SpawnDelay = 5f;
    [SerializeField] float SpawnRandomDelay = 1.5f;
    [SerializeField] float SpawnRandomRangeX = 1.5f;
    [SerializeField] float SpawnRandomRangeY = 1.5f;

    private float temp = 0;
    private float delay = 0;

    private void Awake()
    {
        delay = SpawnDelay + Random.Range(-SpawnRandomDelay, SpawnRandomDelay);
    }

    private void Update()
    {
        if(transform.childCount < MaxChildCount && FishSingleton.Singleton.transform != null && Vector2.Distance(FishSingleton.Singleton.transform.position, transform.position) < 50)
        {
            temp += Time.deltaTime;

            if(temp > delay)
            {
                Transform fish = PoolManager.Singleton.Pool(type, transform).transform;
                fish.position = new Vector2(transform.position.x + Random.Range(-SpawnRandomRangeX, SpawnRandomRangeX), transform.position.y + Random.Range(-SpawnRandomRangeY, SpawnRandomRangeY));

                temp = 0;
                delay = SpawnDelay + Random.Range(-SpawnRandomDelay, SpawnRandomDelay);

                FishScale scale = fish.GetComponent<FishScale>();

                scale.Scale = Random.Range(SpawnMinScale, SpawnMaxScale);
            }
        }
    }
}
