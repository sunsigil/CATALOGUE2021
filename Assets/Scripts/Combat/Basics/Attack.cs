using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The basic unit of combat exchanges
/// </summary>
public class Attack
{
	// State
	Combatant _sender;
	public Combatant sender => _sender;

	int _damage;
	public int damage => _damage;

	Vector3 _velocity;
	public Vector3 velocity => _velocity;

	bool _lethal;
	public bool lethal => _lethal;

	public Attack(Combatant sender, Vector3 velocity, int damage, bool lethal)
	{
		_sender = sender;
		_velocity = velocity;
		_damage = damage;
		_lethal = lethal;
	}
}
