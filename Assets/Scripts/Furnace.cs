using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]

public class Furnace : MonoBehaviour
{
	[SerializeField]
	GameObject rune_get_prefab;

    [SerializeField]
    Ingredient input;
    [SerializeField]
    Rune output;

    Usable usable;

    void Use()
    {
        Satchel satchel = usable.user.GetComponent<Satchel>();
		Logger logger = usable.user.GetComponent<Logger>();

        if(!satchel.Contains(input)){ return; }

		satchel.Remove(input);
		logger.AddRune(output.flag);
		rune_get_prefab.GetComponent<RuneGet>().rune = output;
		Instantiate(rune_get_prefab);
    }

    void Awake()
    {
        usable = GetComponent<Usable>();
    }

    void Start()
    {
        usable.on_used.AddListener(Use);
    }

    void Update()
    {
        usable.show_prompt = true;
    }
}
