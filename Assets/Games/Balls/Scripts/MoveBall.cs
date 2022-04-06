using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveBall : MonoBehaviour
{
    Vector3 velocity;
    float sides = 30.0f;
    float speedMax = 0.3f;
    private string _color = "_Color";

    void Start()
    {
        velocity = new Vector3(Random.Range(0.0f, speedMax),
                        Random.Range(0.0f, speedMax),
                        Random.Range(0.0f, speedMax));
    }

    Color GetRandomColor()
    {
        var pos = transform.position;
        
        return new Color(pos.x/sides, pos.y/sides, pos.z/sides);
    }

    //Update is called once per frame
    void Update()
    {
        transform.Translate(velocity);
    
        if (transform.position.x > sides)
        {
            velocity.x = -velocity.x;
        }
        if (transform.position.x < -sides)
        {
            velocity.x = -velocity.x;
        }
        if (transform.position.y > sides)
        {
            velocity.y = -velocity.y;
        }
        if (transform.position.y < -sides)
        {
            velocity.y = -velocity.y;
        }
        if (transform.position.z > sides)
        {
            velocity.z = -velocity.z;
        }
        if (transform.position.z < -sides)
        {
            velocity.z = -velocity.z;
        }
    
        GetComponent<Renderer>().material.SetColor(_color, GetRandomColor());
    }
}
