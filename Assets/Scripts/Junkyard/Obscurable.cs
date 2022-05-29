/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obscurable : MonoBehaviour
{
    ObscurityRing ring;
    bool _obscured;
    public bool obscured => _obscured;

    void Unobscure()
    {
        _obscured = false;
    }

    void Start()
    {
        float _smallest_distance = Mathf.Infinity;

        foreach(ObscurityRing ring in FindObjectsOfType<ObscurityRing>())
        {
            if(ring.Envelopes(gameObject))
            {
                float distance = ring.DistanceFromCenter(gameObject);

                if(distance <= _smallest_distance)
                {
                    _smallest_distance = distance;

                    this.ring = ring;
                    ring.on_dispelled.AddListener(Unobscure);
                    _obscured = true;
                }
            }
        }
    }
}*/
