using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : CombatMode
{
	[SerializeField]
	SpriteRenderer pulse_wave;
	Material pulse_mat;

	[SerializeField]
	float base_arc;
	[SerializeField]
	float boost_arc;
	[SerializeField]
	float pulse_time;

	float arc;
    Timeline timeline;

    Vector3 MouseDirection()
    {
        Vector3 mouse_point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_point.z = transform.position.z;

        Vector3 mouse_line = mouse_point - transform.position;
        return mouse_line.normalized;
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
				timeline = new Timeline(pulse_time);

				pulse_wave.gameObject.SetActive(true);
				pulse_mat.SetFloat("_Progress", 0);

				combatant.ToggleInvincible(true);
            break;

            case StateSignal.TICK:
				pulse_mat.SetFloat("_Progress", timeline.progress);

				combatant.ArcStrike
				(
					pulse_wave.transform.localScale.x/2 * timeline.progress,
					1.57f, arc,
					new Attack(combatant, transform.up, 1, lethal)
				);

				timeline.Tick(Time.deltaTime);

                if(timeline.Evaluate())
				{
					machine.Transition(default_state);
				}
            break;

            case StateSignal.EXIT:
                combatant.ToggleInvincible(false);
				pulse_wave.gameObject.SetActive(false);

				cooldown_timeline = new Timeline(cooldown);
            break;
        }
    }

    protected override void Awake()
    {
		base.Awake();

		pulse_mat = pulse_wave.material;

		arc = powered ? boost_arc : base_arc;

		pulse_mat.SetFloat("_Thickness", 0.05f);
		pulse_mat.SetFloat("_Arc", arc);
		pulse_wave.gameObject.SetActive(false);
    }
}
