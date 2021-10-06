using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    [SerializeField]
    [Range(0, 0.5f)]
    float combatant_scale_factor;

    [SerializeField]
    [Range(0, 1f)]
    float shooter_spawn_depth;
    [SerializeField]
    [Range(0, 6.2831f)]
    float shooter_spawn_angle;
    [SerializeField]
    [Range(0, 1.5707f)]
    float spawn_safe_zone;
    [SerializeField]
    GameObject shooter_prefab;

    [SerializeField]
    int enemy_count;
    [SerializeField]
    int max_aggroed;
    [SerializeField]
    GameObject enemy_prefab;
    [SerializeField]
    [Range(0, 0.5f)]
    float enemy_depth_mod_range;

    GameObject roster;
    Shooter shooter;
    Enemy[] enemies;

    bool _in_combat;
    public bool in_combat => _in_combat;

    void SpawnShooter(Vector3 arena_center, float arena_radius)
    {
        // Clean up existing shooter
        if(shooter != null){Destroy(shooter.gameObject);}

        // Spawn, group, record
        GameObject shooter_object = Instantiate(shooter_prefab);
        shooter = shooter_object.GetComponent<Shooter>();
        shooter.transform.SetParent(roster.transform);


        // Size character relative to the arena
        float diameter = arena_radius * 2 * combatant_scale_factor;
        shooter.transform.localScale = new Vector3(diameter, diameter, 1);
        shooter.SetLimits(arena_center, arena_radius);

        // Position the character along a radial line according to angle value and depth value
        Vector3 offset = new Vector3(Mathf.Cos(shooter_spawn_angle), Mathf.Sin(shooter_spawn_angle)) * arena_radius * shooter_spawn_depth;
        shooter.transform.position = arena_center + offset;
    }

    void SpawnEnemies(Vector3 arena_center, float arena_radius)
    {
        // Address possibility of unitinitialized enemy array
        if(enemies == null){enemies = new Enemy[enemy_count];}

        for(int i = 0; i < enemy_count; i++)
        {
            // Clean up existing enemies
            if(enemies[i] != null){Destroy(enemies[i].gameObject);}

            // Spawn, group, record
            GameObject enemy_object = Instantiate(enemy_prefab);
            enemies[i] = enemy_object.GetComponent<Enemy>();
            enemies[i].transform.SetParent(roster.transform);

            // Size character relative to the arena
            float diameter = arena_radius * 2 * combatant_scale_factor;
            enemies[i].transform.localScale = new Vector3(diameter, diameter, 1);


            // Position the character along a radial line according to slice arc and depth value
            // theta is offset by one half slice size to put each character in the middle of a slice
            float arc = (((2 * Mathf.PI) - (2 * spawn_safe_zone))) / enemy_count;
            float theta = shooter_spawn_angle + spawn_safe_zone + arc * (i + 0.5f);
            float depth = 0.5f + Random.Range(-enemy_depth_mod_range, enemy_depth_mod_range);
            Vector3 offset = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta)) * arena_radius * depth;
            enemies[i].transform.position = arena_center + offset;
        }
    }

    public void StartCombat(Vector3 arena_center, float arena_radius)
    {
        if(roster != null){Destroy(roster);}

        roster = new GameObject("Roster");
        roster.transform.SetParent(transform);

        SpawnShooter(arena_center, arena_radius);
        SpawnEnemies(arena_center, arena_radius);

        _in_combat = true;
    }

    public int AssessCombat()
    {
        if(!_in_combat){return 0;}

        if(!shooter){return -1;}

        foreach(Enemy enemy in enemies)
        {
            if(enemy != null)
            {
                return 0;
            }
        }

        return 1;
    }

    public void EndCombat()
    {
        if(roster != null){Destroy(roster);}

        _in_combat = false;
    }

    void Update()
    {
        if(!_in_combat){return;}

        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null && enemies[i].aggro)
            {
                // Not yet time to pick another round of attackers
                return;
            }
        }

        int[] aggro_indices = new int[max_aggroed];

        for(int i = 0; i < max_aggroed; i++)
        {
            int random_index = -1;
            bool already_picked = true;

            while(already_picked)
            {
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
            }

            enemies[random_index].aggro = true;
            aggro_indices[i] = random_index;
        }
    }
}
