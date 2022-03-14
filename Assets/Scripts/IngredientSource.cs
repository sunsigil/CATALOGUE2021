using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]

public class IngredientSource : MonoBehaviour
{
    [SerializeField]
    Ingredient ingredient;

    Usable usable;

    void Use()
    {
        Satchel satchel = usable.user.GetComponent<Satchel>();
        if(!satchel.Contains(ingredient)){ satchel.Add(ingredient); }
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
        if(usable.usability >= 1)
        {
            Vector3 prompt_position = (transform.position + usable.user.position) * 0.5f;
            prompt_position += Vector3.up * 0.5f;
            InputPrompter._.Draw(InputCode.CONFIRM, prompt_position);
        }
    }
}
