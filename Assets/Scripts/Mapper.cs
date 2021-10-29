using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapper : MonoBehaviour
{
    GameObject start;
    GameObject end;
    List<GameObject> _beacons;
    public List<GameObject> beacons => _beacons;

    public void RegisterBeacon(GameObject beacon)
    {
        _beacons.Add(beacon);
    }

    public float NormalizePosition(GameObject map_element)
    {
        float start_x = start.transform.position.x;
        float end_x = end.transform.position.x;
        float element_x = map_element.transform.position.x;

        float span = end_x - start_x;
        return  (element_x - start_x) / span;
    }

    void Awake()
    {
        start = GameObject.FindWithTag("Map Start");
        end = GameObject.FindWithTag("Map End");

        _beacons = new List<GameObject>();
        foreach(GameObject beacon in GameObject.FindGameObjectsWithTag("Map Beacon"))
        {
            _beacons.Add(beacon);
        }
    }
}
