using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : CombatMode
{
	[SerializeField]
    GameObject bullet_prefab;
	[SerializeField]
    float speed;

    Vector3 mouse_ray; // ray from body to mouse
    Vector3 input_ray; // wasd ray
    Vector3 last_nzi; // last nonzero input

    Vector3 x; // position
    Vector3 v; // velocity
    Vector3 last_nzv; // last nonzero v

    bool swivelling; // is mouse in use
    bool last_swivelling;
    bool controlling; // are keys in use
    bool last_controlling;
    bool moving; // is velocity significant
    bool last_moving;

	void UpdateMouseRay()
    {
        Vector2 mouse_velocity = new Vector2(InputValue("Mouse X", true), InputValue("Mouse Y", true));
        if(mouse_velocity.magnitude <= 0.1f){ return; }

        Vector3 reference = transform.position;

        Vector3 mouse_point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_point.z = reference.z;

        mouse_ray = mouse_point - reference;

        last_swivelling = swivelling;
        swivelling = mouse_ray.magnitude > 0.1f;
    }

    void UpdateInputRay()
    {
        Vector3 h_axis = Vector3.right;
        Vector3 v_axis = Vector3.up;
        Vector3 h_input = h_axis * InputValue("Horizontal", true);
        Vector3 v_input = v_axis * InputValue("Vertical", true);

        if(controlling){ last_nzi = input_ray; }
        input_ray = h_input + v_input;

        last_controlling = controlling;
        controlling = input_ray.magnitude > 0.1f;
    }

    void UpdateRotation()
    {
        if(swivelling)
        { transform.rotation = NumTools.XY_Quat(mouse_ray.normalized, -90); }
    }

    void UpdateVelocity()
    {
        if(moving){ last_nzv = v; }
        v = input_ray * speed;

        last_moving = moving;
        moving = v.magnitude > 0.1f;
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
            case StateSignal.TICK:
				UpdateMouseRay();
				UpdateInputRay();

				UpdateRotation();
				UpdateVelocity();

				if(Pressed(InputCode.ACTION) && cooldown_timeline.Evaluate())
				{
					Bullet bullet = Instantiate(bullet_prefab, transform.parent).GetComponent<Bullet>();
					bullet.transform.position = transform.position + transform.up * combatant.arena.scale;
					bullet.velocity = transform.up * 5;
					bullet.lethal = lethal;

					cooldown_timeline = new Timeline(cooldown);
				}
            break;

			case StateSignal.FIXED_TICK:
				x = transform.position;
				x += v * Time.fixedDeltaTime;
				combatant.Move(x);
			break;
        }
    }
}
