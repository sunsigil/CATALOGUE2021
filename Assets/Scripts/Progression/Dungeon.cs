using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dungeons contain data which
/// DungeonSpawners use to set up
/// minigames and reward players
/// </summary>
[CreateAssetMenu(menuName="Dungeon")]
public class Dungeon : ScriptableObject
{
	[SerializeField]
	Card[] _cards;
	public Card[] cards => _cards;

	[SerializeField]
	Rune _rune;
	public Rune rune => _rune;
}
