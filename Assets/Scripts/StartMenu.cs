using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartMenu : Controller
{
    [SerializeField]
    GameObject background_object;
    [SerializeField]
    GameObject lock_object;
    [SerializeField]
    GameObject key_start;
    [SerializeField]
    GameObject key_end;

    Camera camera;

    Vector2 screen_center;
    Vector3 anchor_point;

    float initial_radius;
    float max_radius;

    LineRenderer shaft_renderer;
    float initial_shaft_width;

    float charge_duration = 1f;
    float charge_timer;
    float charge_progress => charge_timer / charge_duration;

    float slide_duration = 1f;
    float slide_timer;
    float slide_progress => slide_timer / slide_duration;

    float collapse_duration = 0.75f;
    float collapse_timer;
    float collapse_progress => collapse_timer / collapse_duration;

    void Awake()
    {
        camera = Camera.main;

        float w = Screen.width;
        float h = Screen.height;

        Vector3 screen_corner = new Vector3(w, h, 1);
        screen_center = new Vector3(w/2, h/2, 1);

        Vector3 corner_point = camera.ScreenToWorldPoint(screen_corner);
        corner_point.z = 0;
        anchor_point = camera.ScreenToWorldPoint(screen_center);
        anchor_point.z = 0;
        float screen_span = (corner_point - anchor_point).magnitude;

        initial_radius = background_object.transform.localScale.x / 2;
        max_radius = screen_span * 1.15f;

        transform.position = anchor_point;
        transform.localScale = new Vector3(max_radius * 2, max_radius * 2, 1);

        shaft_renderer = key_start.GetComponent<LineRenderer>();
        initial_shaft_width = shaft_renderer.widthCurve[0].value;
    }

    void FixedUpdate()
    {
        float shaft_width = initial_shaft_width * (transform.localScale.x / 8);
        shaft_renderer.SetWidth(shaft_width, shaft_width);

        if(charge_progress < 1)
        {
            if(Pressed(InputCode.ACTION) || Held(InputCode.ACTION))
            {
                charge_timer += Time.fixedDeltaTime;
                charge_timer = Mathf.Clamp(charge_timer, 0, charge_duration);
            }
            else
            {
                charge_timer -= Time.fixedDeltaTime;
                charge_timer = Mathf.Clamp(charge_timer, 0, charge_duration);
            }

            // speed ramp function in form:
            //  f(x) = (e^(kx) - 1) / (e^k - 1)
            float k = -3f;
            float progress = (Mathf.Exp(k * charge_progress) - 1) / (Mathf.Exp(k) - 1);

            Vector3 destination = Vector3.Lerp(key_start.transform.position, key_end.transform.position, progress);

            shaft_renderer.SetPosition(0, key_start.transform.position);
            shaft_renderer.SetPosition(1, destination);
        }
        else if(slide_progress < 1)
        {
            Vector3 current = Vector3.Lerp(key_start.transform.position, key_end.transform.position, slide_progress);

            key_start.transform.position = current;
            shaft_renderer.SetPosition(0, current);

            slide_timer += Time.fixedDeltaTime;
        }
        else if(collapse_progress < 1)
        {
            float radius_t = 1 - Mathf.Pow(collapse_progress, 5);
            float radius = Mathf.Lerp(initial_radius, max_radius, radius_t);

            transform.localScale = new Vector3(radius * 2, radius * 2, 1);

            collapse_timer += Time.fixedDeltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
