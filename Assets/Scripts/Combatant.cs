using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Combatant : MonoBehaviour
{
	[SerializeField]
	Faction _faction;
	public Faction faction => _faction;

	[SerializeField]
	int max_lives;
	int lives;
	public float life => (float)lives / (float)max_lives;

	[SerializeField]
	float damage_cooldown;
	Timeline damage_timeline;

	Queue<Attack> incoming;
	UnityEvent _on_hit;
	public UnityEvent on_hit => _on_hit;
	UnityEvent _on_die;
	public UnityEvent on_die => _on_die;

	[SerializeField]
    bool talkative;

	void ProcessAttack(Attack attack)
	{
		if(talkative){ print($"Procesing attack (1 of {incoming.Count + 1}) from {attack.sender.gameObject}"); }

		lives -= attack.damage;
		lives = Mathf.Clamp(lives, 0, max_lives);

		_on_hit.Invoke();

		if(life <= 0)
		{
			_on_die.Invoke();
		}
	}

	public void EnqueueAttack(Attack attack)
	{
		if(attack.sender == this){ return; }
		if(attack.sender.faction == _faction){ return; }

		if(talkative){ print($"Enqueueing attack from {attack.sender.gameObject}"); }

		incoming.Enqueue(attack);
	}

	void Awake()
	{
		lives = max_lives;

		damage_timeline = new Timeline(damage_cooldown);

		incoming = new Queue<Attack>();
		_on_hit = new UnityEvent();
		_on_die = new UnityEvent();
	}

	void Update()
	{
		damage_timeline.Tick(Time.deltaTime);

		if(incoming.Count > 0)
		{
			if(damage_timeline.Evaluate())
			{
				if(talkative){ print($"Dequeueing an attack..."); }

				ProcessAttack(incoming.Dequeue());
				damage_timeline = new Timeline(damage_cooldown);
			}
			else{ incoming.Dequeue(); }
		}
	}
}
