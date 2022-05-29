using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
	Combatant _sender;
	public Combatant sender => _sender;

	int _damage;
	public int damage => _damage;

	Vector3 _velocity;
	public Vector3 velocity => _velocity;

	public Attack(Combatant sender, Vector3 velocity, int damage)
	{
		_sender = sender;
		_velocity = velocity;
		_damage = damage;
	}
}
