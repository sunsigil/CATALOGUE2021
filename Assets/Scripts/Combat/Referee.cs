using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    [SerializeField]
    GameObject player_prefab;
    [SerializeField]
    Enemy[] enemy_prefabs;

    [Range(0, 6.2831f)]
    float player_angle;
    [SerializeField]
    [Range(0, 1f)]
    float player_depth;

    [SerializeField]
    int max_aggroed;
    [Range(0, 1.5707f)]
    float safe_arc;

    GameObject player;
    Enemy[] enemies;

    bool in_combat;

    void SpawnPlayer(Arena arena)
    {
        if(player != null){ Destroy(player); }

        player = arena.Add(player_prefab, player_angle, player_depth);
    }

    void SpawnEnemies(Arena arena)
    {
        // Address possibility of unitinitialized enemy array
        if(enemies == null){enemies = new Enemy[enemy_prefabs.Length];}

        /*for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null){Destroy(enemies[i].gameObject);}

            float arc = (((2 * Mathf.PI) - (2 * safe_arc))) / enemies.Length;
            float angle = player_angle + safe_arc + arc * (i + 0.5f);
            float depth = 0.5f + Random.Range(-0.1f, 0.1f);

            enemies[i] = arena.Add(enemy_prefabs[i].gameObject, angle, depth).GetComponent<Enemy>();
        }*/

        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                int idx = 10 * i + j;

                if(enemies[idx] != null){Destroy(enemies[idx].gameObject);}

                Vector3 pos = 0.5f * (new Vector3(i, j, 0) - new Vector3(5, 5, 0));
                enemies[idx] = arena.Add(enemy_prefabs[idx].gameObject, pos).GetComponent<Enemy>();
            }
        }
    }

    public void StartCombat(Arena arena)
    {
        SpawnPlayer(arena);
        SpawnEnemies(arena);

        in_combat = true;
    }

    public int EvaluateCombat()
    {
        if(!in_combat){return 0;}

        foreach(Enemy enemy in enemies)
        {
            if(enemy != null)
            {
                return 0;
            }
        }

        in_combat = false;

        if(player == null)
        { return -1; }
        else
        { return 1; }
    }

    void Update()
    {
        if(!in_combat){ return; }

        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null && enemies[i].IsAggroed())
            {
                return;
            }
        }

        int[] aggro_indices = new int[max_aggroed];

        for(int i = 0; i < max_aggroed; i++)
        {
            int random_index = -1;
            bool already_picked = false;

            do {
                random_index = Random.Range(0, enemies.Length);
                already_picked = false;

                for(int j = 0; j < i; j++)
                {
                    if(aggro_indices[j] == random_index)
                    {
                        already_picked = true;
                        break;
                    }
                }
            } while(already_picked);

            enemies[random_index].Aggro();
            aggro_indices[i] = random_index;
        }
    }
}
