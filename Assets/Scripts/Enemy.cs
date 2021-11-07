using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum EnemyState
    {
        IDLE,
        AIM,
        CHARGE,
        TRAVEL
    }
    EnemyState state;

    [SerializeField]
    GameObject death_ring_prefab;

    [SerializeField]
    int max_lives;

    Rigidbody2D rigidbody;
    LineRenderer line_renderer;

    Vector3 initial_scale;
    float initial_line_width;

    int lives;

    Shooter shooter;
    Vector3 destination;
    Vector3 path;

    float aim_duration = 1;
    float aim_timer;
    float aim_progress => (aim_timer > 0) ? (aim_timer / aim_duration) : 0;

    float charge_duration = 0.5f;
    float charge_timer;
    float charge_progress => (charge_timer > 0) ? (charge_timer / charge_duration) : 0;

    float travel_duration = 0.25f;
    float travel_timer;
    float travel_progress => (travel_timer > 0) ? (travel_timer / travel_duration) : 0;

    public float speed => (state == EnemyState.TRAVEL) ? (path.magnitude / travel_duration) : 0;

    void ShowAimers()
    {
        line_renderer.SetPosition(0, transform.position);
        line_renderer.SetPosition(1, Vector3.Lerp(transform.position, destination, charge_progress));

        float line_width = initial_line_width * transform.localScale.x;
        line_renderer.SetWidth(line_width, line_width);

        line_renderer.enabled = true;
    }

    void HideAimers()
    {
        line_renderer.enabled = false;
    }

    void FaceTarget()
    {
        transform.rotation = CowTools.Vec2Rot(path.normalized, -90);
    }

    public void RecordSpawnValues()
    {
        initial_scale = transform.localScale;
        initial_line_width = line_renderer.widthCurve[0].value * initial_scale.x;
    }

    public bool IsAggroed(){return state != EnemyState.IDLE;}

    public void Aggro()
    {
        state = EnemyState.AIM;
        aim_timer = 0;
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        line_renderer = GetComponent<LineRenderer>();

        shooter = FindObjectOfType<Shooter>();
    }

    void FixedUpdate()
    {
        if(!shooter){return;}

        print(state);

        if(state == EnemyState.AIM)
        {
            HideAimers();

            destination = shooter.transform.position;
            path = destination - transform.position;

            FaceTarget();

            aim_timer += Time.fixedDeltaTime;
            if(aim_progress >= 1)
            {
                state = EnemyState.CHARGE;
                charge_timer = 0;
            }
        }
        else if(state == EnemyState.CHARGE)
        {
            ShowAimers();

            // "Flash" scaling function:
            // Form of f(x) = Ax * sin(Bx - C) + D
            // Domain  and range ~= [0, 1], [0, 0.25]
            float A = 0.14f;
            float B = -17.6f;
            float C = -0.2f;
            float D = 0.12f;
            float scale = A * charge_progress * Mathf.Sin(B * charge_progress - C) + D;
            transform.localScale = initial_scale + new Vector3(scale, scale, 0);

            charge_timer += Time.fixedDeltaTime;
            if(charge_progress >= 1)
            {
                state = EnemyState.TRAVEL;
                travel_timer = 0;
            }
        }
        else if(state == EnemyState.TRAVEL)
        {
            HideAimers();

            Vector3 offset = path.normalized * speed * Time.fixedDeltaTime;
            rigidbody.MovePosition(transform.position + offset);

            travel_timer += Time.fixedDeltaTime;
            if(travel_progress >= 1)
            {
                state = EnemyState.IDLE;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform == shooter.transform)
        {
            if(speed >= shooter.speed)
            {
                print("Hit!");

                shooter.ProcessHit();
            }
            else
            {
                print("Hurt...");

                GameObject death_ring_object = Instantiate(death_ring_prefab);
                death_ring_object.transform.localScale = transform.localScale * 10;
                death_ring_object.transform.position = transform.position;

                Destroy(gameObject);
            }
        }
    }
}
