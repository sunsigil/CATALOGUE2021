using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] clutter;
    [SerializeField]
    GameObject[] trees;

    [SerializeField]
    float spawn_y;

    [SerializeField]
    [Range(0, 1)]
    float clutter_rate;
    [SerializeField]
    float clutter_padding;
    [SerializeField]
    float clutter_spacing;

    [SerializeField]
    [Range(0, 1)]
    float tree_rate;
    [SerializeField]
    float tree_padding;
    [SerializeField]
    float tree_spacing;

    Mapper mapper;
    AntiTreeVolume[] anti_tree_volumes;

    GameObject clutter_holder;
    GameObject tree_holder;

    float start;
    float end;

    void FillRange
    (
        GameObject[] pieces,
        float rate,
        float padding,
        float spacing,
        GameObject destination
    )
    {
        start = mapper.start.transform.position.x;
        end = mapper.end.transform.position.x;

        while(start < end)
        {
            bool fill = Random.value <= rate;

            if(fill)
            {
                int index = Random.Range(0, pieces.Length);
                GameObject instance = Instantiate(pieces[index], destination.transform);
                float size = instance.GetComponent<SpriteRenderer>().bounds.size.x;
                Vector3 position = new Vector3(start, spawn_y, 0);

                foreach(AntiTreeVolume volume in anti_tree_volumes)
                {
                    if(volume.TreeViolating(start, size))
                    {
                        start = volume.NextBestX(size);
                        position.x = start;
                        break;
                    }
                }

                instance.transform.position = position;

                start += size + padding;
            }
            else
            {
                start += spacing;
            }
        }
    }

    void Start()
    {
        mapper = FindObjectOfType<Mapper>();
        anti_tree_volumes = FindObjectsOfType<AntiTreeVolume>();

        clutter_holder = new GameObject("Clutter Holder");
        tree_holder = new GameObject("Tree Holder");

        FillRange
        (
            clutter,
            clutter_rate,
            clutter_padding,
            clutter_spacing,
            clutter_holder
        );

        FillRange
        (
            trees,
            tree_rate,
            tree_padding,
            tree_spacing,
            tree_holder
        );
    }
}
