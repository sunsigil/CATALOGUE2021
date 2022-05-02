using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelRing : MonoBehaviour
{
	Timeline timeline;
	float max_radius;

	public void Activate(float duration, float radius)
	{
		timeline = new Timeline(duration);
		max_radius = radius;
	}

	void Update()
	{
		if(timeline == null || timeline.Evaluate())
		{
			transform.localScale = Vector3.zero;
		}
		else
		{
			timeline.Tick(Time.deltaTime);
			transform.localScale = NumTools.XY_Scale(Mathf.Lerp(0, max_radius, timeline.progress));
		}
	}
}
