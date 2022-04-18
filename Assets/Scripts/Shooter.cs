using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Machine))]

public class Shooter : Controller
{
    [SerializeField]
    GameObject death_ring_prefab;
    [SerializeField]
    GameObject[] life_orbs;

    [SerializeField]
    [Range(0.01f, 0.99f)]
    float travel_time;
    [SerializeField]
    Vector3 strike_offset;
    [SerializeField]
    float strike_radius;

    Rigidbody2D rigidbody;
    LineRenderer line_renderer;
    Machine machine;
    Combatant combatant;

    Timeline timeline;
    Vector3 start;
    Vector3 end;

    float line_width;

    void HitEffects()
    {
        int life_index = (int)(combatant.life * 3);
        Vector3 orb_position = life_orbs[life_index].transform.position;
        Destroy(life_orbs[life_index]);

        GameObject hit_ring_object = Instantiate(death_ring_prefab);
        hit_ring_object.transform.localScale = transform.lossyScale * 3;
        hit_ring_object.transform.position = orb_position;
    }

    void DeathEffects()
    {
        GameObject death_ring_object = Instantiate(death_ring_prefab);
        death_ring_object.transform.localScale = transform.lossyScale * 10;
        death_ring_object.transform.position = transform.position;

        Destroy(gameObject);
    }

    Vector3 MouseDirection()
    {
        Vector3 mouse_point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_point.z = transform.position.z;

        Vector3 mouse_line = mouse_point - transform.position;
        return mouse_line.normalized;
    }

    Vector3 RayToWall(Vector3 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, ~(1 << 3));

        if(hit.transform != null)
        {
            Vector3 hit_point = new Vector3(hit.point.x, hit.point.y, transform.position.z);
            return hit_point - transform.position;
        }

        return Vector3.zero;
    }

    void RingStrike()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position + strike_offset, strike_radius);

        foreach(Collider2D col in cols)
        {
            Combatant target = col.GetComponent<Combatant>();

            if(target != null)
            {
                target.EnqueueAttack(new Attack(combatant, 1));
            }
        }
    }

    void Aim(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                start = transform.position;
                end = start;

                timeline = new Timeline(1 - travel_time);
            break;

            case StateSignal.TICK:
                transform.rotation = NumTools.XY_Rot(MouseDirection(), -90);

                if(Pressed(InputCode.ACTION) || Held(InputCode.ACTION))
                {
                    timeline.Tick(Time.deltaTime);

                    float charge = NumTools.Hillstep(timeline.progress, 2.4f);
                    Vector3 ray = Vector3.ClampMagnitude(RayToWall(MouseDirection()), 5 * transform.lossyScale.x);
                    Vector3 path = Vector3.Lerp(Vector3.zero, ray, charge);

                    start = transform.position;
                    end = start + path;
                }
                else if(Released(InputCode.ACTION))
                {
                    machine.Transition(Travel);
                }
            break;
        }
    }

    void Travel(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(travel_time);
            break;

            case StateSignal.TICK:
                timeline.Tick(Time.deltaTime);

                if(timeline.Evaluate())
                {
                    machine.Transition(Aim);
                }
            break;

            case StateSignal.FIXED_TICK:
                Vector3 velocity = (end - start) / travel_time;
                rigidbody.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
            break;

            case StateSignal.EXIT:
                RingStrike();
            break;
        }
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        line_renderer = GetComponent<LineRenderer>();
        machine = GetComponent<Machine>();
        combatant = GetComponent<Combatant>();

        combatant.on_hit.AddListener(HitEffects);
        combatant.on_die.AddListener(DeathEffects);

        machine.Transition(Aim);

        float line_width = line_renderer.widthCurve[0].value * transform.lossyScale.x;
        line_renderer.SetWidth(line_width, line_width);
    }

    void Update()
    {
        line_renderer.SetPosition(0, transform.position);
        line_renderer.SetPosition(1, end);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + strike_offset, strike_radius);
    }
}
