using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapRing : MonoBehaviour
{
    [SerializeField]
    GameObject icon_prefab;

    GameObject[] icon_objects;
    Dictionary<string, Sprite> icons;

    Mapper mapper;

    public void Refresh()
    {
        if(icon_objects != null)
        {
            for(int i = 0; i < icon_objects.Length; i++){Destroy(icon_objects[i]);}
        }

        icon_objects = new GameObject[mapper.beacons.Count];
        icons = ResourceTools.MapResources<Sprite>(mapper.GetBeaconNames(), "Map Icons");

        for(int i = 0; i < mapper.beacons.Count; i++)
        {
            GameObject beacon = mapper.beacons[i];

            icon_objects[i] = Instantiate(icon_prefab, transform);
            icon_objects[i].transform.localScale *= 0.2f;

            SpriteRenderer sprite_renderer = icon_objects[i].GetComponent<SpriteRenderer>();
            Sprite sprite = icons[beacon.name.ToLower()];
            sprite_renderer.sprite = sprite;
            if(beacon.name.Equals("Catalogue")){sprite_renderer.color = Color.red;}
            else{sprite_renderer.color = Color.black;}

            float radius = 0.65f;
            float angle = mapper.NormalizePosition(beacon) * 2 * Mathf.PI;
            Vector3 position = radius * NumTools.XY_Circle(angle);
            Vector3 orientation = new Vector3(0, 0, angle * Mathf.Rad2Deg - 90);
            icon_objects[i].transform.localPosition = position;
            icon_objects[i].transform.rotation = Quaternion.Euler(orientation);
        }
    }

    void Awake()
    {
        mapper = FindObjectOfType<Mapper>();
    }

    void Start()
    {
        Refresh();
    }
}
