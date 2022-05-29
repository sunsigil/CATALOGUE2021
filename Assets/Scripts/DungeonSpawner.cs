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
	float zoom_inner_radius;
	[SerializeField]
	float zoom_outer_radius;

	Usable usable;

	CameraFollow camera_follow;
	Distline zoomline;

	Logger logger;

	void Use()
	{
		bool playable = false;
		for(int i = 0; i < dungeon.cards.Length; i++)
		{
			if(logger.GetCard(dungeon.cards[i].flag)){ playable = true; }
		}

		if(playable)
		{
			dungeon_menu_prefab.GetComponent<DungeonMenu>().dungeon = dungeon;
			Instantiate(dungeon_menu_prefab);
		}
		else
		{
			usable.Fail("No requisite cards in inventory");
		}
	}

	void Awake()
	{
		usable = GetComponent<Usable>();

		camera_follow = FindObjectOfType<CameraFollow>();
		logger = FindObjectOfType<Logger>();
	}

	void Start()
	{
		usable.on_used.AddListener(Use);

		zoomline = new Distline(usable.user, transform, zoom_inner_radius, zoom_outer_radius);
	}

	void Update()
	{
		if(zoomline.progress > 0)
		{
			camera_follow.Zoom(zoom_size, zoomline.progress);
		}
	}
}
