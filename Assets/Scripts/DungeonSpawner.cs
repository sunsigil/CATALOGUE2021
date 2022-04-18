using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]

public class DungeonSpawner : MonoBehaviour
{
	[SerializeField]
	GameObject dungeon_menu;

	[SerializeField]
	Dungeon dungeon;

	Usable usable;
	SpawnQueue spawn_queue;

	void Use()
	{
		spawn_queue.Add(dungeon_menu);
	}

	void Awake()
	{
		usable = GetComponent<Usable>();
		spawn_queue = GetComponent<SpawnQueue>();

		dungeon_menu.GetComponent<DungeonMenu>().dungeon = dungeon;
	}

	void Start()
	{
		usable.on_used.AddListener(Use);
	}
}
