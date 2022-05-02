using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    float smoothing;
    float normal_size;

    Camera camera;

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

    public void Zoom(float size, float t)
    {
        camera.orthographicSize = Mathf.Lerp(normal_size, size, NumTools.Perlinstep(t));
    }

    void Awake()
    {
        camera = GetComponent<Camera>();
        normal_size = camera.orthographicSize;

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
        pos.y = camera.orthographicSize - 0.5f;

        transform.position = pos;
    }
}
