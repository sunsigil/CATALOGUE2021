using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Controller
{
    enum ShooterState
    {
        AIM,
        CHARGE,
        TRAVEL
    }
    ShooterState state;

    [SerializeField]
    GameObject death_ring_prefab;

    [SerializeField]
    Transform tip;

    [SerializeField]
    GameObject marker;
    [SerializeField]
    GameObject[] life_orbs;

    Rigidbody2D rigidbody;
    LineRenderer line_renderer;

    float initial_line_width;

    Vector3 arena_center;
    float arena_radius;
    float max_travel;

    int lives = 3;

    float charge_duration = 1f;
    float charge_timer;
    float charge_progress => charge_timer / charge_duration;

    float travel_duration = 0.25f;
    float travel_timer;
    float travel_progress => travel_timer / travel_duration;

    Vector3 mouse_direction;
    Vector3 destination;
    Vector3 path;

    public float speed => (state == ShooterState.TRAVEL) ? (path.magnitude / travel_duration) : 0;

    void TurnToMouse()
    {
        Vector3 mouse_point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_point.z = transform.position.z;
        Vector3 mouse_line = mouse_point - transform.position;
        mouse_direction = mouse_line.normalized;
        transform.rotation = CowTools.Vec2Rot(mouse_direction, -90);
    }

    void TurnToPath()
    {
        transform.rotation = CowTools.Vec2Rot(path.normalized, -90);
    }

    void ShowAimers()
    {
        marker.transform.position = destination;
        line_renderer.SetPosition(0, tip.position);
        line_renderer.SetPosition(1, destination);

        float line_width = initial_line_width * transform.localScale.x;
        line_renderer.SetWidth(line_width, line_width);

        marker.SetActive(true);
        line_renderer.enabled = true;
    }

    void HideAimers()
    {
        marker.SetActive(false);
        line_renderer.enabled = false;
    }

    public void SetLimits(Vector3 arena_center, float arena_radius)
    {
        this.arena_center = arena_center;
        this.arena_radius = arena_radius;

        max_travel = arena_radius / 1.5f;
    }

    public void ProcessHit()
    {
        Destroy(life_orbs[lives-1]);

        lives--;

        if(lives <= 0)
        {
            GameObject death_ring_object = Instantiate(death_ring_prefab);
            death_ring_object.transform.localScale = transform.localScale * 10;
            death_ring_object.transform.position = transform.position;

            Destroy(gameObject);
        }
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        line_renderer = GetComponent<LineRenderer>();

        initial_line_width = line_renderer.widthCurve[0].value;
    }

    void Update()
    {
        if(state == ShooterState.AIM)
        {
            if(Pressed(InputCode.ACTION) || Held(InputCode.ACTION))
            {
                state = ShooterState.CHARGE;
                charge_timer = 0;
            }
        }
        else if(state == ShooterState.CHARGE)
        {
            if(Released(InputCode.ACTION))
            {
                state = ShooterState.TRAVEL;
                travel_timer = 0;
            }
        }
    }

    void FixedUpdate()
    {
        if(state == ShooterState.AIM)
        {
            TurnToMouse();
            HideAimers();
        }
        else if(state == ShooterState.CHARGE)
        {
            charge_timer += Time.fixedDeltaTime;
            charge_timer = Mathf.Clamp(charge_timer, 0, charge_duration);

            TurnToMouse();

            // speed ramp function in form:
            //  f(x) = (e^(kx) - 1) / (e^k - 1)
            float k = 2.4f;
            float progress = (Mathf.Exp(k * charge_progress) - 1) / (Mathf.Exp(k) - 1);

            Vector3 furthest_offset = mouse_direction * max_travel;
            Vector3 offset = Vector3.Lerp(Vector3.zero, furthest_offset, progress);

            int mask = ~(1 << 3);
            RaycastHit2D hit = Physics2D.Raycast(tip.position, mouse_direction, offset.magnitude, mask);
            if(hit.transform != null)
            {
                Vector3 hit_point = new Vector3(hit.point.x, hit.point.y, transform.position.z);
                Vector3 hit_line = hit_point - tip.position;

                offset = Vector3.ClampMagnitude(offset, hit_line.magnitude);
            }

            destination = tip.position + offset;
            path = destination - transform.position;

            ShowAimers();
        }
        else if(state == ShooterState.TRAVEL)
        {
            TurnToPath();

            Vector3 offset = path.normalized * speed * Time.fixedDeltaTime;
            rigidbody.MovePosition(transform.position + offset);

            travel_timer += Time.fixedDeltaTime;

            ShowAimers();

            if(travel_progress >= 1)
            {
                state = ShooterState.AIM;
            }
        }
    }
}
