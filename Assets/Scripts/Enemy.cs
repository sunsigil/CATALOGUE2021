using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IHittable
{
    [SerializeField]
    int max_lives;

    bool _aggro;
    public bool aggro
    {
        get => _aggro;
        set => _aggro = value;
    }

    Rigidbody2D rigidbody;
    LineRenderer line_renderer;

    Shooter shooter;
    Vector3 target_position;

    int lives;

    float cooldown = 2;
    float lock_time = 0.5f;
    float timer;

    public void ProcessHit()
    {
        lives--;

        if(lives == 0){Destroy(gameObject);}
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        line_renderer = GetComponent<LineRenderer>();

        shooter = FindObjectOfType<Shooter>();
    }

    void Update()
    {
        if(!shooter){return;}

        if(aggro)
        {
            timer += Time.deltaTime;

            float targeting_cutoff = cooldown - lock_time;

            if(timer < targeting_cutoff)
            {
                target_position = shooter.transform.position;

                Vector3 target_line = shooter.transform.position - transform.position;
                float target_angle = Mathf.Atan2(target_line.y, target_line.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 + target_angle));
            }
            else if(timer >= targeting_cutoff && timer < cooldown)
            {
                line_renderer.enabled = true;
                line_renderer.SetPosition(0, transform.position);
                line_renderer.SetPosition(1, target_position);
            }
            else
            {
                rigidbody.MovePosition(target_position);
                line_renderer.enabled = false;

                _aggro = false;
                timer = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform == shooter.transform){Destroy(gameObject);}
    }
}
