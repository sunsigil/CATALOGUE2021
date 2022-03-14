using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour
{
	[SerializeField]
	TerritoryProfile profile;

	float weight_sum;

	Prop GetRandomProp()
	{
		int i = 0;
		float rand_weight = Random.value * weight_sum;

		while(i < profile.props.Length)
		{
			rand_weight -= profile.props[i].weight;

			if(rand_weight <= 0){break;}

			i++;
		}

		i = Mathf.Clamp(i, 0, profile.props.Length-1);
		return profile.props[i];
	}

	void Awake()
	{
		weight_sum = 0;
		foreach(Prop prop in profile.props)
		{
			weight_sum += prop.weight;
		}

		float width = transform.localScale.x;
		float usable_space = width * profile.saturation;
		float used_space = 0;

		List<Prop> instances = new List<Prop>();
		Prop last_prop = null;

		while(used_space < usable_space)
		{
			Prop prop = GetRandomProp();

			if(profile.props.Length > 1)
			{
				while(prop == last_prop)
				{
					prop = GetRandomProp();
				}
			}
			last_prop = prop;

			Prop instance = Instantiate(prop).GetComponent<Prop>();

			if((used_space + instance.width) <= usable_space)
			{
				instances.Add(instance);
				used_space += instance.width;
			}
			else
			{
				Destroy(instance.gameObject);
				break;
			}
		}

		float remaining_space = width - used_space;
		float padding = remaining_space / (instances.Count + 1);

		Vector3 spawn_point = transform.position;
		spawn_point +=  -Vector3.right * width/2;

		foreach(Prop instance in instances)
		{
			spawn_point += Vector3.right * (instance.width/2 + padding);
			instance.transform.position = spawn_point;
			spawn_point += Vector3.right * (instance.width/2);

			if(profile.override_sorting)
			{
				instance.SetSorting(profile.layer, profile.order);
			}
		}
	}

	void OnDrawGizmos()
	{
		Vector3 grounded_position = transform.position + Vector3.up * transform.localScale.y/2;

		if(profile != null)
		{
			Color colour = profile.colour;
			colour.a = 0.5f;
			Gizmos.color = colour;
			Gizmos.DrawCube(grounded_position, transform.localScale);
		}
		else
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(grounded_position, transform.localScale);
		}
	}
}
