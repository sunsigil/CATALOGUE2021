using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combatant))]
[RequireComponent(typeof(Machine))]

public class Enemy : MonoBehaviour
{
    ProgressRing progress_ring;

    [SerializeField]
    Bullet bullet_prefab;
    [SerializeField]
    Material idle_material;
    [SerializeField]
    Material aggro_material;

    [SerializeField]
    SpriteRenderer deathblow_dot;

    [SerializeField]
    EnemyMotionMode motion_mode;
    [SerializeField]
    EnemyFireMode fire_mode;
    [SerializeField]
    float fire_cooldown;

    Machine machine;
    Combatant combatant;
    SpriteRenderer sprite_renderer;

    Shooter shooter;
    bool aggroed;

    Timeline timeline;

    Vector3 GetVelocity()
    {
        if(shooter != null)
        {
            switch(motion_mode)
            {
                case EnemyMotionMode.LINE:
                    Vector3 line = (shooter.transform.position - transform.position);
                    if(line.magnitude > combatant.arena.scale){ return line.normalized; }
                    else{ return Vector3.zero; }
                break;

                case EnemyMotionMode.CIRCLE:
                    if(transform.localPosition.magnitude < 3){ return transform.localPosition.normalized; }
                    else{ return Vector3.Cross(-transform.localPosition, Vector3.forward).normalized; }
                break;
            }
        }

        return Vector3.zero;
    }

    void Fire()
    {
        switch(fire_mode)
        {
            case EnemyFireMode.BEAM:
                Bullet bullet = combatant.arena.Add(bullet_prefab.gameObject, transform.localPosition + transform.forward).GetComponent<Bullet>();
                bullet.velocity = (shooter.transform.position - transform.position).normalized;
            break;

            case EnemyFireMode.RING:
                for(int i = 0; i < 8; i++)
                {
                    float t = Mathf.PI * 0.25f * i;
                    float r = 1.5f;

                    bullet = combatant.arena.Add(bullet_prefab.gameObject, transform.localPosition + NumTools.XY_Polar(t, r)).GetComponent<Bullet>();
                    bullet.velocity = NumTools.XY_Polar(t, r);
                }
            break;
        }
    }

    void DeathEffects()
    {
        ProgressRing death_ring = AssetTools.SpawnComponent(progress_ring);
        death_ring.Initialize(Color.black, 0.1f, combatant.arena.scale * 2.5f, 1);
        death_ring.transform.position = transform.position;

        AudioWizard._.PlayEffect("sentinel death");

        Destroy(gameObject);
    }

    public void Aggro(){ aggroed = true; }
    public bool IsAggroed(){ return aggroed; }

    void Idle(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                sprite_renderer.material = idle_material;
                combatant.ToggleInvincible(true);
            break;

            case StateSignal.TICK:
                if(aggroed)
                {
                    machine.Transition(Active);
                }
            break;

            case StateSignal.EXIT:
                sprite_renderer.material = aggro_material;
                combatant.ToggleInvincible(false);
            break;
        }
    }

    void Active(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.TICK:
                if(!aggroed || shooter == null)
                {
                    machine.Transition(Idle);
                }

                if(timeline == null || timeline.Evaluate())
                {
                    Fire();
                    timeline = new Timeline(fire_cooldown);
                }
                else
                {
                    timeline.Tick(Time.deltaTime);
                }
            break;

            case StateSignal.FIXED_TICK:
                transform.rotation = NumTools.XY_Quat(GetVelocity(), 90);
                combatant.Move(transform.position + GetVelocity() * Time.fixedDeltaTime);
            break;
        }
    }

    void Depleted(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                deathblow_dot.enabled = true;
                timeline = new Timeline(1);
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

        machine = GetComponent<Machine>();
        combatant = GetComponent<Combatant>();
        sprite_renderer = GetComponent<SpriteRenderer>();

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
