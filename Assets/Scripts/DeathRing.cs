using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRing : MonoBehaviour
{
    [SerializeField]
    [Range(0, 0.5f)]
    float radius;

    [SerializeField]
    float duration;

    SpriteRenderer sprite_renderer;

    Material material;
    Color colour;

    float timer;

    void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();

        material = sprite_renderer.material;
        colour = sprite_renderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;

        float radius_scalar = Mathf.Pow(progress, 0.35f);

        float thickness = 0.01f;
        float inner_radius = radius * radius_scalar - thickness;
        float outer_radius = radius * radius_scalar;

        material.SetFloat("_Inner_Radius", inner_radius);
        material.SetFloat("_Outer_Radius", outer_radius);

        float alpha_scalar = 1-Mathf.Pow(progress, 6);
        colour.a *= alpha_scalar;
        sprite_renderer.color = colour;

        if(timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
