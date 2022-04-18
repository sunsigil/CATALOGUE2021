using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuneWidget : MonoBehaviour
{
	[SerializeField]
	Image image;
	[SerializeField]
	Image ornament;

	Rune _rune;
	public Rune rune => _rune;

	public void Bind(Rune data)
	{
		_rune = data;
		image.sprite = (_rune != null) ? _rune.icon : ResourceTools.GetDefault<Sprite>();
	}

	public bool IsBound(){ return _rune != null; }

	void Update()
	{
		ornament.transform.rotation = NumTools.PinwheelRot(Time.time);
	}
}
