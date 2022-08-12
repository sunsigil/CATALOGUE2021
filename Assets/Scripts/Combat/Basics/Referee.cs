using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates and maintains the
/// rule context for combat
/// </summary>
public class Referee : MonoBehaviour
{
    // Prefabs
    [SerializeField]
    GameObject player_prefab;
    [SerializeField]
    Enemy[] enemy_prefabs;

    // Settings
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

    // Props
    GameObject player;
    Enemy[] enemies;

    // State
    CombatState _state;
    public CombatState state => _state;

    Timeline sumsick_timeline;

    /// <summary>
    /// Instantiate player inside arena grounds
    /// at set angle and depth.
    /// </summary>
    /// <param name="arena"></param>
    void SpawnPlayer(Arena arena)
    {
        if(player != null){ Destroy(player); }

        player = arena.Add(player_prefab, player_angle, player_depth);
    }

    /// <summary>
    /// Instantiate enemies inside arena grounds.
    /// Enemy positions are randomized but constrained
    /// such that they fall outside of set safe zone.
    /// </summary>
    /// <param name="arena"></param>
    void SpawnEnemies(Arena arena)
    {
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

    /// <summary>
    /// Spawn players and enemies and initialize
    /// combat state.
    /// </summary>
    /// <param name="arena"></param>
    public void StartCombat(Arena arena)
    {
        SpawnPlayer(arena);
        SpawnEnemies(arena);

        _state = CombatState.ONGOING;
        sumsick_timeline = new Timeline(0.75f);
    }

    void Update()
    {
        // Exit if combat is not happening
        // or if summoning sickness phase has not ended
        if(_state != CombatState.ONGOING){ return; }
        if(!sumsick_timeline.Evaluate())
        {
            sumsick_timeline.Tick(Time.deltaTime);
            return;
        }

        // Count alive and aggroed enemies
        int alive_ct = 0;
        int aggroed_ct = 0;

        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null)
            {
                alive_ct++;
                if(enemies[i].IsAggroed()){ aggroed_ct++; }
            }
        }

        // If all enemies are dead, set state to win or loss depending on player's status and exit
        if(alive_ct == 0){ _state = player == null ? CombatState.DORMANT : CombatState.COMPLETE; return; }
        // Exit if there is no aggroing to be done
        if(aggroed_ct == max_aggroed || aggroed_ct == alive_ct){ return; }

        // Select a number of enemies to be aggroed
        // according to the max aggro setting
        // or the number of living enemies,
        // whichever is smaller
        int[] aggro_indices = new int[Mathf.Min(max_aggroed, alive_ct)];

        for(int i = 0; i < aggro_indices.Length; i++)
        {
            int random_index = -1;
            bool index_valid = false;

            // Only select indices which correspond to living enemies
            // that have not been selected this turn
            do {
                random_index = Random.Range(0, enemies.Length);
                index_valid = true;

                for(int j = 0; j < i; j++)
                {
                    if(random_index == aggro_indices[j] || enemies[random_index] == null)
                    {
                        index_valid = false;
                        break;
                    }
                }
            } while(!index_valid);

            enemies[random_index].Aggro();
            aggro_indices[i] = random_index;
        }
    }
}
