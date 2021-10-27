using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueMenu : Controller
{
    [SerializeField]
    GameObject background_object;
    [SerializeField]
    GameObject display_object;
    [SerializeField]
    GameObject[] covers;

    Camera camera;

    Vector2 screen_center;
    Vector3 anchor_point;

    float max_background_radius;
    float max_display_radius;
    float initial_background_radius;
    float initial_display_radius;

    float expansion_duration = 1;
    float expansion_timer;
    bool expanding => expansion_timer > 0 && expansion_timer < expansion_duration;

    bool collapsing;
    float collapse_duration = 0.25f;
    float collapse_timer;

    public void Expose(int cover_index)
    {
        covers[cover_index].SetActive(false);
    }

    void Awake()
    {
        camera = Camera.main;

        float w = Screen.width;
        float h = Screen.height;

        Vector2 screen_corner = new Vector2(w, h);
        screen_center = screen_corner / 2;

        Vector3 corner_point = camera.ScreenToWorldPoint(screen_corner);
        corner_point.z = 0;
        anchor_point = camera.ScreenToWorldPoint(screen_center);
        anchor_point.z = 0;
        float screen_span = (corner_point - anchor_point).magnitude;

        max_background_radius = screen_span;
        max_display_radius = max_background_radius * 0.25f;

        transform.position = anchor_point;

        initial_background_radius = background_object.transform.localScale.x / 2;
        initial_display_radius = background_object.transform.localScale.x / 2;

        expansion_timer = 0;
    }

    void Update()
    {
        if(Pressed(InputCode.CANCEL) && !expanding)
        {
            collapsing = true;
        }
    }

    void FixedUpdate()
    {
        if(expansion_timer <= expansion_duration)
        {
            float progress = expansion_timer / expansion_duration;
            progress = Mathf.Clamp(progress, 0, 1);

            float background_radius = Mathf.Lerp(initial_background_radius, max_background_radius, progress);
            float display_radius = Mathf.Lerp(initial_display_radius, max_display_radius, progress);

            background_object.transform.localScale = new Vector3(background_radius * 2, background_radius * 2, 1);
            display_object.transform.localScale = new Vector3(display_radius * 2, display_radius * 2, 1);

            expansion_timer += Time.fixedDeltaTime;
        }
        else if(collapsing)
        {
            if(collapse_timer < collapse_duration)
            {
                float progress = collapse_timer / collapse_duration;
                progress = 1 - Mathf.Clamp(progress, 0, 1);

                float background_radius = Mathf.Lerp(initial_background_radius, max_background_radius, progress);
                float display_radius = Mathf.Lerp(initial_display_radius, max_display_radius, progress);

                background_object.transform.localScale = new Vector3(background_radius * 2, background_radius * 2, 1);
                display_object.transform.localScale = new Vector3(display_radius * 2, display_radius * 2, 1);

                collapse_timer += Time.fixedDeltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        display_object.transform.Rotate(-Vector3.forward * 11.25f * Time.fixedDeltaTime);
    }
}
