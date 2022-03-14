using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline
{
	float duration;
	float timer;
	public float progress => (duration != 0) ? (timer / duration) : 1;

	public void Initialize(float offset){ timer = Mathf.Clamp(offset, 0, duration); }
	public void Tick(float dt){ timer = Mathf.Clamp(timer + dt, 0, duration); }
	public bool Evaluate(){ return progress >= 1; }

	public Timeline(float duration)
	{
		this.duration = duration;
		timer = 0;
	}
}
