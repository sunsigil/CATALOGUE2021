using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Controller
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
    GameObject start_menu_prefab;
    [SerializeField]
    GameObject catalogue_menu_prefab;
    [SerializeField]
    GameObject card_menu_prefab;

    [SerializeField]
    float walk_fps;

    [SerializeField]
    float speed;

    bool _walking;
    public bool walking => _walking;

    Camera main_camera;
    CameraFollow camera_follow;

    Rigidbody2D rigidbody_2d;
    SpriteRenderer body_renderer;
    Catalogue catalogue;
    Satchel satchel;

    Vector3 hat_origin;
    Vector3 head_origin;
    Vector3 body_origin;

    float walk_frequency;
    float walk_timer;
    int walk_sprite_index;

    [SerializeField]
    Dungeon dungeon;

    void Awake()
    {
        main_camera = Camera.main;
        camera_follow = FindObjectOfType<CameraFollow>();

        rigidbody_2d = GetComponent<Rigidbody2D>();
        body_renderer = body.GetComponent<SpriteRenderer>();
        catalogue = GetComponent<Catalogue>();

        hat_origin = hat.transform.localPosition;
        head_origin = head.transform.localPosition;
        body_origin = body.transform.localPosition;

        walk_frequency = 1/walk_fps;
    }

    void Start()
    {
        FindObjectOfType<CameraFollow>().Snap();
        Instantiate(start_menu_prefab);
    }

    void Update()
    {
        if(Pressed(InputCode.CONFIRM))
        {
            float max_grab_dist = 4;
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, max_grab_dist);

            Usable best_usable = null;
            float best_grab_dist = max_grab_dist;

            foreach (Collider2D col in cols)
            {
                Usable usable = col.transform.GetComponent<Usable>();

                if(usable != null)
                {
                    float grab_dist = Mathf.Abs(usable.transform.position.x - transform.position.x);

                    if(grab_dist < best_grab_dist)
                    {
                        best_usable = usable;
                        best_grab_dist = grab_dist;
                    }
                }
            }

            if(best_usable != null)
            {
                best_usable.RequestUse();
            }
        }
        else if(Pressed(InputCode.JOURNAL))
        {
            camera_follow.Snap();
            //Instantiate(catalogue_menu_prefab);
            catalogue.AddCard(CardFlag.STARVING_DOG);
            catalogue.AddCard(CardFlag.BREAKING_FORM);
            catalogue.AddCard(CardFlag.REBIRTH);
            catalogue.AddCard(CardFlag.CYCLOPS_MONK);
            Instantiate(card_menu_prefab).GetComponent<CardMenu>().dungeon = dungeon;
        }
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
