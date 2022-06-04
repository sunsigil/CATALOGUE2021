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
	public string life_string => $"{lives} / {max_lives} = {life}";

	[SerializeField]
	protected float hurt_cooldown;
	protected Timeline nohurt_timeline;
	protected bool invincible;

	[Header("Dealing Damage")]

	protected Queue<Attack> incoming;
	protected UnityEvent _on_hit;
	public UnityEvent on_hit => _on_hit;
	protected UnityEvent _on_deplete;
	public UnityEvent on_deplete => _on_deplete;
	protected UnityEvent _on_die;
	public UnityEvent on_die => _on_die;

	public float arena_scale => transform.lossyScale.x;

	protected void ProcessAttack(Attack attack)
	{
		int old_lives = lives;
		lives = Mathf.Clamp(lives - attack.damage, 0, max_lives);

		_on_hit.Invoke();

		if(lives == 0)
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

	public bool RingStrike(Vector3 offset, float radius, Attack attack)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position + offset * arena_scale, radius * arena_scale);

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

	public void Heal(int quant)
	{
		lives = Mathf.Clamp(lives + quant, 0, max_lives);
	}

	protected void Awake()
	{
		lives = max_lives;

		nohurt_timeline = new Timeline(hurt_cooldown);

		incoming = new Queue<Attack>();
		_on_hit = new UnityEvent();
		_on_deplete = new UnityEvent();
		_on_die = new UnityEvent();
	}

	protected void Update()
	{
		nohurt_timeline.Tick(Time.deltaTime);

		if(incoming.Count > 0)
		{ ProcessAttack(incoming.Dequeue()); }
	}
}
