using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine
{
	public delegate void MachineState(StateSignal signal);

	MachineState state;

	public void Tick(){ state(StateSignal.TICK); }
	public void Transition(MachineState state)
	{
		if(this.state != null){ this.state(StateSignal.EXIT); }
		this.state = state;
		if(this.state != null){ this.state(StateSignal.ENTER); }
	}

	public Machine(MachineState state)
	{
		Transition(state);
	}
}
