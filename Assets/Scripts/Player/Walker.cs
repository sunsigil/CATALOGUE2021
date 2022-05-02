using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Subcontroller
{
    [SerializeField]
    GameObject hat;
    [SerializeField]
    GameObject head;
    [SerializeField]
    GameObject body;
    [SerializeField]
    Sprite[] walk_sprites;

    [SerializeField]
    float walk_fps;

    [SerializeField]
    float speed;

    Rigidbody2D rigidbody_2d;
    SpriteRenderer body_renderer;

    Vector3 hat_origin;
    Vector3 head_origin;
    Vector3 body_origin;

    float walk_frequency;
    float walk_timer;
    int walk_sprite_index;

    bool _walking;
    public bool walking => _walking;

    void Awake()
    {
        rigidbody_2d = GetComponent<Rigidbody2D>();
        body_renderer = body.GetComponent<SpriteRenderer>();

        hat_origin = hat.transform.localPosition;
        head_origin = head.transform.localPosition;
        body_origin = body.transform.localPosition;

        walk_frequency = 1/walk_fps;
    }

    void FixedUpdate()
    {
        float max_frame_speed = speed * Time.fixedDeltaTime;
        float current_frame_speed = max_frame_speed * InputValue("Horizontal");
        float speed_ratio = Mathf.Abs(current_frame_speed / max_frame_speed);

        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 destination = position + Vector2.right * current_frame_speed;

        rigidbody_2d.MovePosition(destination);
        _walking = !Mathf.Approximately(current_frame_speed, 0);

        if(_walking)
        {
            float direction = Mathf.Sign(current_frame_speed);
            float rotation = direction > 0 ? 0 : 180;
            transform.rotation = Quaternion.AngleAxis(rotation, transform.up);

            walk_timer += Time.fixedDeltaTime * speed_ratio;

            if(walk_timer >= walk_frequency)
            {
                walk_sprite_index = (walk_sprite_index+1) % walk_sprites.Length;
                body_renderer.sprite = walk_sprites[walk_sprite_index];

                walk_timer = 0;
            }
        }
    }
}