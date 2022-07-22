using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]

public class Combatant : MonoBehaviour
{
	[SerializeField]
	protected Faction _faction;
	public Faction faction => _faction;

	[Header("Positioning")]
	[SerializeField]
	Vector3 offset;

	[Header("Taking Damage")]

	[SerializeField]
	protected int _max_lives;
	public int max_lives => _max_lives;
	protected int _lives;
	public int lives => _lives;
	public float life => (float)_lives / (float)_max_lives;

	[SerializeField]
	protected float hurt_cooldown;
	protected Timeline nohurt_timeline;
	protected bool invincible;

	protected Queue<Attack> incoming;
	protected UnityEvent _on_hurt;
	public UnityEvent on_hurt => _on_hurt;
	protected UnityEvent _on_deplete;
	public UnityEvent on_deplete => _on_deplete;
	protected UnityEvent _on_die;
	public UnityEvent on_die => _on_die;

	protected Arena _arena;
	public Arena arena
	{
		get => _arena;
		set => _arena = value;
	}

	//public float _arena.scale => _arena.scale;
	public Vector3 local_offset => transform.TransformPoint(offset);
	public Vector3 arena_offset => transform.TransformPoint(offset * _arena.scale);

	protected Rigidbody2D rigidbody;

	protected void ProcessAttack(Attack attack)
	{
		int old_lives = _lives;
		_lives = Mathf.Clamp(_lives - attack.damage, 0, _max_lives);

		_on_hurt.Invoke();

		if(_lives == 0)
		{
			if(old_lives != 0)
			{
				_on_deplete.Invoke();
			}
			else if(attack.lethal)
			{
				_on_die.Invoke();
			}
		}
	}

	public void ToggleInvincible(bool toggle){ invincible = toggle; }

	public void Heal(int quant)
	{
		_lives = Mathf.Clamp(_lives + quant, 0, _max_lives);
	}

	public bool EnqueueAttack(Attack attack)
	{
		if(invincible){ return false; }
		if(!nohurt_timeline.Evaluate()){ return false; }

		if(attack.sender == this){ return false; }
		if(attack.sender.faction == _faction){ return false; }

		incoming.Enqueue(attack);
		nohurt_timeline = new Timeline(hurt_cooldown);

		return true;
	}

	public void RingStrike(float radius, Attack attack)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(arena_offset, radius * _arena.scale);

        foreach(Collider2D col in cols)
        {
            Combatant target = col.GetComponent<Combatant>();

            if(target != null)
            {
                target.EnqueueAttack(attack);
            }
        }
    }

	public void ArcStrike(float radius, float phase, float arc, Attack attack)
	{
		Vector3 c = transform.position + offset * _arena.scale;
		float r = radius * _arena.scale;

		float min = phase - arc/2;
		float max = phase + arc/2;

		Collider2D[] cols = Physics2D.OverlapCircleAll(c, r);

        foreach(Collider2D col in cols)
        {
			Vector3 spoke = transform.InverseTransformPoint(col.transform.position);
			float angle = Mathf.Atan2(spoke.y, spoke.x);

			if(angle >= min && angle <= max)
			{
				Combatant target = col.GetComponent<Combatant>();

	            if(target != null)
	            {
	                target.EnqueueAttack(attack);
	            }
			}
        }
	}

	public bool Move(Vector3 destination)
	{
		Vector3 spoke = destination - _arena.center;
		Vector3 dir = spoke.normalized;
		bool full_move = true;

		if(spoke.magnitude >= _arena.limit)
		{
			destination = _arena.center + dir * _arena.limit * 0.95f;
			full_move = false;
		}

		rigidbody.MovePosition(destination);
		return full_move;
	}

	protected void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();

		_lives = _max_lives;

		nohurt_timeline = new Timeline(hurt_cooldown);

		incoming = new Queue<Attack>();
		_on_hurt = new UnityEvent();
		_on_deplete = new UnityEvent();
		_on_die = new UnityEvent();

		_arena = transform.root.GetComponentInChildren<Arena>();
	}

	protected void Update()
	{
		nohurt_timeline.Tick(Time.deltaTime);

		if(incoming.Count > 0)
		{ ProcessAttack(incoming.Dequeue()); }
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if(_arena != null)
		{ Gizmos.DrawSphere(arena_offset, 0.1f * _arena.scale); }
		else
		{ Gizmos.DrawSphere(offset, 0.1f); }
	}
}
