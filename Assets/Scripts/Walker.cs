using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{
    [SerializeField]
    float speed;

    Camera main_camera;

    Rigidbody2D rigidbody_2d;

    void Awake()
    {
        main_camera = Camera.main;

        rigidbody_2d = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 offset = Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector2 destination = position + offset;

        rigidbody_2d.MovePosition(destination);
    }
}
