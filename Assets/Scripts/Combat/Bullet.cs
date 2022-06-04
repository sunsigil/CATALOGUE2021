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

	void Awake()
	{
		base.Awake();

		rigidbody = GetComponent<Rigidbody2D>();
		collider = GetComponent<CircleCollider2D>();
	}

	void FixedUpdate()
	{
		transform.Rotate(NumTools.PinwheelVelocity(2));
		rigidbody.MovePosition(transform.position + _velocity * Time.fixedDeltaTime);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Combatant other = col.GetComponent<Combatant>();

		if(other == null || other.faction != faction)
		{
			RingStrike(collider.offset, collider.radius, new Attack(this, _velocity, damage, true));
			Destroy(gameObject);
		}
	}
}