using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelSwarm : MonoBehaviour
{
	[SerializeField]
	Texture2D template;

	[SerializeField]
	GameObject sentinel_prefab;

	Color[] colours;
	int rows;
	int cols;

	Vector3 origin;
	float element_size;

	List<Vector3> positions;
	GameObject[] sentinels;

	float duration = 1;
	float timer;
	bool active;

	void Awake()
	{
		colours = template.GetPixels();
		rows = template.height;
		cols = template.width;

		origin = transform.position;
		element_size = sentinel_prefab.transform.localScale.x;

		positions = new List<Vector3>();

		for(int i = 0; i < rows; i++)
		{
			for(int j = 0; j < cols; j++)
			{
				Color colour = colours[i * cols + j];

				if(colour == Color.black)
				{
					positions.Add(origin + new Vector3(j, i, 0) * element_size);
				}
			}
		}

		sentinels = new GameObject[positions.Count];

		for(int i = 0; i < positions.Count; i++)
		{
			sentinels[i] = Instantiate(sentinel_prefab, null);
			sentinels[i].transform.position = origin;
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.S))
		{
			active = true;
		}
	}

	void FixedUpdate()
	{
		if(active)
		{
			if(timer >= duration)
			{
				active = false;
			}

			float progress = timer / duration;

			for(int i = 0; i < sentinels.Length; i++)
			{
				Vector3 pos = sentinels[i].transform.position;
				Vector3 dest = positions[i];

				sentinels[i].transform.position = Vector3.Lerp(pos, dest, progress);
			}

			timer += Time.fixedDeltaTime;
		}
	}
}
