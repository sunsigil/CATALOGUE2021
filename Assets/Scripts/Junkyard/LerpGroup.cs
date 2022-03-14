/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpGroup
{
	Dictionary<Transform, Tuple<Vector3, Vector3>> positions;
	Dictionary<Transform, Tuple<Vector3, Vector3>> scales;
	Dictionary<Transform, Timeline> transform_timelines;

	Dictionary<string, Tuple<float, float>> floats;
	Dictionary<string, Timeline> name_timelines;

	public LerpGroup()
	{
		positions = new Dictionary<Transform, Tuple<Vector3, Vector3>>();
		scales = new Dictionary<Transform, Tuple<Vector3, Vector3>>();
		transform_timelines = new Dictionary<Transform, Timeline>();

		floats = new Dictionary<string, Tuple<float, float>>();
		name_timelines = new Dictionary<string, Timeline>();
	}

	public void RegisterPosition(Transform transform, Vector3 start, Vector3 end, Timeline timeline = null)
	{
		positions.Add(transform, new Tuple<Vector3, Vector3>(start, end));
		if(!transform_timelines.ContainsKey(transform)){ transform_timelines.Add(transform, timeline); }
	}

	public void RegisterScale(Transform transform, Vector3 start, Vector3 end, Timeline timeline = null)
	{
		scales.Add(transform, new Tuple<Vector3, Vector3>(start, end));
		if(!transform_timelines.ContainsKey(transform)){ transform_timelines.Add(transform, timeline); }
	}

	public void RegisterFloat(string name, float start, float end, Timeline timeline = null)
	{
		floats.Add(name, new Tuple<float, float>(start, end));
		name_timelines.Add(name, timeline);
	}

	public void Tick(float dt)
	{
		foreach(Transform transform in transform_timelines.Keys)
		{
			Timeline timeline = transform_timelines[transform];
			if(timeline == null){ return; }

			Tuple<Vector3, Vector3> points;

			if(positions.ContainsKey(transform))
			{
				points = positions[transform];
				transform.position = Vector3.Lerp(points.Item1, points.Item2, timeline.progress);
			}
			if(scales.ContainsKey(transform))
			{
				points = scales[transform];
				transform.localScale = Vector3.Lerp(points.Item1, points.Item2, timeline.progress);
			}
		}
	}

	public Vector3 GetPosition(Transform transform, float progress)
	{
		Tuple<Vector3, Vector3> points = positions[transform];
		return Vector3.Lerp(points.Item1, points.Item2, progress);
	}

	public Vector3 GetScale(Transform transform, float progress)
	{
		Tuple<Vector3, Vector3> points = scales[transform];
		return Vector3.Lerp(points.Item1, points.Item2, progress);
	}

	public float GetFloat(string name, float progress)
	{
		Tuple<float, float> points = floats[name];
		return Mathf.Lerp(points.Item1, points.Item2, progress);
	}
}*/
