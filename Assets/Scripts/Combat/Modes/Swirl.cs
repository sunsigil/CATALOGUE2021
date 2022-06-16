using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Swirl : CombatMode
{
    [SerializeField]
    GameObject bullet_prefab;

    [SerializeField]
    int base_count;
    [SerializeField]
    int boost_count;
    [SerializeField]
    float rotation_time;

    int count;
    Timeline timeline;
    float omega;
    float theta;
    float spawn_arc;
    float arc_accum;

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

    public override void Entry(StateSignal signal)
    {
        if(signal == StateSignal.ENTER)
        {
            machine.Transition(Resolution);
        }
    }

    public void Resolution(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(rotation_time);
                combatant.ToggleInvincible(true);

                omega = 2 * Mathf.PI / rotation_time;
                theta = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                spawn_arc = 2 * Mathf.PI / count;
                arc_accum = 0;
            break;

            case StateSignal.FIXED_TICK:
                transform.rotation = NumTools.XY_Quat(theta);
                theta += omega * Time.fixedDeltaTime;

                if(arc_accum == 0 || arc_accum >= spawn_arc)
                {
                    Bullet bullet = Instantiate(bullet_prefab, transform.parent).GetComponent<Bullet>();
					bullet.transform.position = transform.position + transform.up * combatant.arena_scale;
					bullet.velocity = transform.up;
					bullet.lethal = lethal;

                    arc_accum = 0;
                }
                arc_accum += omega * Time.deltaTime;

                if(timeline.Evaluate()){ machine.Transition(default_state); }
                timeline.Tick(Time.fixedDeltaTime);
            break;

            case StateSignal.EXIT:
                combatant.ToggleInvincible(false);
                cooldown_timeline = new Timeline(cooldown);
            break;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        count = powered ? boost_count : base_count;
    }

    void Update()
    {
        cooldown_timeline.Tick(Time.deltaTime);
    }
}
