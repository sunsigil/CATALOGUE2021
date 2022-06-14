using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Slingshot : CombatMode
{
    [SerializeField]
	LineRenderer attack_line;
	[SerializeField]
	ProgressRing attack_ring;

    [SerializeField]
    float travel_time;
    [SerializeField]
    float strike_radius;

    Rigidbody2D rigidbody;

    Timeline timeline;
    Vector3 start;
    Vector3 end;

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
            machine.Transition(Anticipation);
        }
    }

    public void Anticipation(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
				start = transform.position;
                end = start;
            break;

            case StateSignal.TICK:
                transform.rotation = NumTools.XY_Rot(MouseDirection(), -90);

				if(Pressed(InputCode.CONFIRM) || Held(InputCode.CONFIRM))
				{
					Vector3 front_ray = RayToWall(MouseDirection()) * 0.95f;
					Vector3 back_ray = RayToWall(-MouseDirection()) * 0.95f;
					start = combatant.arena_offset + back_ray;
					end = combatant.arena_offset + front_ray;

                    attack_line.gameObject.SetActive(true);
                    attack_line.SetPosition(0, transform.InverseTransformPoint(start));
                    attack_line.SetPosition(1, transform.InverseTransformPoint(end));
				}
				else if(Released(InputCode.CONFIRM))
				{
					machine.Transition(Resolution);
				}
            break;

			case StateSignal.EXIT:
				attack_ring.gameObject.SetActive(true);
			break;
        }
    }

    public void Resolution(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(travel_time);
                combatant.ToggleInvincible(true);
            break;

            case StateSignal.FIXED_TICK:
				timeline.Tick(Time.fixedDeltaTime);

                Vector3 velocity = (end - start) / travel_time;
                rigidbody.MovePosition(transform.position + velocity * Time.fixedDeltaTime);

				attack_ring.transform.position = combatant.local_offset;
                attack_line.SetPosition(0, transform.InverseTransformPoint(start));
                attack_line.SetPosition(1, transform.InverseTransformPoint(end));

                if(timeline.Evaluate())
				{
					machine.Transition(default_state);
				}
            break;

            case StateSignal.TICK:
                combatant.RingStrike(strike_radius, new Attack(combatant, Vector3.zero, 1, lethal));
            break;

            case StateSignal.EXIT:
                cooldown_timeline = new Timeline(cooldown);
                combatant.ToggleInvincible(false);
                attack_line.gameObject.SetActive(false);
    			attack_ring.gameObject.SetActive(false);
            break;
        }
    }

    protected override void Awake()
    {
		base.Awake();

        rigidbody = GetComponent<Rigidbody2D>();

        float line_width = attack_line.widthCurve[0].value * combatant.arena_scale;
        attack_line.SetWidth(line_width, line_width);
        attack_line.gameObject.SetActive(false);

		attack_ring.transform.localScale = NumTools.XY_Scale(strike_radius * 2);
        attack_ring.gameObject.SetActive(false);
    }

    void Update()
    {
        cooldown_timeline.Tick(Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        combatant = GetComponent<Combatant>();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(combatant.arena_offset, strike_radius * combatant.arena_scale);
    }
}
