using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    float smoothing;

    Walker walker;

    public void Snap()
    {
        Vector3 pos = transform.position;
        Vector3 dest = walker.transform.position;
        pos.x = dest.x;

        transform.position = pos;
    }

    void Awake()
    {
        walker = FindObjectOfType<Walker>();
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        Vector3 dest = walker.transform.position;

        pos.x = Mathf.Lerp(pos.x, dest.x, Time.fixedDeltaTime * 3);
        transform.position = pos;
    }
}
