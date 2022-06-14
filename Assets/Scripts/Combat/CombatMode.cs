using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combatant))]
[RequireComponent(typeof(Machine))]

public abstract class CombatMode : Subcontroller
{
	[SerializeField]
	protected bool rune_ability;
	[SerializeField]
	[Range(0, 3)]
	protected int rune_idx;

	[SerializeField]
	protected float cooldown;

	[SerializeField]
	protected bool lethal;

	protected Logger logger;

	protected Combatant combatant;
	protected Machine machine;
	protected Machine.MachineState default_state;
	protected Timeline cooldown_timeline;

	public bool unlocked => logger.GetRune(rune_idx * 2) || !rune_ability;
	public bool powered => logger.GetRune(rune_idx * 2 + 1) && rune_ability;
	public bool ready => cooldown_timeline.Evaluate();

	public void BindDefault(CombatMode mode){ default_state = mode.Entry; }
	public abstract void Entry(StateSignal signal);

	protected virtual void Awake()
	{
		combatant = GetComponent<Combatant>();
		machine = GetComponent<Machine>();

		logger = FindObjectOfType<Logger>();
		cooldown_timeline = new Timeline(0);
	}
}
