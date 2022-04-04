using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]

public class Usable : MonoBehaviour
{
    [SerializeField]
    [Range(1, 3)]
    float use_radius;
    [SerializeField]
    Vector3 prompt_offset;
    [SerializeField]
    float prompt_radius;

    CircleCollider2D collider;

    Transform _user;
    public Transform user => _user;
    Distline _distline;
    public Distline distline => _distline;

    UnityEvent _on_used;
    public UnityEvent on_used => _on_used;
    public float usability => _distline.progress;

    bool _show_prompt;
    public bool show_prompt
    {
        get => _show_prompt;
        set => _show_prompt = value;
    }

    public void RequestUse()
    {
        if(usability >= 1)
        {
            _on_used.Invoke();
        }
    }

    void Awake()
    {
        collider = GetComponent<CircleCollider2D>();

        _on_used = new UnityEvent();

        _user = GameObject.FindObjectOfType<User>().transform;
        _distline  = new Distline(_user, transform, use_radius, 3);
    }

    void Update()
    {
        if(show_prompt && usability >= 1)
        {
            Vector3 prompt_center = transform.position + NumTools.XY_Pos(collider.offset) + prompt_offset;
            Vector3 dir = NumTools.XY_Pos(_user.position - prompt_center).normalized;
            Vector3 arm = dir * prompt_radius;
            Vector3 pos = prompt_center + arm;
            InputPrompter._.Request(InputCode.CONFIRM, pos);
        }
    }

    void OnDrawGizmos()
    {
        collider = GetComponent<CircleCollider2D>();

        Vector3 center = transform.position + NumTools.XY_Pos(collider.offset);
        Vector3 use_arm = Vector3.right * use_radius;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(center - use_arm, center + use_arm);
    }
}
