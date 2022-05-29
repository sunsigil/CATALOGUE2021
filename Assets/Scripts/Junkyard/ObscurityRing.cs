/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObscurityRing : MonoBehaviour
{
    [SerializeField]
    ShrineFlag shrine;

    [SerializeField]
    float radius;
    [SerializeField]
    float thickness;

    [SerializeField]
    float angular_velocity;

    SpriteRenderer sprite_renderer;
    Material material;
    float r;

    UnityEvent _on_dispelled;
    public UnityEvent on_dispelled => _on_dispelled;

    public float DistanceFromCenter(GameObject candidate)
    {
        return (transform.position - candidate.transform.position).magnitude;
    }

    public bool Envelopes(GameObject candidate)
    {
        return DistanceFromCenter(candidate) <= radius;
    }

    void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        material = sprite_renderer.material;

        _on_dispelled = new UnityEvent();

        transform.localScale = NumTools.XY_Scale(radius*2);

        thickness /= transform.localScale.x;
        r = 0.5f - thickness;

        material.SetFloat("_Inner_Radius", r);
        material.SetFloat("_Outer_Radius", 0.5f);
    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * angular_velocity * Time.fixedDeltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}*/
