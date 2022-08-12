using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subcontroller for in-place
/// bullet spiral attack
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Swirl : CombatMode
{
    // Prefabs
    [SerializeField]
    GameObject bullet_prefab;

    // Settings
    [SerializeField]
    int base_count;
    [SerializeField]
    int boost_count;
    [SerializeField]
    float rotation_time;
    
    // State
    int count;
    Timeline timeline;
    float omega;
    float theta;
    float spawn_arc;
    float arc_accum;

    public override void Entry(StateSignal signal)
    {
        if(signal == StateSignal.ENTER)
        {
            machine.Transition(Resolution);
        }
    }

    /// <summary>
    /// [MachineState] Spin and launch bullets
    /// </summary>
    /// <param name="signal"></param>
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
					bullet.transform.position = transform.position + transform.up * combatant.arena.scale;
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
}
