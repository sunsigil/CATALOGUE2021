using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPrompter : MonoBehaviour
{
	static InputPrompter instance;
    public static InputPrompter _ => instance;

	[SerializeField]
	GameObject prefab;

	ControlScheme scheme;
    Dictionary<InputCode, Sprite> sprites;

	Transform holder;
 	List<SpriteRenderer> renderers;
	List<InputCode> codes;
	List<Vector3> positions;
	List<bool> log;

	float pulse;

	void Draw(int i)
	{
		SpriteRenderer renderer = renderers[i];
		InputCode code = codes[i];
		Vector3 position = positions[i];

		renderer.sprite = sprites[code];
		renderer.transform.position = position;
		log[i] = false;
	}

	void Discard(int i)
	{
		GameObject prompt = renderers[i].gameObject;

		renderers.RemoveAt(i);
		codes.RemoveAt(i);
		positions.RemoveAt(i);
		log.RemoveAt(i);

		Destroy(prompt);
	}

	public void Request(InputCode code, Vector3 position)
	{
		for(int i = 0; i < renderers.Count; i++)
		{
			InputCode c = codes[i];
			Vector3 p = positions[i];

			if(Vector3.Distance(p, position) <= 0.1f)
			{
				codes[i] = code;
				log[i] = true;
				return;
			}
		}

		SpriteRenderer renderer = Instantiate(prefab, holder).GetComponent<SpriteRenderer>();
		renderers.Add(renderer);
		codes.Add(code);
		positions.Add(position);
		log.Add(true);
	}

	void Awake()
	{
		if(!instance){instance = this;}
		else{Destroy(this);}

		holder = new GameObject("Input Prompts").transform;
		renderers = new List<SpriteRenderer>();
		codes = new List<InputCode>();
		positions = new List<Vector3>();
		log = new List<bool>();
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
		for(int i = 0; i < renderers.Count; i++)
		{
			Draw(i);
		}
	}

	void LateUpdate()
	{
		for(int i = 0; i < renderers.Count; i++)
		{
			if(!log[i]){ Discard(i); i -= 1; }
		}
	}
}
