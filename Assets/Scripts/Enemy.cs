using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
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

    Vector3 original_scale;
    float original_line_width;

    Shooter shooter;
    Vector3 target_position;
    bool primed;
    int lives;

    float attack_period = 2;
    float firing_duration = 0.5f;
    float targeting_duration => attack_period - firing_duration;
    float timer;

    void UpdateBeam()
    {
        if(!aggro)
        {
            line_renderer.enabled = false;
            return;
        }

        if(timer < targeting_duration)
        {
            line_renderer.enabled = false;
        }
        else if(timer < attack_period)
        {
            float time_to_fire = timer - targeting_duration;
            float firing_progress = time_to_fire / firing_duration;

            float beam_width = original_line_width;
            Vector3 beam_end = Vector3.Lerp(transform.position, target_position, firing_progress);

            line_renderer.enabled = true;
            line_renderer.SetWidth(beam_width, beam_width);
            line_renderer.SetPosition(0, transform.position);
            line_renderer.SetPosition(1, beam_end);
        }
    }

    void UpdateBody()
    {
        if(!aggro)
        {
            transform.localScale = original_scale;
            return;
        }

        if(timer < targeting_duration)
        {
            transform.localScale = original_scale;

            Vector3 target_line = target_position - transform.position;
            float target_angle = Mathf.Atan2(target_line.y, target_line.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 + target_angle));
        }
        else if(timer < attack_period)
        {
            float time_to_fire = timer - targeting_duration;
            float firing_progress = time_to_fire / firing_duration;

            // "Flash" scaling function:
            // Form of f(x) = Ax * sin(Bx - C) + D
            // Domain  and range ~= [0, 1], [0, 0.25]
            float A = 0.14f;
            float B = -17.6f;
            float C = -0.2f;
            float D = 0.12f;
            float scale = A * firing_progress * Mathf.Sin(B * firing_progress - C) + D;
            transform.localScale = original_scale + new Vector3(scale, scale, 0);
        }
    }

    public void RecordSpawnValues()
    {
        original_scale = transform.localScale;
        original_line_width = line_renderer.widthCurve[0].value * original_scale.x;
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

        UpdateBody();
        UpdateBeam();

        if(aggro)
        {
            if(primed)
            {
                print("Miss");
                primed = false;
                Debug.Break();
            }

            timer += Time.deltaTime;

            if(timer < targeting_duration)
            {
                target_position = shooter.transform.position;
            }
            else if(timer >= attack_period)
            {
                primed = true;
                rigidbody.MovePosition(target_position);

                _aggro = false;
                timer = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.transform == shooter.transform)
        {
            if(primed)
            {
                print("Hit!");
                shooter.ProcessHit();
                primed = false;
            }
            else
            {
                print("Hurt...");
                Destroy(gameObject);
            }
        }
    }
}
