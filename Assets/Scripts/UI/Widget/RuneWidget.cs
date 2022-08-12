using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays the icon of whatever
/// rune is bound to it
/// </summary>
public class RuneWidget : MonoBehaviour
{
	[SerializeField]
	Image image;
	[SerializeField]
	Image ornament;

	[SerializeField]
	Rune _rune;
	public Rune rune => _rune;

	public void Bind(Rune data)
	{
		_rune = data;
		image.sprite = (_rune != null) ? _rune.icon : AssetTools.DefaultResource<Sprite>();
	}

	public bool IsBound()
	{ return _rune != null; }
}
