using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSnapshot
{
	Vector3 scale;
	Quaternion rotation;
	Vector3 position;

	public static TransformSnapshot Interpolate(TransformSnapshot a, TransformSnapshot b, float t)
	{
		return new TransformSnapshot
		(
			Vector3.Lerp(a.scale, b.scale, t),
			Quaternion.Slerp(a.rotation, b.rotation, t),
			Vector3.Lerp(a.position, b.position, t)
		);
	}

	public void Read(Transform transform)
	{
		scale = transform.localScale;
		rotation = transform.rotation;
		position = transform.position;
	}

	public void Write(Transform transform)
	{
		transform.localScale = scale;
		transform.rotation = rotation;
		transform.position = position;
	}

	public TransformSnapshot(Vector3 scale, Quaternion rotation, Vector3 position)
	{
		this.scale = scale;
		this.rotation = rotation;
		this.position = position;
	}

	public TransformSnapshot(Transform transform)
	{
		Read(transform);
	}
}
