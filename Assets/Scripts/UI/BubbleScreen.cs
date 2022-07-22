using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Machine))]

public class BubbleScreen : MonoBehaviour
{
	[SerializeField]
	float expand_time;
	[SerializeField]
	float collapse_time;

	Machine machine;

	Camera camera;
	CameraFollow camera_follow;

    Vector2 screen_center;
    Vector3 world_anchor;

    float start_radius;
    float end_radius;

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
				timeline.Tick(Time.deltaTime);

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
                timeline.Tick(Time.deltaTime);

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
		if(this.attach_point == null)
		{
			this.attach_point = attach_point;
		}
	}

	public void Chain(Machine.MachineState link_point)
	{
		machine.Transition(link_point);
	}

	public void Detach()
	{
		machine.Transition(Collapsing);
	}

    void Awake()
    {
		machine = GetComponent<Machine>();

		camera = Camera.main;
		camera_follow = camera.GetComponent<CameraFollow>();
		camera_follow.Snap();

        Vector3 view_corner = new Vector3(1, 1, camera.nearClipPlane);
        Vector3 view_center = new Vector3(0.5f, 0.5f, camera.nearClipPlane);

        Vector3 world_corner = camera.ViewportToWorldPoint(view_corner);
        world_corner.z = 0;
        Vector3 world_center = camera.ViewportToWorldPoint(view_center);
        world_center.z = 0;

        float screen_span = 2 * (world_corner - world_center).magnitude;
        start_radius = transform.localScale.x * 0.5f;
        end_radius = screen_span * 0.5f;

        transform.position = world_center;
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
			machine.Transition(Expanding);
		}
	}

	void OnDestroy()
	{
		camera_follow.Snap();
	}
}
