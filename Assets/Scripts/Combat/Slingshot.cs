using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]

public class Slingshot : CombatMode
{
	ProgressRing progress_ring;

    [SerializeField]
    float travel_time;
    [SerializeField]
    float strike_shift;
	Vector3 strike_offset => transform.up * strike_shift;
    [SerializeField]
    float strike_radius;

    Rigidbody2D rigidbody;
    LineRenderer line_renderer;

    Timeline timeline;
    Vector3 start;
    Vector3 end;
	ProgressRing damage_ring;

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

				if(Pressed(InputCode.ACTION) || Held(InputCode.ACTION))
				{
					Vector3 front_ray = RayToWall(MouseDirection());
					Vector3 back_ray = RayToWall(-MouseDirection());
					start = transform.position + back_ray;
					end = transform.position + front_ray;
				}
				else if(Released(InputCode.ACTION))
				{
					machine.Transition(Resolution);
				}
            break;
        }
    }

    public void Resolution(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(travel_time);

				damage_ring = AssetTools.SpawnComponent(progress_ring);
				damage_ring.transform.SetParent(transform);
				damage_ring.Lock(Color.red, 0.1f, strike_radius);
            break;

            case StateSignal.FIXED_TICK:

				timeline.Tick(Time.fixedDeltaTime);

                Vector3 velocity = (end - start) / travel_time;
                rigidbody.MovePosition(transform.position + velocity * Time.fixedDeltaTime);

				if(timeline.Evaluate())
				{
					machine.Transition(Anticipation);
				}

				combatant.RingStrike(strike_offset, strike_radius, new Attack(combatant, Vector3.zero, 1));
				damage_ring.transform.localPosition = strike_offset;
            break;

            case StateSignal.EXIT:
				Destroy(damage_ring.gameObject);
            break;
        }
    }

    void Awake()
    {
		progress_ring = Resources.Load<ProgressRing>("Progress Ring");

        rigidbody = GetComponent<Rigidbody2D>();
        line_renderer = GetComponent<LineRenderer>();
        machine = GetComponent<Machine>();
        combatant = GetComponent<Combatant>();

        float line_width = line_renderer.widthCurve[0].value * transform.lossyScale.x;
        line_renderer.SetWidth(line_width, line_width);
    }

    void Update()
    {
        line_renderer.SetPosition(0, start);
        line_renderer.SetPosition(1, end);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + strike_offset, strike_radius);
    }
}
