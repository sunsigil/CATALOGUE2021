using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
	Combatant _sender;
	public Combatant sender => _sender;

	int _damage;
	public int damage => _damage;

	public Attack(Combatant sender, int damage)
	{
		_sender = sender;
		_damage = damage;
	}
}
