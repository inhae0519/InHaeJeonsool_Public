using Dohee;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moss : Poolable
{
    [SerializeField] private float SpawnRandomValue = 0.1f;

    private FishScale scale;

    private void Awake()
    {
        scale = GetComponent<FishScale>();
        Type = PoolType.Moss;
    }

    public override void Initialize()
    {
        scale.SetScale(scale.DefaultSize + Random.Range(-SpawnRandomValue, SpawnRandomValue));
    }
}
