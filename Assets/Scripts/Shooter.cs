using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour, IHittable
{
    [SerializeField]
    Transform tip;

    [SerializeField]
    GameObject marker;

    [SerializeField]
    int max_lives;

    [SerializeField]
    float charge_period;

    Rigidbody2D rigidbody;
    LineRenderer line_renderer;

    int lives;

    Vector3 arena_center;
    float arena_radius;

    float max_speed;
    float charge_timer;
    Vector3 direction;
    float speed;
    Vector3 destination;

    public void SetLimits(Vector3 arena_center, float arena_radius)
    {
        this.arena_center = arena_center;
        this.arena_radius = arena_radius;

        max_speed = arena_radius / 1.5f;
    }

    public void ProcessHit()
    {
        lives--;

        if(lives == 0){Destroy(gameObject);}
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        line_renderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 mouse_point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_point.z = transform.position.z;
        Vector3 mouse_line = mouse_point - transform.position;

        direction = mouse_line.normalized;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);

        line_renderer.SetPosition(0, tip.position);
        line_renderer.SetPosition(1, tip.position);
        marker.SetActive(false);

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space))
        {
            if(charge_timer < charge_period)
            {
                charge_timer += Time.deltaTime;
            }

            // speed ramp function f(x) = 10 ^ 3(x-1) tested in desmos
            speed = max_speed * Mathf.Pow(5, 3 * (charge_timer - 1));

            Vector3 offset = direction * speed;

            int mask = ~(1 << 3);
            RaycastHit2D hit = Physics2D.Raycast(tip.position, direction, offset.magnitude, mask);
            if(hit.transform != null)
            {
                Vector3 hit_point = new Vector3(hit.point.x, hit.point.y, transform.position.z);
                Vector3 hit_line = hit_point - tip.position;

                offset = Vector3.ClampMagnitude(offset, hit_line.magnitude);
            }

            destination = tip.position + offset;

            line_renderer.SetPosition(0, tip.position);
            line_renderer.SetPosition(1, Vector3.Lerp(tip.position, destination, charge_timer/charge_period));
            marker.SetActive(true);
            marker.transform.position = destination;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            rigidbody.MovePosition(destination);

            charge_timer = 0;
        }
    }
}
