using UnityEngine;

[System.Serializable]
public class GameObjectPool
{
    [SerializeField] private ObjectPoolType poolType;
    public ObjectPoolType PoolType => poolType;
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int amount;

    private GameObject[] _pool;
    private int _index;
    
    public void InitializePool()
    {
        _pool = new GameObject[amount];

        for (int i = 0; i < amount; i++)
        {
            var obj = Object.Instantiate(objectPrefab, ObjectPooler.Instance.transform);
            
            obj.SetActive(false);
            
            _pool[i] = obj;
        }
    }

    public GameObject GetObjectFromPool()
    {
        var obj = _pool[_index];
        obj.SetActive(true);
        IncrementIndex();
        
        return obj;
    }

    private void IncrementIndex()
    {
        _index++;
        if (_index >= amount)
        {
            _index -= amount;
        }
    }
}
