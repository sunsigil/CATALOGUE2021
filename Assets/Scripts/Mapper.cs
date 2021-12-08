using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapper : MonoBehaviour
{
    GameObject _start;
    public GameObject start => _start;
    GameObject _end;
    public GameObject end => _end;
    List<GameObject> _beacons;
    public List<GameObject> beacons => _beacons;

    public float NormalizePosition(GameObject map_element)
    {
        float _start_x = _start.transform.position.x;
        float _end_x = _end.transform.position.x;
        float element_x = map_element.transform.position.x;

        float span = _end_x - _start_x;
        return  (element_x - _start_x) / span;
    }

    void Awake()
    {
        _start = GameObject.FindWithTag("Map Start");
        _end = GameObject.FindWithTag("Map End");

        _beacons = new List<GameObject>();
        foreach(GameObject beacon in GameObject.FindGameObjectsWithTag("Map Beacon"))
        {
            _beacons.Add(beacon);
        }
    }

    void OnDrawGizmos()
    {
        if(_start == null || _end == null)
        {
            _start = GameObject.FindWithTag("Map Start");
            _end = GameObject.FindWithTag("Map End");
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_start.transform.position, _end.transform.position);
    }
}
