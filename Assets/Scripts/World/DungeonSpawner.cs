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
		if(dungeon == null){ usable.Fail("Null dungeon!"); return; }
		if(logger.GetRune(dungeon.rune.flag)){ usable.Fail("You have completed this challenge"); return; }

		for(int i = 0; i < dungeon.cards.Length; i++)
		{
			if(logger.GetCard(dungeon.cards[i].flag))
			{
				dungeon_menu_prefab.GetComponent<DungeonMenu>().dungeon = dungeon;
				Instantiate(dungeon_menu_prefab);
				return;
			}
		}

		usable.Fail("No requisite cards in inventory");
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
		usable.show_prompt = true;
		
		zoomline = new Distline(usable.user, transform, zoom_inner_radius, zoom_outer_radius);
	}

	void Update()
	{
		if(zoomline.progress > 0)
		{
			camera_follow.Zoom(zoom_size, zoomline.progress);
		}
	}

	void OnDrawGizmos()
	{
		Vector3 origin = transform.position;
		Vector3 peak = origin + Vector3.up * (zoom_size * 2 - 0.5f);

		Vector3 outer_l = origin - Vector3.right * zoom_outer_radius;
		Vector3 outer_r = origin + Vector3.right * zoom_outer_radius;
		Vector3 inner_l = peak - Vector3.right * zoom_inner_radius;
		Vector3 inner_r = peak + Vector3.right * zoom_inner_radius;

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(outer_l, inner_l);
		Gizmos.DrawLine(inner_l, inner_r);
		Gizmos.DrawLine(inner_r, outer_r);
		Gizmos.DrawLine(outer_r, outer_l);
	}
}
