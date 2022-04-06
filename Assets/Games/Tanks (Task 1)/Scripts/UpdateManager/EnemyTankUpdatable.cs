using System;
using UnityEngine;


public class EnemyTankUpdatable : MonoBehaviour, IUpdatable
{
    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        
        UpdateManager.Instance.AddUpdatable(this);
    }

    public void OnUpdate()
    {
        if (_player == null) { return; }
            
        var playerPos = _player.position;
        
        if ((_player.position - transform.position).sqrMagnitude > 0.0025f)
        {
            transform.LookAt(playerPos);
            transform.Translate(0, 0, 0.05f);
        }
    }

    private void OnDestroy()
    {
        UpdateManager.Instance.RemoveUpdatable(this);
    }
}
