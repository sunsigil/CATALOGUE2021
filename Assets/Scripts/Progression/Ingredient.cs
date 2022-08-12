using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ingredients are stored in
/// the Satchel and used up during
/// crafting operations.
/// </summary>
[CreateAssetMenu(menuName="Ingredient")]
public class Ingredient : ScriptableObject
{
	[SerializeField]
	string _name;
	public string name => _name;
	
	[SerializeField]
	Sprite _icon;
	public Sprite icon => _icon;
}
