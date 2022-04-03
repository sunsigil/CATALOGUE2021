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
        Satchel satchel = usable.user.GetComponent<Satchel>();

        foreach(Ingredient input in inputs)
        {
            if(!satchel.Contains(input)){ return; }
        }
        if(satchel.Contains(output)){ return; }

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
