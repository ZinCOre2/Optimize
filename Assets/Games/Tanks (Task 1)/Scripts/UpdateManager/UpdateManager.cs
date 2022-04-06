using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class UpdateManager : MonoBehaviour
{
    public static UpdateManager Instance;

    [SerializeField] private int initialArraySize = 100;
    
    private IUpdatable[] _updatables;
    private int _index;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _updatables = new IUpdatable[initialArraySize];
    }

    public void AddUpdatable(IUpdatable updatable)
    {
        if (_index >= _updatables.Length)
        {
            Array.Resize(ref _updatables, _index + 1);
        }
        
        _updatables[_index] = updatable;
        _index++;
    }
    
    public void RemoveUpdatable(IUpdatable updatable)
    {
        int i;
        // bool isFound = false;
        
        _index--;
        
        for (i = 0; i < _index; i++)
        {
            if (_updatables[i] != null && _updatables[i].Equals(updatable))
            {
                _updatables[i] = null;
                //isFound = true;
            }
        
            // if (isFound)
            // {
            //     _updatables[i] = _updatables[i + 1];
            // }
        }
    }

    private void Update()
    {
        foreach (var upd in _updatables)
        {
            if (upd == null) { return; }
            
            upd.OnUpdate();
        }
    }
}
