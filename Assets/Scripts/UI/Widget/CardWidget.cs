using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays the icon of whatever card is bound to it.
/// Supports appearance changes which reflect the effects
/// of interaction during Dungeon minigames.
/// </summary>
public class CardWidget : MonoBehaviour
{
	[SerializeField]
	Image image;
	[SerializeField]
	TextMeshProUGUI text;
	[SerializeField]
	Image marker;

	Card _card;
	public Card card => _card;

	public void ToggleMark(bool toggle)
	{ marker.enabled = toggle; }

	public void Bind(object data)
	{
		_card = data as Card;
		image.sprite = (_card != null) ? _card.icon : AssetTools.DefaultResource<Sprite>();
		text.text = (_card != null) ? _card.name : "Null";
	}

	public bool IsBound()
	{ return _card != null; }

	void Awake()
	{ marker.enabled = false; }
}
