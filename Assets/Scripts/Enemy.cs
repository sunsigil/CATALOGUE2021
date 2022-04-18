using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Machine))]

public class Enemy : MonoBehaviour
{
    [SerializeField]
    GameObject death_ring_prefab;

    [SerializeField]
    int max_lives;
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

    Shooter shooter;
    Vector3 start;
    Vector3 end;
    bool aggroed;
    int lives;

    float line_width;

    public void DeathEffects()
    {
        GameObject death_ring_object = Instantiate(death_ring_prefab);
        death_ring_object.transform.localScale = transform.lossyScale * 10;
        death_ring_object.transform.position = transform.position;

        Destroy(gameObject);
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

    public void Aggro(){ aggroed = true; }
    public bool IsAggroed(){ return aggroed; }

    void Idle(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                start = transform.position;
                end = start;
            break;

            case StateSignal.TICK:
                if(aggroed)
                {
                    machine.Transition(Aim);
                    aggroed = false;
                }
            break;
        }
    }

    void Aim(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(0.5f);

                shooter = FindObjectOfType<Shooter>();
                if(shooter == null){ machine.Transition(Idle); }

                start = transform.position;
                end = shooter.transform.position;
                Vector3 path = end - start;

                transform.rotation = NumTools.XY_Rot(path.normalized, -90);
            break;

            case StateSignal.TICK:
                timeline.Tick(Time.deltaTime);

                if(timeline.Evaluate())
                {
                    machine.Transition(Charge);
                }
            break;
        }
    }

    void Charge(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(1 - travel_time);
            break;

            case StateSignal.TICK:
                timeline.Tick(Time.deltaTime);

                float scale = NumTools.Flash(timeline.progress, 0.14f, -17.6f, -0.2f, 0.12f);
                transform.localScale = NumTools.XY_Scale(scale);

                if(timeline.Evaluate())
                {
                    machine.Transition(Travel);
                }
            break;

            case StateSignal.EXIT:
                transform.localScale = Vector3.one;
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

            case StateSignal.FIXED_TICK:
                timeline.Tick(Time.fixedDeltaTime);

                Vector3 offset = (end-start) * Time.fixedDeltaTime;
                rigidbody.MovePosition(transform.position + offset);
                RingStrike();

                if(timeline.Evaluate())
                {
                    machine.Transition(Idle);
                }
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

        combatant.on_die.AddListener(DeathEffects);

        machine.Transition(Idle);

        float line_width = line_renderer.widthCurve[0].value * transform.localScale.x;
    }

    void Update()
    {
        float scale = transform.localScale.x;
        line_renderer.SetWidth(line_width*scale, line_width*scale);

        line_renderer.SetPosition(0, start);
        line_renderer.SetPosition(1, start+end);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + strike_offset, strike_radius);
    }
}
