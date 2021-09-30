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

    Shooter shooter;

    int lives;

    float seek_timer;

    public void ProcessHit()
    {
        lives--;

        if(lives == 0){Destroy(gameObject);}
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        shooter = FindObjectOfType<Shooter>();
    }

    void Update()
    {
        if(!shooter){return;}
        if(!_aggro){return;}

        rigidbody.MovePosition(shooter.transform.position);

        _aggro = false;
    }
}
