using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]

public class DungeonSpawner : MonoBehaviour
{
	[SerializeField]
	GameObject dungeon_menu_prefab;

	[SerializeField]
	Dungeon dungeon;
	[SerializeField]
	float zoom_size;
	[SerializeField]
	float zoom_radius;

	Usable usable;

	CameraFollow camera_follow;
	Distline zoomline;

	void Use()
	{
		Instantiate(dungeon_menu_prefab);
	}

	void Awake()
	{
		usable = GetComponent<Usable>();

		camera_follow = FindObjectOfType<CameraFollow>();
	}

	void Start()
	{
		usable.on_used.AddListener(Use);

		zoomline = new Distline(usable.user, transform, 0, zoom_radius);
	}

	void Update()
	{
		if(zoomline.progress > 0)
		{
			camera_follow.Zoom(zoom_size, zoomline.progress);
		}
	}
}
