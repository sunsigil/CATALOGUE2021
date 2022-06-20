using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientWidget : MonoBehaviour, IBindable
{
	[SerializeField]
	Image image;

	Ingredient ingredient;

	public void Bind(object data)
	{
		ingredient = data as Ingredient;

		if(ingredient == null || ingredient.icon == null){ image.sprite = AssetTools.DefaultResource<Sprite>(); }
		else{ image.sprite = ingredient.icon; }
	}

	public bool IsBound(){ return ingredient != null; }
}
