using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [ABSTRACT]
/// Defines the interface and required state
/// for subcontrollers which define a set of combat
/// abilities usable by the player.
/// </summary>
[RequireComponent(typeof(Combatant))]
[RequireComponent(typeof(Machine))]
public abstract class CombatMode : Subcontroller
{
	// Settings
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

	// Outsiders
	protected Logger logger;

	// Components
	protected Combatant combatant;
	protected Machine machine;
	protected Machine.MachineState default_state;
	protected Timeline cooldown_timeline;

	// State
	public bool unlocked => _skill_rune == null || logger.GetRune(_skill_rune.flag);
	public bool powered => _power_rune != null && logger.GetRune(_power_rune.flag);
	public bool ready => cooldown_timeline.Evaluate();
	public float readiness => cooldown_timeline.progress;

	/// <summary>
	/// Determine which combat mode this mode
	/// should yield to upon exit
	/// </summary>
	/// <param name="mode"></param>
	public void BindDefault(CombatMode mode)
	{ default_state = mode.Entry; }
	/// <summary>
	/// [MachineState] Entry point for the commander's Machine
	/// </summary>
	/// <param name="signal"></param>
	public abstract void Entry(StateSignal signal);

	protected virtual void Awake()
	{
		combatant = GetComponent<Combatant>();
		machine = GetComponent<Machine>();

		logger = FindObjectOfType<Logger>();
		cooldown_timeline = new Timeline(cooldown);
	}

	protected virtual void Update()
	{ cooldown_timeline.Tick(Time.deltaTime); }
}
