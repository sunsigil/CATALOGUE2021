using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
	[SerializeField]
	Sprite _icon;
	public Sprite icon => _icon;

	[SerializeField]
	CardFlag _flag;
	public CardFlag flag => _flag;
}
