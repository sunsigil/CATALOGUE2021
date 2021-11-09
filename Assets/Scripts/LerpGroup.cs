using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpGroup
{
	List<Transform> scale_transforms;
	List<Vector3> start_scales;
	List<Vector3> end_scales;

	List<Transform> position_transforms;
	List<Vector3> start_positions;
	List<Vector3> end_positions;

	Dictionary<string, float> start_values;
	Dictionary<string, float> end_values;

	float offset;

	public LerpGroup()
	{
		scale_transforms = new List<Transform>();
		start_scales = new List<Vector3>();
		end_scales = new List<Vector3>();

		position_transforms = new List<Transform>();
		start_positions = new List<Vector3>();
		end_positions = new List<Vector3>();

		start_values = new Dictionary<string, float>();
		end_values = new Dictionary<string, float>();
	}

	public void RegisterScale(Transform transform, Vector3 start_scale, Vector3 end_scale)
	{
		scale_transforms.Add(transform);
		start_scales.Add(start_scale);
		end_scales.Add(end_scale);
	}

	public void RegisterPosition(Transform transform, Vector3 start_position, Vector3 end_position)
	{
		position_transforms.Add(transform);
		start_positions.Add(start_position);
		end_positions.Add(end_position);
	}

	public void RegisterFloat(string name, float start_value, float end_value)
	{
		start_values.Add(name, start_value);
		end_values.Add(name, end_value);
	}

	public void Offset(float offset)
	{
		this.offset = Mathf.Clamp(offset, -1, 1);
	}

	public void UpdateTransforms(float progress)
	{
		progress = Mathf.Clamp(progress + offset, 0, 1);

		for(int i = 0; i < scale_transforms.Count; i++)
		{
			scale_transforms[i].localScale = Vector3.Lerp(start_scales[i], end_scales[i], progress);
		}

		for(int i = 0; i < position_transforms.Count; i++)
		{
			position_transforms[i].position = Vector3.Lerp(start_positions[i], end_positions[i], progress);
		}
	}

	public float UpdateFloat(string name, float progress)
	{
		progress = Mathf.Clamp(progress + offset, 0, 1);

		return Mathf.Lerp(start_values[name], end_values[name], progress);
	}
}
