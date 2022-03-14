// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class InputPrompt : MonoBehaviour
// {
// 	static InputPrompt instance;
//     public static InputPrompt _ => instance;
//
// 	SpriteRenderer sprite_renderer;
//
// 	ControlScheme scheme;
//     Dictionary<InputCode, Sprite> sprites;
//
// 	bool visible;
// 	int visible_frames;
//
// 	float pulse;
//
// 	public void Draw(InputCode code, Vector3 position)
// 	{
// 		sprite_renderer.sprite = sprites[code];
// 		transform.position = position;
//
// 		visible = true;
// 		visible_frames = 0;
// 	}
//
// 	void Awake()
// 	{
// 		if(!instance){instance = this;}
// 		else{Destroy(this);}
//
// 		sprite_renderer = GetComponent<SpriteRenderer>();
// 	}
//
// 	void Start()
// 	{
// 		scheme = ControllerRegistry._.scheme;
// 		Dictionary<string, Sprite> string_sprite_map = CowTools.MapResources<Sprite>(System.Enum.GetNames(typeof(InputCode)), "Input Prompts");
// 		sprites = new Dictionary<InputCode, Sprite>();
//
// 		foreach(InputCode code in CowTools.EnumArray<InputCode>())
// 		{
// 			sprites.Add(code, string_sprite_map[code.ToString()]);
// 		}
// 	}
//
// 	void Update()
// 	{
// 		if(visible)
// 		{
// 			transform.localScale = CowTools.ScaleXY(CowTools.Throb(pulse, 1.5f));
//
// 			visible_frames++;
// 			pulse += Time.deltaTime;
// 		}
//
// 		if(visible_frames < 1)
// 		{
// 			sprite_renderer.enabled = true;
// 		}
// 		else
// 		{
// 			visible = false;
// 		}
// 	}
//
// 	void LateUpdate()
// 	{
// 		sprite_renderer.enabled = visible;
// 	}
// }
