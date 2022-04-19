using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Combatant : MonoBehaviour
{
	[SerializeField]
	protected Faction _faction;
	public Faction faction => _faction;

	[Header("Taking Damage")]

	[SerializeField]
	protected int max_lives;
	protected int lives;
	public float life => (float)lives / (float)max_lives;

	[SerializeField]
	protected float hurt_cooldown;
	protected Timeline hurt_timeline;

	[Header("Dealing Damage")]

	protected Queue<Attack> incoming;
	protected UnityEvent _on_hit;
	public UnityEvent on_hit => _on_hit;
	protected UnityEvent _on_die;
	public UnityEvent on_die => _on_die;

	protected void ProcessAttack(Attack attack)
	{
		lives -= attack.damage;
		lives = Mathf.Clamp(lives, 0, max_lives);

		_on_hit.Invoke();

		if(life <= 0)
		{
			_on_die.Invoke();
		}
	}

	public bool EnqueueAttack(Attack attack)
	{
		if(attack.sender == this){ return false; }
		if(attack.sender.faction == _faction){ return false; }

		incoming.Enqueue(attack);
		return true;
	}

	public bool RingStrike(Vector3 offset, float radius, Attack attack)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position + offset, radius);

        foreach(Collider2D col in cols)
        {
            Combatant target = col.GetComponent<Combatant>();

            if(target != null)
            {
                if(target.EnqueueAttack(attack))
				{
					return true;
				}
            }
        }

		return false;
    }

	protected void Awake()
	{
		lives = max_lives;

		hurt_timeline = new Timeline(hurt_cooldown);

		incoming = new Queue<Attack>();
		_on_hit = new UnityEvent();
		_on_die = new UnityEvent();
	}

	protected void Update()
	{
		hurt_timeline.Tick(Time.deltaTime);

		if(incoming.Count > 0)
		{
			if(hurt_timeline.Evaluate())
			{
				ProcessAttack(incoming.Dequeue());
				hurt_timeline = new Timeline(hurt_cooldown);
			}
			else{ incoming.Dequeue(); }
		}
	}
}
