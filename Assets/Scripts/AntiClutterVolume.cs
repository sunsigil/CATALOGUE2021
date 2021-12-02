using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiClutterVolume : MonoBehaviour
{
    float _width; public float width => _width;
    float back;
    float front;

    public bool IsViolating(Clutter clutter)
    {
        if(clutter.back < back && clutter.front < back){return false;}
        if(clutter.back > front && clutter.front > front){return false;}

        return true;
    }

    public float NextBestX(Clutter clutter)
    {
        return front + clutter.width/2;
    }

    void Awake()
    {
        _width = transform.localScale.x;
        back = transform.position.x - _width/2;
        front = transform.position.x + _width/2;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
