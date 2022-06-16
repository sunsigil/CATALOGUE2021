using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

	public float arena_scale => transform.lossyScale.x;
	public Vector3 local_offset => transform.TransformPoint(offset);
	public Vector3 arena_offset => transform.TransformPoint(offset * arena_scale);

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
        Collider2D[] cols = Physics2D.OverlapCircleAll(arena_offset, radius * arena_scale);

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
		Vector3 c = transform.position + offset * arena_scale;
		float r = radius * arena_scale;

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

	public void Heal(int quant)
	{
		_lives = Mathf.Clamp(_lives + quant, 0, _max_lives);
	}

	protected void Awake()
	{
		_lives = _max_lives;

		nohurt_timeline = new Timeline(hurt_cooldown);

		incoming = new Queue<Attack>();
		_on_hurt = new UnityEvent();
		_on_deplete = new UnityEvent();
		_on_die = new UnityEvent();
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
		Gizmos.DrawSphere(arena_offset, 0.1f * arena_scale);
	}
}
