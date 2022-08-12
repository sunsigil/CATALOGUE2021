using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subcontroller for
/// semi-auto shooting and
/// WASD movement.
/// The default player combat mode.
/// </summary>
public class Cannon : CombatMode
{
    // Settings
	[SerializeField]
    GameObject bullet_prefab;
	[SerializeField]
    float speed;

    // State
    Vector3 mouse_ray;
    Vector3 input_ray;

    Vector3 x;
    Vector3 v;

    bool swivelling;

	void UpdateMouseRay()
    {
        Vector2 mouse_velocity = new Vector2(InputValue("Mouse X", true), InputValue("Mouse Y", true));
        if(mouse_velocity.magnitude <= 0.1f){ return; }

        Vector3 reference = transform.position;

        Vector3 mouse_point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_point.z = reference.z;

        mouse_ray = mouse_point - reference;
        swivelling = mouse_ray.magnitude > 0.1f;
    }

    void UpdateInputRay()
    {
        Vector3 h_axis = Vector3.right;
        Vector3 v_axis = Vector3.up;
        Vector3 h_input = h_axis * InputValue("Horizontal", true);
        Vector3 v_input = v_axis * InputValue("Vertical", true);

        input_ray = h_input + v_input;
    }

	public override void Entry(StateSignal signal)
	{
		if(signal == StateSignal.ENTER)
		{ machine.Transition(Anticipation); }
	}

    /// <summary>
    /// [MachineState] Move and fire bullets
    /// </summary>
    /// <param name="signal"></param>
	public void Anticipation(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.TICK:
				UpdateMouseRay();
				UpdateInputRay();

                if (swivelling)
                { transform.rotation = NumTools.XY_Quat(mouse_ray.normalized, -90); }
                v = input_ray * speed;
                   
                if (Pressed(InputCode.ACTION) && cooldown_timeline.Evaluate())
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
