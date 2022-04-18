using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Usable))]
[RequireComponent(typeof(Obscurable))]

public class NPC : MonoBehaviour
{
    [SerializeField]
    GameObject bubble_prefab;
    [SerializeField]
    TMP_FontAsset obscured_font;
    [SerializeField]
    TMP_FontAsset unobscured_font;
    [SerializeField]
    GameObject card_get_prefab;
    [SerializeField]
    GameObject rune_get_prefab;

    [SerializeField]
    string[] standard_dialogue;

    [SerializeField]
    string[] card_dialogue;
    [SerializeField]
    Card card;

    [SerializeField]
    string[] essence_dialogue;
    [SerializeField]
    Ingredient input;
    [SerializeField]
    Rune output;

    [SerializeField]
    float triangle_area;
    [SerializeField]
    float max_altitude;

    Usable usable;
    Obscurable obscurable;
    SpawnQueue spawn_queue;

    Logger logger;
    Satchel satchel;

    GameObject bubble;
    TextMeshProUGUI text;
    Vector3 bubble_scale;

    string[] dialogue;
    int index;

    void Setup()
    {
        bubble = Instantiate(bubble_prefab);
        text = bubble.GetComponentInChildren<TextMeshProUGUI>();
        text.font = obscurable.obscured ? obscured_font : unobscured_font;
        bubble.transform.position = transform.position + transform.up * 2;
        bubble_scale = bubble.transform.localScale;

        if(card && !logger.GetCard(card.flag))
        {
            dialogue = card_dialogue;
        }
        else if(input && satchel.Contains(input) && !logger.GetRune(output.flag))
        {
            dialogue = essence_dialogue;
        }
        else
        {
            dialogue = standard_dialogue;
        }

        index = 0;
    }

    void Cleanup()
    {
        Destroy(bubble);
        text = null;

        if(dialogue == card_dialogue)
        {
            logger.AddCard(card.flag);
            card_get_prefab.GetComponent<CardGet>().card = card;
            spawn_queue.Add(card_get_prefab);
        }

        if(dialogue == essence_dialogue)
        {
            satchel.Remove(input);
            logger.AddRune(output.flag);
            rune_get_prefab.GetComponent<RuneGet>().rune = output;
            spawn_queue.Add(rune_get_prefab);
        }
    }

    public void Use()
    {
        if(bubble == null)
        {
            Setup();
        }
        else if(index < dialogue.Length-1)
        {
            index++;
        }
        else
        {
            Cleanup();
            return;
        }

        text.text = dialogue[index];
    }

    void Awake()
    {
        usable = GetComponent<Usable>();
        obscurable = GetComponent<Obscurable>();
        spawn_queue = GetComponent<SpawnQueue>();

        logger = FindObjectOfType<Logger>();
        satchel = FindObjectOfType<Satchel>();
    }

    void Start()
    {
        usable.on_used.AddListener(Use);
    }

    void Update()
    {
        if(bubble == null)
        {
            usable.show_prompt = true;
            return;
        }
        usable.show_prompt = false;

        if(usable.usability > 0)
        {
            if(usable.usability < 1)
            {
                bubble.transform.localScale = bubble_scale * Mathf.SmoothStep(0, 1, usable.usability);
            }
            else
            {
                Vector3 y_corrected_pos = transform.position;
                y_corrected_pos.y = usable.user.position.y;
                Vector3 beeline = y_corrected_pos - usable.user.position;
                float dist_sign = Mathf.Sign(beeline.x);

                // area = 0.5 * base * height
                // height = 2 * area / base
                float height = Mathf.Clamp(2 * triangle_area / usable.distline.distance, 0, max_altitude);

                Vector3 origin = usable.user.position;
                Vector3 midpoint = beeline * 0.5f;
                Vector3 altitude = Vector3.Cross(midpoint, Vector3.forward).normalized * height * -dist_sign;
                Vector3 apex = origin + midpoint + altitude;

                bubble.transform.position = apex;
            }
        }
        else
        {
            Cleanup();
        }
    }
}
