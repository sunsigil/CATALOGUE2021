using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]

public class Kiln : MonoBehaviour
{
    [SerializeField]
    GameObject rune_get_prefab;

    [SerializeField]
    Ingredient input;
    [SerializeField]
    Rune output;

    Usable usable;

    Satchel satchel;
    Logger logger;

    void Use()
    {
        if(!satchel.Contains(input)){ return; }
        if(logger.GetRune(output.flag)){ return; }

        satchel.Remove(input);
        logger.AddRune(output.flag);

        rune_get_prefab.GetComponent<RuneGet>().rune = output;
        Instantiate(rune_get_prefab);
    }

    void Awake()
    {
        usable = GetComponent<Usable>();

        satchel = FindObjectOfType<Satchel>();
        logger = FindObjectOfType<Logger>();
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
