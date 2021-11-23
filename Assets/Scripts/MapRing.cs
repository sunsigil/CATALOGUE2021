using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapRing : MonoBehaviour
{
    [SerializeField]
    GameObject beacon_prefab;

    GameObject[] beacon_objects;

    Mapper mapper;

    public void Refresh()
    {
        if(beacon_objects != null)
        {
            for(int i = 0; i < beacon_objects.Length; i++){Destroy(beacon_objects[i]);}
        }

        beacon_objects = new GameObject[mapper.beacons.Count];

        for(int i = 0; i < mapper.beacons.Count; i++)
        {
            GameObject beacon = mapper.beacons[i];

            beacon_objects[i] = Instantiate(beacon_prefab, transform);
            TextMeshProUGUI text = beacon_objects[i].GetComponentInChildren<TextMeshProUGUI>();
            text.text = beacon.name;
            if(beacon.name.Equals("Catalogue")){text.color = Color.red;}

            float radius = 0.55f;
            float angle = mapper.NormalizePosition(beacon) * 2 * Mathf.PI;
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Vector3 orientation = Vector3.forward * angle * Mathf.Rad2Deg;
            beacon_objects[i].transform.localPosition = position;
            beacon_objects[i].transform.rotation = Quaternion.Euler(orientation);
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
