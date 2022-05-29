using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressRing : MonoBehaviour
{
    Color colour;
    float thickness;

    SpriteRenderer sprite_renderer;
    Material material;

    Timeline timeline;
    bool locked;

    public void Initialize(Color colour, float thickness, float radius,  float duration)
    {
        this.colour = colour;
        this.thickness = thickness;

        transform.localScale = NumTools.XY_Scale(radius * 2);
        timeline = new Timeline(duration);
    }

    public void Lock(Color colour, float thickness, float radius)
    {
        this.colour = colour;
        this.thickness = thickness;

        sprite_renderer.color = colour;
        transform.localScale = NumTools.XY_Scale(radius * 2);
        material.SetFloat("_Inner_Radius", 1 - thickness);
        material.SetFloat("_Outer_Radius", 1);

        locked = true;
    }

    void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        material = sprite_renderer.material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(locked){ return; }
        if(timeline == null){ return; }

        timeline.Tick(Time.fixedDeltaTime);

        float radius_scalar = NumTools.Powstep(timeline.progress, 0.35f);
        float inner_radius = radius_scalar - thickness;
        float outer_radius = radius_scalar;

        material.SetFloat("_Inner_Radius", inner_radius);
        material.SetFloat("_Outer_Radius", outer_radius);

        float alpha_scalar = NumTools.Powstep(timeline.progress, 6, true);
        colour.a *= alpha_scalar;
        sprite_renderer.color = colour;

        if(timeline.Evaluate())
        {
            Destroy(gameObject);
        }
    }
}
