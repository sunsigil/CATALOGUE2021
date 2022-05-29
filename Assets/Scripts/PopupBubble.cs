using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupBubble : Controller
{
    [SerializeField]
    string _message;
	public string message
	{
		get => _message;
		set => _message = value;
	}

	TextMeshProUGUI text;

    void Awake()
    {
		text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
	{
        text.text = _message;

		if(Pressed(InputCode.CONFIRM))
		{
			Destroy(gameObject);
		}
	}
}
