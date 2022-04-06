using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    [SerializeField] private int numberOfTanks;
    [SerializeField] private Transform tankPrefab;
    
    private Transform[] _tanks;

    void Start()
    {
        _tanks = new Transform[numberOfTanks];

        for (int i = 0; i < numberOfTanks; i++)
        {
            _tanks[i] = Instantiate(tankPrefab);
            _tanks[i].position = new Vector3(Random.Range(-50,50), 0, Random.Range(-50,50));
        }
    }
}
