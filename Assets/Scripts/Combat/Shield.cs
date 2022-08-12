using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subcontroller for destructible
/// and decaying shield bubble
/// </summary>
public class Shield : CombatMode
{
	// Settings
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

	// Plugins
	[SerializeField]
	ProgressRing shield_ring;

	// State
	Combatant shield_combatant;

	int durability;
    Timeline timeline;
	float shield_radius;

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

	/// <summary>
	/// [MachineState] Sustain shield
	/// </summary>
	/// <param name="signal"></param>
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

		durability = powered ? boost_durability : base_durability;
    }

	void Start()
	{
		shield_combatant.on_hurt.AddListener(DamageShield);
		shield_ring.gameObject.SetActive(false);
	}
}
