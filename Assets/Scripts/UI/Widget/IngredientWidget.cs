using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the icon of whatever
/// ingredient is bound to it
/// </summary>
public class IngredientWidget : MonoBehaviour
{
	[SerializeField]
	Image image;

	Ingredient ingredient;

	public void Bind(Ingredient data)
	{
		ingredient = data;

		if(ingredient == null || ingredient.icon == null)
		{ image.sprite = AssetTools.DefaultResource<Sprite>(); }
		else
		{ image.sprite = ingredient.icon; }
	}

	public bool IsBound()
	{ return ingredient != null; }
}
