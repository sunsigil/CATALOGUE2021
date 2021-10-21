using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueMenu : MonoBehaviour
{
    [SerializeField]
    GameObject background_prefab;
    [SerializeField]
    GameObject display_prefab;

    Camera camera;

    GameObject menu;
    GameObject background_object;
    GameObject display_object;

    Vector2 screen_center;
    Vector3 anchor_point;

    float max_background_radius;
    float max_display_radius;
    float initial_background_radius;
    float initial_display_radius;

    float expansion_duration = 1;
    float expansion_timer;

    void SpawnMenu()
    {
        if(menu){Destroy(menu);}

        anchor_point = camera.ScreenToWorldPoint(screen_center);
        anchor_point.z = 0;

        menu = new GameObject("Menu");
        menu.transform.position = anchor_point;

        background_object = Instantiate(background_prefab, menu.transform);
        display_object = Instantiate(display_prefab, menu.transform);

        initial_background_radius = background_object.transform.localScale.x / 2;
        initial_display_radius = background_object.transform.localScale.x / 2;

        expansion_timer = 0;
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
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(!menu)
            {
                SpawnMenu();
            }
            else if(expansion_timer >= expansion_duration)
            {
                Destroy(menu);
            }
        }
    }

    void FixedUpdate()
    {
        if(menu && expansion_timer <= expansion_duration)
        {
            float progress = expansion_timer / expansion_duration;
            progress = Mathf.Clamp(progress, 0, 1);

            float background_radius = Mathf.Lerp(initial_background_radius, max_background_radius, progress);
            float display_radius = Mathf.Lerp(initial_display_radius, max_display_radius, progress);

            background_object.transform.localScale = new Vector3(background_radius * 2, background_radius * 2, 1);
            display_object.transform.localScale = new Vector3(display_radius * 2, display_radius * 2, 1);

            expansion_timer += Time.fixedDeltaTime;
        }
    }
}
