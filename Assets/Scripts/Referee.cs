using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    [SerializeField]
    int max_aggroed;

    [SerializeField]
    float turn_duration;

    Shooter shooter;
    Enemy[] enemies;

    float turn_timer;

    /*void SelectAggroes()
    {
        for(int i = 0; i < max_aggroed; i++)
        {
            int index = -1;
            Enemy candidate = null;

            while(candidate && !candidate.aggro)
            {
                index = Random.Range(0, enemies.Count);
                candidate = enemies[index];

                foreach(Enemy enemy in last_aggroed)
                {
                    if(candidate == enemy){already_picked = true;}
                }
            }

            last_aggroed[i] = candidate;
            candidate.aggroed = true;
        }
    }

    public void Bind(Shooter shooter, Enemy[] enemies)
    {
        this.shooter = shooter;
        this.enemies = enemies;

        max_aggroed = Mathf.Clamp(max_aggroed, 0, enemies.Count);
    }*/
}
