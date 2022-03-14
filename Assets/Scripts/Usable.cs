using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]

public class Usable : MonoBehaviour
{
    [SerializeField]
    float request_radius;
    [SerializeField]
    float use_radius;

    protected Transform _user;
    public Transform user => _user;
    protected Distline _distline;
    public Distline distline => _distline;

    UnityEvent _on_used;
    public UnityEvent on_used => _on_used;
    public float usability => _distline.progress;

    public void RequestUse()
    {
        print(name);

        if(usability >= 1)
        {
            _on_used.Invoke();
        }
    }

    void Awake()
    {
        _on_used = new UnityEvent();

        _user = GameObject.FindWithTag("Player").transform;
        _distline  = new Distline(_user, transform, use_radius, request_radius);
    }

    void OnDrawGizmos()
    {
        Vector3 center = transform.position;
        Vector3 request_arm = Vector3.right * request_radius;
        Vector3 use_arm = Vector3.right * use_radius;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(center - request_arm, center + request_arm);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(center - use_arm, center + use_arm);
    }
}
