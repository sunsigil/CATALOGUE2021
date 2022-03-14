using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    float smoothing;

    Walker walker;

    Vector3 pos;
    Vector3 dest;

    public void Snap()
    {
        pos = transform.position;

        dest = (walker != null) ? walker.transform.position : Vector3.zero;
        pos.x = dest.x;

        transform.position = pos;
    }

    void Awake()
    {
        walker = FindObjectOfType<Walker>();
    }

    void Update()
    {
        if(walker)
        {
            dest = walker.transform.position;
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                dest += Vector3.right * 5;
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                dest -= Vector3.right * 5;
            }
        }
    }

    void FixedUpdate()
    {
        pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, dest.x, Time.fixedDeltaTime * 3);

        transform.position = pos;
    }
}
