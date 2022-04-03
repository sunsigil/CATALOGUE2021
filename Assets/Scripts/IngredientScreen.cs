using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientScreen : MonoBehaviour
{
	ArcGroup group;
	Satchel satchel;

	void Awake()
	{
		group = GetComponent<ArcGroup>();
		satchel = FindObjectOfType<Satchel>();
	}

	void Start()
	{
		satchel.on_add.AddListener(delegate{ group.Fill(satchel.contents); });
		satchel.on_remove.AddListener(delegate{ group.Fill(satchel.contents); });
	}
}
