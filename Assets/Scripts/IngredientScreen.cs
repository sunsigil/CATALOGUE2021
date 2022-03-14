using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientScreen : MonoBehaviour
{
	ArcGroup<IngredientWidget, Ingredient> group;
	Satchel satchel;

	void Awake()
	{
		group = GetComponent<ArcGroup<IngredientWidget, Ingredient>>();
		satchel = GetComponent<Satchel>();
	}

	void Start()
	{
		satchel.on_add.AddListener(delegate{ group.Fill(satchel.contents); });
		satchel.on_remove.AddListener(delegate{ group.Fill(satchel.contents); });
	}
}
