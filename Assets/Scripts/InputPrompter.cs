using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPrompter : MonoBehaviour
{
	static InputPrompter instance;
    public static InputPrompter _ => instance;

	SpriteRenderer sprite_renderer;

	ControlScheme scheme;
    Dictionary<InputCode, Sprite> sprites;

	bool visible;
	int visible_frames;

	float pulse;

	public void Draw(InputCode code, Vector3 position)
	{
		sprite_renderer.sprite = sprites[code];
		sprite_renderer.transform.position = position;

		visible = true;
		visible_frames = 0;
	}

	void Awake()
	{
		if(!instance){instance = this;}
		else{Destroy(this);}

		sprite_renderer = new GameObject("Input Prompt").AddComponent<SpriteRenderer>();
		sprite_renderer.sortingLayerID = SortingLayer.NameToID("Overlay");
	}

	void Start()
	{
		scheme = ControllerRegistry._.scheme;
		string[] input_code_names = EnumTools.StringArray<InputCode>();
		Dictionary<string, Sprite> string_sprite_map = ResourceTools.MapResources<Sprite>(input_code_names, "Input Prompts");
		sprites = new Dictionary<InputCode, Sprite>();

		foreach(InputCode code in EnumTools.EnumArray<InputCode>())
		{
			sprites.Add(code, string_sprite_map[code.ToString()]);
		}
	}

	void Update()
	{
		if(visible)
		{
			sprite_renderer.transform.localScale = NumTools.XY_Scale(NumTools.Throb(pulse, 1.5f));

			visible_frames++;
			pulse += Time.deltaTime;
		}

		if(visible_frames < 1)
		{
			sprite_renderer.enabled = true;
		}
		else
		{
			visible = false;
		}
	}

	void LateUpdate()
	{
		sprite_renderer.enabled = visible;
	}
}
