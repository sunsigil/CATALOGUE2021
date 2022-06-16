using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Combatant))]
[RequireComponent(typeof(Machine))]

public class Enemy : MonoBehaviour
{
    ProgressRing progress_ring;

    [SerializeField]
    Bullet bullet_prefab;

    [SerializeField]
    SpriteRenderer deathblow_dot;

    [SerializeField]
    EnemyMotionMode motion_mode;
    [SerializeField]
    EnemyFireMode fire_mode;
    [SerializeField]
    float fire_cooldown;

    [SerializeField]
    int max_lives;

    Rigidbody2D rigidbody;
    Machine machine;
    Combatant combatant;

    Shooter shooter;
    bool aggroed;

    Timeline timeline;

    Vector3 GetVelocity()
    {
        switch(motion_mode)
        {
            case EnemyMotionMode.LINE:
                return (shooter.transform.position - transform.position).normalized;
            break;

            case EnemyMotionMode.CIRCLE:
                if(transform.localPosition.magnitude < 3){ return transform.localPosition.normalized; }
                else{ return Vector3.Cross(-transform.localPosition, Vector3.forward).normalized; }
            break;

            default:
                return Vector3.zero;
            break;
        }
    }

    void Fire()
    {
        switch(fire_mode)
        {
            case EnemyFireMode.BEAM:
                Bullet bullet = Instantiate(bullet_prefab, transform.parent).GetComponent<Bullet>();
                bullet.transform.position = transform.position + transform.forward;
                bullet.velocity = (shooter.transform.position - transform.position).normalized;
            break;

            case EnemyFireMode.RING:
                for(int i = 0; i < 8; i++)
                {
                    float t = Mathf.PI * 0.25f * i;
                    float r = 1.5f * combatant.arena_scale;

                    bullet = Instantiate(bullet_prefab, transform.parent).GetComponent<Bullet>();
                    bullet.transform.position = transform.position + NumTools.XY_Polar(t, r);
                    bullet.velocity = NumTools.XY_Polar(t, r);
                }
            break;
        }
    }

    public void DeathEffects()
    {
        ProgressRing death_ring = AssetTools.SpawnComponent(progress_ring);
        death_ring.Initialize(Color.black, 0.1f, combatant.arena_scale * 2.5f, 1);
        death_ring.transform.position = transform.position;

        Destroy(gameObject);
    }

    public void Aggro(){ aggroed = true; }
    public bool IsAggroed(){ return aggroed; }

    void Idle(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.TICK:
                if(aggroed)
                {
                    machine.Transition(Active);
                }
            break;
        }
    }

    void Active(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.TICK:
                if(timeline == null || timeline.Evaluate())
                {
                    Fire();
                    timeline = new Timeline(fire_cooldown);
                }
                else
                {
                    timeline.Tick(Time.deltaTime);
                }

                if(!aggroed)
                {
                    machine.Transition(Idle);
                }
            break;

            case StateSignal.FIXED_TICK:
                transform.rotation = NumTools.XY_Quat(GetVelocity());
                rigidbody.MovePosition(transform.position + GetVelocity() * Time.fixedDeltaTime);
            break;
        }
    }

    void Depleted(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                deathblow_dot.enabled = true;
                timeline = new Timeline(3);
            break;

            case StateSignal.TICK:
                float alpha = NumTools.Throb(1-timeline.progress, 0.5f) + 0.5f;
                Color col = deathblow_dot.color;
                col.a = alpha;
                deathblow_dot.color = col;

                timeline.Tick(Time.deltaTime);

                if(timeline.Evaluate())
                {
                    combatant.Heal(1);
                    machine.Transition(Idle);
                }
            break;

            case StateSignal.EXIT:
                deathblow_dot.enabled = false;
            break;
        }
    }

    void Awake()
    {
        progress_ring = Resources.Load<ProgressRing>("Progress Ring");

        rigidbody = GetComponent<Rigidbody2D>();
        machine = GetComponent<Machine>();
        combatant = GetComponent<Combatant>();

        combatant.on_deplete.AddListener(delegate{ machine.Transition(Depleted); });
        combatant.on_die.AddListener(DeathEffects);

        shooter = FindObjectOfType<Shooter>();

        machine.Transition(Idle);
    }

    void Update()
    {
        if(shooter == null){Destroy(gameObject);}
    }
}
