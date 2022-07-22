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

    [SerializeField]
    AudioClip grab_clip;

    Usable usable;

    void Use()
    {
        if(output == null){ usable.Notify("Null output!"); return; }

        Satchel satchel = usable.user.GetComponent<Satchel>();

        if(satchel.Contains(output))
        { usable.Notify($"You are already holding {output.name}"); return; }

        if(inputs.Length > 0)
        {
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
                usable.Notify(report);
                return;
            }

            foreach(Ingredient input in inputs)
            {
                satchel.Remove(input);
            }

            if(satchel.Add(output)){ usable.Notify($"Brewed {output.name}"); }
        }
        else
        {
            if(satchel.Add(output)){ AudioWizard._.PlayEffect(grab_clip); }
        }
    }

    void Awake()
    {
        usable = GetComponent<Usable>();
    }

    void Start()
    {
        usable.on_used.AddListener(Use);
        usable.show_prompt = true;
    }
}
