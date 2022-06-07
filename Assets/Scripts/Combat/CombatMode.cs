using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combatant))]
[RequireComponent(typeof(Machine))]

public abstract class CombatMode : Subcontroller
{
	[SerializeField]
	bool rune_ability;

	[SerializeField]
	[Range(0, 3)]
	int rune_idx;

	protected Combatant combatant;
	protected Machine machine;
	protected Machine.MachineState default_state;

	protected Logger logger;

	public bool unlocked => logger.GetRune(rune_idx * 2) || !rune_ability;
	public bool powered => logger.GetRune(rune_idx * 2 + 1) && rune_ability;

	public void BindDefault(CombatMode mode){ default_state = mode.Entry; }
	public abstract void Entry(StateSignal signal);
	public abstract bool Jump(CombatMode mode);

	protected virtual void Awake()
	{
		combatant = GetComponent<Combatant>();
		machine = GetComponent<Machine>();

		logger = FindObjectOfType<Logger>();
	}
}
