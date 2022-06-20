using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]

public class IngredientSource : MonoBehaviour
{
    [SerializeField]
    Ingredient[] inputs;
    [SerializeField]
    Ingredient output;

    Usable usable;

    void Use()
    {
        if(output == null){ usable.Fail("Null output!"); return; }

        Satchel satchel = usable.user.GetComponent<Satchel>();

        foreach(Ingredient input in inputs)
        {
            if(!satchel.Contains(input))
            { usable.Fail($"You are missing at least 1 of {inputs.Length} reagents"); return; }
        }
        if(satchel.Contains(output))
        { usable.Fail($"You are already holding {output.name}"); return; }

        foreach(Ingredient input in inputs)
        {
            satchel.Remove(input);
        }
        satchel.Add(output);
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
