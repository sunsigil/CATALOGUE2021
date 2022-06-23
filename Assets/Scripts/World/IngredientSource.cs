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

        if(satchel.Contains(output))
        { usable.Fail($"You are already holding {output.name}"); return; }

        List<Ingredient> misses = new List<Ingredient>();
        foreach(Ingredient input in inputs)
        {
            if(!satchel.Contains(input))
            { misses.Add(input); }
        }

        if(misses.Count > 0)
        {
            string report = "Missing reagents: ";
            for(int i = 0; i < misses.Count; i++)
            {
                report += misses[i].name;
                if(i < misses.Count-1){ report += ", "; }
            }
            usable.Fail(report);
            return;
        }

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
