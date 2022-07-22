using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    [SerializeField]
    GameObject player_prefab;
    [SerializeField]
    Enemy[] enemy_prefabs;

    [SerializeField]
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

        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null){Destroy(enemies[i].gameObject);}

            float arc = (((2 * Mathf.PI) - (2 * safe_arc))) / enemies.Length;
            float angle = player_angle + safe_arc + arc * (i + 0.5f);
            float depth = 0.5f + Random.Range(-0.1f, 0.1f);

            enemies[i] = arena.Add(enemy_prefabs[i].gameObject, angle, depth).GetComponent<Enemy>();
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

        int alive = 0;
        int aggroed = 0;
        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null)
            {
                alive++;

                if(enemies[i].IsAggroed()){ aggroed++; }
            }

        }
        if(aggroed == max_aggroed || aggroed == alive){ return; }

        int[] aggro_indices = new int[Mathf.Min(max_aggroed, alive)];

        for(int i = 0; i < aggro_indices.Length; i++)
        {
            int random_index = -1;
            bool invalid_index = false;

            do {
                random_index = Random.Range(0, enemies.Length);
                invalid_index = false;

                for(int j = 0; j < i; j++)
                {
                    if(random_index == aggro_indices[j] || enemies[random_index] == null)
                    {
                        invalid_index = true;
                        break;
                    }
                }
            } while(invalid_index);

            if(random_index != -1)
            {
                enemies[random_index].Aggro();
                aggro_indices[i] = random_index;
            }
        }
    }
}
