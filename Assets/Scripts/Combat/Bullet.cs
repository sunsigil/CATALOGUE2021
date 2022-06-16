using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class Bullet : Combatant
{
	[SerializeField]
	int damage;

	Rigidbody2D rigidbody;
	CircleCollider2D collider;

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
	{
		Destroy(gameObject);
	}

	void Awake()
	{
		base.Awake();

		rigidbody = GetComponent<Rigidbody2D>();
		collider = GetComponent<CircleCollider2D>();

		on_deplete.AddListener(Die);
		on_die.AddListener(Die);
	}

	void FixedUpdate()
	{
		transform.Rotate(NumTools.XY_Omega(2));
		rigidbody.MovePosition(transform.position + _velocity * Time.fixedDeltaTime);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Combatant other = col.GetComponent<Combatant>();

		if(other == null || other.faction != faction)
		{
			RingStrike(collider.radius, new Attack(this, _velocity, damage, _lethal));
			Die();
		}
	}
}
