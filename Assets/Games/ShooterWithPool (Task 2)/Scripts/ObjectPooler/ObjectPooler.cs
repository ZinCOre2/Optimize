using System;
using UnityEngine;


public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    
    [SerializeField] private GameObjectPool[] pools;

    private void Awake()
    {
        CreateInstance();
        InitializePools();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void InitializePools()
    {
        foreach (var pool in pools)
        {
            pool.InitializePool();
        }
    }

    public GameObject GetObjectFromPool(ObjectPoolType poolType)
    {
        foreach (var pool in pools)
        {
            if (pool.PoolType == poolType)
            {
                return pool.GetObjectFromPool();
            }
        }

        Debug.Log("ObjectPooler error: no object pool of required type");
        return null;
    }
}
