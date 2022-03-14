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
    string[] dialogue;

    [SerializeField]
    float triangle_area;
    [SerializeField]
    float max_altitude;

    Usable usable;
    Obscurable obscurable;

    GameObject bubble;
    TextMeshProUGUI text;
    Vector3 bubble_scale;

    int index;

    void Setup()
    {
        bubble = Instantiate(bubble_prefab);
        text = bubble.GetComponentInChildren<TextMeshProUGUI>();
        text.font = obscurable.obscured ? obscured_font : unobscured_font;
        bubble.transform.position = transform.position + transform.up * 2;
        bubble_scale = bubble.transform.localScale;

        index = 0;
    }

    void Cleanup()
    {
        Destroy(bubble);
        text = null;
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
    }

    void Start()
    {
        usable.on_used.AddListener(Use);
    }

    void Update()
    {
        if(bubble == null)
        {
            if(usable.usability >= 1)
            {
                Vector3 prompt_position = usable.user.position + (transform.position - usable.user.position) * 0.5f;
                prompt_position += Vector3.up * 0.5f;
                InputPrompter._.Draw(InputCode.CONFIRM, prompt_position);
            }

            return;
        }

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

                // area = 0.5 * base * height
                // height = 2 * area / base
                float height = Mathf.Clamp(2 * triangle_area / usable.distline.distance, 0, max_altitude);

                Vector3 origin = usable.user.position;
                Vector3 midpoint = beeline * 0.5f;
                Vector3 altitude = Vector3.Cross(midpoint, Vector3.forward).normalized * height;/* * -Mathf.Sign(usable.signed_user_distance);*/
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
