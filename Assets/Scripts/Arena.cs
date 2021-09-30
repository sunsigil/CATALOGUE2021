using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField]
    GameObject arena_prefab;

    [SerializeField]
    float arena_radius;
    [SerializeField]
    float wreath_radius;

    [SerializeField]
    [Range(0, 0.5f)]
    float character_scale_factor;

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
    GameObject enemy_prefab;
    [SerializeField]
    [Range(0, 0.5f)]
    float enemy_depth_mod_range;

    [SerializeField]
    int collider_count;
    [SerializeField]
    GameObject collider_prefab;

    [SerializeField]
    int ornament_count;
    [SerializeField]
    GameObject[] ornament_prefabs;

    [SerializeField]
    float rotation_speed;

    [SerializeField]
    float regen_cooldown;

    GameObject arena;
    GameObject wreath;

    float regen_timer;

    void GenerateColliders()
    {
        // Divide arena into collider_count slices
        float collider_arc = 2 * Mathf.PI / collider_count;

        // Determine linear width of each slice
        // SECANT LENGTH FORMULA: 2r * sin(theta / 2)
        float collider_width = 2 * arena_radius * Mathf.Sin(collider_arc / 2);

        for(int i = 0; i < collider_count; i++)
        {
            // If colliders are already present, destroy them
            if(arena.transform.childCount > i)
            {
                Destroy(arena.transform.GetChild(i).gameObject);
            }

            // Resize the collider such that it will fit within its allotted slice
            GameObject collider_object = Instantiate(collider_prefab);
            collider_object.transform.SetParent(arena.transform);
            BoxCollider2D collider = collider_object.GetComponent<BoxCollider2D>();
            Vector3 original_size = collider.size;
            collider.size = new Vector3(collider_width * 1.1f, original_size.y, original_size.z);


            // Position the ornament on the wreath, which surrounds the arena
            float theta = collider_arc * i;
            Vector3 offset = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * arena_radius;
            Vector3 position = arena.transform.position + offset;
            collider_object.transform.position = position;

            // Rotate the ornament to face away from the center of the arena
            collider_object.transform.rotation = Quaternion.Euler(new Vector3(0, 0, theta * Mathf.Rad2Deg - 90));
        }
    }

    void GenerateOrnaments()
    {
        // Divide wreath into ornament_count slices
        float ornament_arc = 2 * Mathf.PI / ornament_count;

        // Determine linear width of each slice
        // SECANT LENGTH FORMULA: 2r * sin(theta / 2)
        float ornament_width = 2 * wreath_radius * Mathf.Sin(ornament_arc / 2);

        for(int i = 0; i < ornament_count; i++)
        {
            // If ornaments are already present, destroy them
            if(wreath.transform.childCount > i)
            {
                Destroy(wreath.transform.GetChild(i).gameObject);
            }

            int prefab_index = Random.Range(0, ornament_prefabs.Length);
            GameObject prefab = ornament_prefabs[prefab_index];

            // Scale the ornament such that it will fit within its allotted slice
            GameObject ornament = Instantiate(prefab);
            ornament.transform.SetParent(wreath.transform);
            ornament.transform.localScale = new Vector3(ornament_width, ornament_width, 1);

            // Position the ornament on the wreath, which surrounds the arena
            float theta = (wreath.transform.rotation.eulerAngles.z * Mathf.Deg2Rad) + ornament_arc * i;
            Vector3 offset = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * wreath_radius;
            Vector3 position = arena.transform.position + offset;
            ornament.transform.position = position;

            // Rotate the ornament to face away from the center of the arena
            ornament.transform.rotation = Quaternion.Euler(new Vector3(0, 0, theta * Mathf.Rad2Deg - 90));
        }
    }

    void SpawnShooter()
    {
        // Size character relative to the arena
        GameObject shooter = Instantiate(shooter_prefab);
        float diameter = arena_radius * 2 * character_scale_factor;
        shooter.transform.localScale = new Vector3(diameter, diameter, 1);
        shooter.GetComponent<Shooter>().SetLimits(arena.transform.position, arena_radius);

        // Position the character along a radial line according to angle value and depth value
        Vector3 offset = new Vector3(Mathf.Cos(shooter_spawn_angle), Mathf.Sin(shooter_spawn_angle)) * arena_radius * shooter_spawn_depth;
        shooter.transform.position = arena.transform.position + offset;
    }

    void SpawnEnemies()
    {
        for(int i = 0; i < enemy_count; i++)
        {
            // Size character relative to the arena
            GameObject enemy = Instantiate(enemy_prefab);
            float diameter = arena_radius * 2 * character_scale_factor;
            enemy.transform.localScale = new Vector3(diameter, diameter, 1);

            // Position the character along a radial line according to slice arc and depth value
            // theta is offset by one half slice size to put each character in the middle of a slice
            float arc = (((2 * Mathf.PI) - (2 * spawn_safe_zone))) / enemy_count;
            float theta = shooter_spawn_angle + spawn_safe_zone + arc * (i + 0.5f);
            float depth = 0.5f + Random.Range(-enemy_depth_mod_range, enemy_depth_mod_range);
            Vector3 offset = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta)) * arena_radius * depth;
            enemy.transform.position = arena.transform.position + offset;
        }
    }

    void Awake()
    {
        // Set the arena's scale to its radius. Note that this scaling op, and all after,
        // assume that prefabs begin with a scale of (1,1,1)
        arena = Instantiate(arena_prefab);
        float diameter = arena_radius * 2;
        arena.transform.localScale = new Vector3(diameter, diameter, 1);

        // The wreath is a dummy object which serves to group ornaments
        wreath = new GameObject("Wreath");

        GenerateColliders();
        GenerateOrnaments();
        SpawnShooter();
        SpawnEnemies();
    }

    void Update()
    {
        if(regen_cooldown > 0)
        {
            if(regen_timer >= regen_cooldown)
            {
                GenerateOrnaments();

                regen_timer = 0;
            }

            regen_timer += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        wreath.transform.Rotate(Vector3.forward * Mathf.PI * rotation_speed * Mathf.Rad2Deg * Time.fixedDeltaTime);
    }
}
