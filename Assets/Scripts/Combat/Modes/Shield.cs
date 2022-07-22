using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : CombatMode
{
	[SerializeField]
	ProgressRing shield_ring;

	[SerializeField]
	int base_durability;
	[SerializeField]
	int boost_durability;
	[SerializeField]
	float max_shield_radius;
	[SerializeField]
	float min_shield_radius;
	[SerializeField]
	float shield_time;

	Combatant shield_combatant;
	Collider2D shield_collider;

	int durability;
    Timeline timeline;
	float shield_radius;

    Vector3 MouseDirection()
    {
        Vector3 mouse_point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_point.z = transform.position.z;

        Vector3 mouse_line = mouse_point - transform.position;
        return mouse_line.normalized;
    }

	void DamageShield()
	{
		if(machine.InState(Anticipation))
		{
			timeline.Tick(shield_time / durability);

			if(timeline.Evaluate())
			{
				machine.Transition(Resolution);
			}
		}
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
				timeline = new Timeline(shield_time);
				shield_ring.gameObject.SetActive(true);
				combatant.ToggleInvincible(true);
            break;

            case StateSignal.TICK:
				if(Pressed(InputCode.CONFIRM) || Held(InputCode.CONFIRM))
				{
					shield_radius = Mathf.Lerp(max_shield_radius, min_shield_radius, timeline.progress);
					shield_ring.Lock(Color.red, 0.15f, shield_radius);

					timeline.Tick(Time.deltaTime);
				}
				if(timeline.Evaluate() || Released(InputCode.CONFIRM))
				{
					machine.Transition(Resolution);
				}
            break;

			case StateSignal.EXIT:
				combatant.ToggleInvincible(false);
			break;
        }
    }

    public void Resolution(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
				timeline = new Timeline(0.5f);
            break;

            case StateSignal.TICK:
				shield_radius = Mathf.Lerp(min_shield_radius, 0, timeline.progress);
				shield_ring.Lock(Color.red, 0.05f, shield_radius);

				timeline.Tick(Time.deltaTime);

                if(timeline.Evaluate())
				{
					machine.Transition(default_state);
				}
            break;

            case StateSignal.EXIT:
                shield_ring.gameObject.SetActive(false);

				cooldown_timeline = new Timeline(cooldown);
            break;
        }
    }

    protected override void Awake()
    {
		base.Awake();

		shield_combatant = shield_ring.GetComponent<Combatant>();
		shield_collider = shield_ring.GetComponent<Collider2D>();

		durability = powered ? boost_durability : base_durability;
    }

	void Start()
	{
		shield_combatant.on_hurt.AddListener(DamageShield);
		shield_ring.gameObject.SetActive(false);
	}
}
