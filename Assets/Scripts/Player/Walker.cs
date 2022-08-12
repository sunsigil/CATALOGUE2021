using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subcontroller which handles
/// the player's sidescrolling
/// worlspace movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Walker : Subcontroller
{
    // Prefabs
    [SerializeField]
    Sprite[] walk_sprites;

    // Settings
    [SerializeField]
    float walk_fps;
    [SerializeField]
    float speed;

    // Plugins
    [SerializeField]
    GameObject body;

    // Components
    Rigidbody2D rigidbody_2d;
    SpriteRenderer body_renderer;

    // State
    float walk_frequency;
    float walk_timer;
    int walk_sprite_index;

    bool _walking;
    public bool walking => _walking;

    void Awake()
    {
        rigidbody_2d = GetComponent<Rigidbody2D>();
        body_renderer = body.GetComponent<SpriteRenderer>();

        walk_frequency = 1/walk_fps;
    }

    void FixedUpdate()
    {
        float max_frame_speed = speed * Time.fixedDeltaTime;
        float current_frame_speed = max_frame_speed * InputValue("Horizontal");
        float speed_ratio = Mathf.Abs(current_frame_speed / max_frame_speed);

        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 destination = position + Vector2.right * current_frame_speed;
        // MOST SCUFFED LINE OF CODE IVE EVER WRITTEN
        destination.x = Mathf.Clamp(destination.x, 0, 165);

        rigidbody_2d.MovePosition(destination);
        _walking = !Mathf.Approximately(current_frame_speed, 0);

        if(_walking)
        {
            float direction = Mathf.Sign(current_frame_speed);
            float rotation = direction > 0 ? 0 : 180;
            transform.rotation = Quaternion.AngleAxis(rotation, transform.up);

            walk_timer += Time.fixedDeltaTime * speed_ratio;

            // Animate the player's walk by indexing a sprite array
            if(walk_timer >= walk_frequency)
            {
                walk_sprite_index = (walk_sprite_index+1) % walk_sprites.Length;
                body_renderer.sprite = walk_sprites[walk_sprite_index];

                walk_timer = 0;
            }
        }
    }
}
