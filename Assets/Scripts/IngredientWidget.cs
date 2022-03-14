using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientWidget : Widget<Ingredient>
{
	Image image;

	public override void Bind(Ingredient data)
	{
		base.Bind(data);
		image.sprite = (data.icon != null) ? data.icon : ResourceTools.GetDefault<Sprite>();
	}
}
