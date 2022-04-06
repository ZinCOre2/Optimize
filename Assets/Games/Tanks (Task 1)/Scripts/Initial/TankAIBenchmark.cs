using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAIBenchmark : MonoBehaviour
{
    [SerializeField] private Transform player;
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
    
    void Update()
    {
        foreach (var t in _tanks)
        {
            if (player == null) { return; }
            
            var playerPos = player.position;
            
            if ((player.position - t.position).sqrMagnitude > 0.0025f)
            {
                t.LookAt(playerPos);
                t.Translate(0, 0, 0.05f);
            }
        } 
    }
}
