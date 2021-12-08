using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiClutterVolume : MonoBehaviour
{
    [SerializeField]
    float tolerance;

    float _width; public float width => _width;
    float back;
    float front;

    public bool IsViolating(Clutter clutter)
    {
        float b_b_difference = clutter.back - back;
        float f_b_difference = clutter.front - back;
        float b_f_difference = clutter.back - front;
        float f_f_difference = clutter.front - front;

        if(b_b_difference <= 0 && f_b_difference <= tolerance){return false;}
        if(b_f_difference >= -tolerance && f_f_difference >= 0){return false;}

        return true;
    }

    public float NextBestX(Clutter clutter)
    {
        return front + clutter.width/2 + 0.25f;
    }

    void Awake()
    {
        _width = transform.localScale.x;
        back = transform.position.x - _width/2;
        front = transform.position.x + _width/2;
    }

    void OnDrawGizmos()
    {
        Vector3 tolerance_scale = new Vector3(transform.localScale.x - 2*tolerance, transform.localScale.y, transform.localScale.z);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, tolerance_scale);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
