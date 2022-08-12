using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cards owned by a player can
/// be used in Dungeon minigames.
/// Rune collection is tracked
/// via bitfields, hence the
/// flag field
/// </summary>
[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
	[SerializeField]
	string _name;
	public string name => _name;
	
	[SerializeField]
	Sprite _icon;
	public Sprite icon => _icon;

	[SerializeField]
	CardFlag _flag;
	public CardFlag flag => _flag;
}
