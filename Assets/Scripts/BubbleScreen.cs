using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScreen : MonoBehaviour
{
	[SerializeField]
	float expand_time;
	[SerializeField]
	float collapse_time;

    Camera camera;

    Vector2 screen_center;
    Vector3 world_anchor;

    float start_radius;
    float end_radius;

    Machine machine;
	Machine.MachineState attach_point;
    Timeline timeline;

    public void Expanding(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(expand_time);
            break;

            case StateSignal.TICK:
				timeline.Tick(Time.fixedDeltaTime);

				float radius = Mathf.Lerp(start_radius, end_radius, NumTools.Powstep(timeline.progress, 5));
                transform.localScale = NumTools.XY_Scale(radius * 2);

                if(timeline.Evaluate())
                {
                    machine.Transition(attach_point);
                }
            break;
        }
    }

    public void Collapsing(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(collapse_time);
            break;

            case StateSignal.TICK:
                timeline.Tick(Time.fixedDeltaTime);

                float radius = Mathf.Lerp(end_radius, start_radius, NumTools.Powstep(timeline.progress, 5));
                transform.localScale = NumTools.XY_Scale(radius * 2);

                if(timeline.Evaluate())
                {
                    machine.Transition(null);
                }
            break;

            case StateSignal.EXIT:
                Destroy(gameObject);
            break;
        }
    }

	public void Attach(Machine.MachineState attach_point)
	{
		if(machine == null)
		{
			this.attach_point = attach_point;
			machine = new Machine(Expanding);
		}
	}

	public void Chain(Machine.MachineState link_point)
	{
		if(machine != null)
		{
			machine.Transition(link_point);
		}
	}

	public void Detach()
	{
		if(machine != null)
		{
			machine.Transition(Collapsing);
		}
	}

    void Awake()
    {
        camera = Camera.main;

        Vector3 screen_corner = new Vector3(Screen.width, Screen.height, 1);
        screen_center = new Vector3(Screen.width/2, Screen.height/2, 1);
        Vector3 corner_anchor = camera.ScreenToWorldPoint(screen_corner);
        corner_anchor.z = 0;

        world_anchor = camera.ScreenToWorldPoint(screen_center);
        world_anchor.z = 0;

        float screen_span = (corner_anchor - world_anchor).magnitude;
        start_radius = transform.localScale.x / 2;
        end_radius = screen_span * 1.15f;

        transform.position = world_anchor;
        transform.localScale = new Vector3(start_radius * 2, start_radius * 2, 1);
    }

	void Start()
	{
		if(attach_point == null)
		{
			gameObject.SetActive(false);
		}
		else
		{
			machine = new Machine(Expanding);
		}
	}

    void FixedUpdate()
    {
        if(machine != null){ machine.Tick(); }
    }
}
