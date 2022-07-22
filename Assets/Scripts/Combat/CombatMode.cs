using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combatant))]
[RequireComponent(typeof(Machine))]

public abstract class CombatMode : Subcontroller
{
	[SerializeField]
	protected Rune _skill_rune;
	public Rune skill_rune => _skill_rune;
	[SerializeField]
	protected Rune _power_rune;
	public Rune power_rune => _power_rune;

	[SerializeField]
	protected float cooldown;

	[SerializeField]
	protected bool lethal;

	protected Logger logger;

	protected Combatant combatant;
	protected Machine machine;
	protected Machine.MachineState default_state;
	protected Timeline cooldown_timeline;

	public bool unlocked => _skill_rune == null || logger.GetRune(_skill_rune.flag);
	public bool powered => _power_rune != null && logger.GetRune(_power_rune.flag);
	public bool ready => cooldown_timeline.Evaluate();
	public float readiness => cooldown_timeline.progress;

	public void BindDefault(CombatMode mode){ default_state = mode.Entry; }
	public abstract void Entry(StateSignal signal);

	protected virtual void Awake()
	{
		combatant = GetComponent<Combatant>();
		machine = GetComponent<Machine>();

		logger = FindObjectOfType<Logger>();
		cooldown_timeline = new Timeline(cooldown);
	}

	protected virtual void Update()
	{
		cooldown_timeline.Tick(Time.deltaTime);
	}
}
