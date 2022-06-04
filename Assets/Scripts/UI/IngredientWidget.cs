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
		image.sprite = (ingredient.icon != null) ? ingredient.icon : AssetTools.DefaultResource<Sprite>();
	}

	public bool IsBound(){ return ingredient != null; }
}
