using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Ingredient")]
public class Ingredient : ScriptableObject
{
	[SerializeField]
	Sprite _icon;
	public Sprite icon => _icon;
}
