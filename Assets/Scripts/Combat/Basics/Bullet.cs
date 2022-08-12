using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A combatant entirely controlled
/// by its given velocity. Attacks once and
/// dies upon collision.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : Combatant
{
	// Settings
	[SerializeField]
	int damage;
	[SerializeField]
	float spin;

	// Components
	CircleCollider2D collider;

	// State
	Vector3 _velocity;
	public Vector3 velocity
	{
		get => _velocity;
		set => _velocity = value;
	}

	bool _lethal;
	public bool lethal
	{
		get => _lethal;
		set => _lethal = value;
	}

	void Die()
	{ Destroy(gameObject); }

	void Awake()
	{
		base.Awake();

		collider = GetComponent<CircleCollider2D>();

		on_deplete.AddListener(Die);
		on_die.AddListener(Die);
	}

	void FixedUpdate()
	{
		// Spin is purely aesthetic
		if(spin > 0)
		{ transform.Rotate(NumTools.XY_Omega(spin)); }
		else
		{ transform.rotation = NumTools.XY_Quat(_velocity, -90); }
		
		// Die if blocked by arena wall
		if(!Move(transform.position + _velocity * Time.fixedDeltaTime))
		{ Die(); }
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Combatant other = col.GetComponent<Combatant>();

		if(other && other.faction != faction)
		{
			RingStrike(collider.radius, new Attack(this, _velocity, damage, _lethal));
			Die();
		}
	}
}
