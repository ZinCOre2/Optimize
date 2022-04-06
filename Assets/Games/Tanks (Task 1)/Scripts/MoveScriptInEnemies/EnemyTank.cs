using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    private Transform _player;
    
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (_player == null) { return; }
            
        var playerPos = _player.position;
        
        if ((_player.position - transform.position).sqrMagnitude > 0.0025f)
        {
            transform.LookAt(playerPos);
            transform.Translate(0, 0, 0.05f);
        }
    }
}
