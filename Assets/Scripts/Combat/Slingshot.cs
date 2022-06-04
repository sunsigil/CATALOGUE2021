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
    float strike_shift;
	Vector3 strike_offset => transform.up * strike_shift;
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

				attack_line.gameObject.SetActive(false);
				attack_ring.gameObject.SetActive(false);
            break;

            case StateSignal.TICK:
                transform.rotation = NumTools.XY_Rot(MouseDirection(), -90);

				if(Pressed(InputCode.ACTION) || Held(InputCode.ACTION))
				{
					Vector3 front_ray = RayToWall(MouseDirection());
					Vector3 back_ray = RayToWall(-MouseDirection());
					start = transform.position + back_ray;
					end = transform.position + front_ray;

					attack_line.gameObject.SetActive(true);
					attack_line.SetPosition(0, start);
			        attack_line.SetPosition(1, end);
				}
				else if(Released(InputCode.ACTION))
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
            break;

            case StateSignal.FIXED_TICK:
				timeline.Tick(Time.fixedDeltaTime);

                Vector3 velocity = (end - start) / travel_time;
                rigidbody.MovePosition(transform.position + velocity * Time.fixedDeltaTime);

				if(timeline.Evaluate())
				{
					machine.Transition(Anticipation);
				}

				combatant.RingStrike(strike_offset, strike_radius, new Attack(combatant, Vector3.zero, 1, false));
				attack_ring.transform.localPosition = strike_offset;
            break;
        }
    }

	public override bool Jump(CombatMode mode)
	{
		if(machine.InState(Anticipation))
		{
			attack_line.gameObject.SetActive(false);
			attack_ring.gameObject.SetActive(false);

			machine.Transition(mode.Entry);
			return true;
		}

		return false;
	}

    protected override void Awake()
    {
		base.Awake();

        rigidbody = GetComponent<Rigidbody2D>();

        float line_width = attack_line.widthCurve[0].value * combatant.arena_scale;
        attack_line.SetWidth(line_width, line_width);

		attack_ring.transform.localScale = NumTools.XY_Scale(strike_radius * 2);
		attack_ring.transform.localPosition = strike_offset;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + strike_offset, strike_radius);
    }
}