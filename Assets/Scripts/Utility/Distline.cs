using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Distline
{
	[SerializeField]
	Transform a;
	[SerializeField]
	Transform b;
	[SerializeField]
	float min_dist;
	[SerializeField]
	float max_dist;

	bool horizontal;

	public float distance
	{
		get
		{
			if(horizontal){ return Mathf.Abs(b.position.x - a.position.x); }
			return (b.position - a.position).magnitude;
		}
	}

	public float progress
	{
		get
		{
			if(distance > max_dist){ return 0; }
			if(distance <= min_dist){ return 1; }
			return 1-((distance - min_dist) / (max_dist - min_dist));
		}
	}

	public bool Evaluate(){ return progress >= 1; }

	public Distline(Transform a, Transform b, float min_dist, float max_dist, bool horizontal = true)
	{
		this.a = a;
		this.b = b;
		this.min_dist = min_dist;
		this.max_dist = max_dist;
		this.horizontal = horizontal;
	}
}
