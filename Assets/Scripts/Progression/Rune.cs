using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// [ScriptableObject]
/// Runes unlock and upgrade
/// combat abilities. Rune collection
/// is tracked via bitfields, hence the
/// flag field
/// </summary>
[CreateAssetMenu(menuName="Rune")]
public class Rune : ScriptableObject
{
	[SerializeField]
	Sprite _icon;
	public Sprite icon => _icon;

	[SerializeField]
	RuneFlag _flag;
	public RuneFlag flag => _flag;
}
