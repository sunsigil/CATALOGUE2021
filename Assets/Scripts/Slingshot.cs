using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Machine))]
[RequireComponent(typeof(Combatant))]

public class Slingshot : Subcontroller
{
	[Header("Slingshot")]

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
                timeline = new Timeline(1 - travel_time);
				start = transform.position;
                end = start;
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
            break;

            case StateSignal.TICK:
                timeline.Tick(Time.deltaTime);

                if(timeline.Evaluate())
                {
                    machine.Transition(Anticipation);
                }
            break;

            case StateSignal.FIXED_TICK:
                Vector3 velocity = (end - start) / travel_time;
                rigidbody.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
            break;

            case StateSignal.EXIT:
                combatant.RingStrike(strike_offset, strike_radius, new Attack(combatant, Vector3.zero, 1));
            break;
        }
    }

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        line_renderer = GetComponent<LineRenderer>();
        machine = GetComponent<Machine>();
        combatant = GetComponent<Combatant>();

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
