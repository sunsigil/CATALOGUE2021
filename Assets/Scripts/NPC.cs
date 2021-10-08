using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour, IUsable
{
    [SerializeField]
    GameObject bubble_prefab;

    [SerializeField]
    string[] dialogue;

    [SerializeField]
    float inner_dist_bound;
    [SerializeField]
    float outer_dist_bound;

    [SerializeField]
    float triangle_area;
    [SerializeField]
    float max_altitude;

    Walker walker;

    GameObject bubble;
    TextMeshProUGUI text;
    Vector3 bubble_scale;

    int index;

    void Setup()
    {
        bubble = Instantiate(bubble_prefab);
        text = bubble.GetComponentInChildren<TextMeshProUGUI>();
        bubble.transform.position = transform.position + transform.up * 2;
        bubble_scale = bubble.transform.localScale;

        index = 0;
    }

    void Cleanup()
    {
        Destroy(bubble);
        text = null;
    }

    public bool AssessUsability()
    {
        float walker_dist = Mathf.Abs(transform.position.x - walker.transform.position.x);
        return walker_dist <= inner_dist_bound;
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
        walker = FindObjectOfType<Walker>();
    }

    void Update()
    {
        Vector3 line = transform.position - walker.transform.position;
        float walker_sign = Mathf.Sign(line.x);
        float walker_dist = Mathf.Abs(line.x);

        if(bubble == null){return;}

        if(walker_dist > outer_dist_bound)
        {
            float grace_dist = 1.5f;
            float grace_value = walker_dist - outer_dist_bound;

            Debug.DrawLine(bubble.transform.position, walker.transform.position, Color.red);

            bubble.transform.localScale = bubble_scale * (1-(grace_value/grace_dist));

            if(grace_value >= grace_dist)
            {
                Cleanup();
            }
        }
        else
        {
            // area = 0.5 * base * height
            // height = 2 * area / base
            float height = Mathf.Clamp(2 * triangle_area / walker_dist, 0, max_altitude);

            Vector3 origin = walker.transform.position;
            Vector3 midpoint = line * 0.5f;
            Vector3 altitude = Vector3.Cross(midpoint, Vector3.forward).normalized * height * -walker_sign;
            Vector3 apex = origin + midpoint + altitude;

            Debug.DrawLine(origin, origin + line, Color.red);
            Debug.DrawLine(origin, apex, Color.red);
            Debug.DrawLine(transform.position, apex, Color.red);

            bubble.transform.position = apex;
        }
    }
}
