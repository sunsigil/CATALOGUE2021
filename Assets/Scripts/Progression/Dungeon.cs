using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
