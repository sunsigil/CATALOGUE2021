using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combatant))]
[RequireComponent(typeof(Machine))]

public abstract class CombatMode : Subcontroller
{
	protected Combatant combatant;
	protected Machine machine;
	protected Machine.MachineState next;

	public abstract void Entry(StateSignal signal);
	public abstract bool Jump(CombatMode mode);

	protected virtual void Awake()
	{
		combatant = GetComponent<Combatant>();
		machine = GetComponent<Machine>();
	}
}