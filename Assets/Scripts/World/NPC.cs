using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Usable))]
[RequireComponent(typeof(SpawnQueue))]

public class NPC : MonoBehaviour
{
    GameObject dialogue_bubble_prefab;
    GameObject card_get_prefab;
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
    SpawnQueue spawn_queue;

    Logger logger;
    Satchel satchel;

    GameObject dialogue_bubble;
    TextMeshProUGUI text;

    string[] dialogue;
    int index;

    void Setup()
    {
        dialogue_bubble = Instantiate(dialogue_bubble_prefab);
        text = dialogue_bubble.GetComponentInChildren<TextMeshProUGUI>();

        dialogue = null;
        index = 0;

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

        if(dialogue == null)
        {
            usable.Fail("Null dialogue!");
            return;
        }
    }

    void Cleanup()
    {
        Destroy(dialogue_bubble);
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
        if(dialogue_bubble == null)
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
        dialogue_bubble_prefab = Resources.Load<GameObject>("Dialogue Bubble");
        rune_get_prefab = Resources.Load<GameObject>("Rune Get");
        card_get_prefab = Resources.Load<GameObject>("Card Get");

        usable = GetComponent<Usable>();
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
        if(dialogue_bubble == null)
        {
            usable.show_prompt = true;
            return;
        }
        usable.show_prompt = false;

        if(usable.usability > 0)
        {
            if(usable.usability < 1)
            {
                dialogue_bubble.transform.localScale = NumTools.XY_Scale(Mathf.SmoothStep(0, 1, usable.usability));
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

                dialogue_bubble.transform.position = apex;
            }
        }
        else
        {
            Cleanup();
        }
    }

    void OnDrawGizmos()
    {
        // A = W * H * 0.5
        float arm_len = triangle_area / max_altitude;

        Vector3 origin = transform.Find("Prompt Anchor").position;
        Vector3 left = origin - Vector3.right * arm_len;
        Vector3 right = origin + Vector3.right * arm_len;
        Vector3 peak = origin + Vector3.up * max_altitude;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(left, peak);
        Gizmos.DrawLine(peak, right);
        Gizmos.DrawLine(right, left);
    }
}
