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
    int collider_count;
    [SerializeField]
    GameObject collider_prefab;

    [SerializeField]
    int ornament_count;
    [SerializeField]
    GameObject[] ornament_prefabs;

    [SerializeField]
    [Range(0, 1)]
    float inner_dist_bound;
    [SerializeField]
    [Range(2, 10)]
    float outer_dist_bound;

    [SerializeField]
    float rotation_speed;

    [SerializeField]
    float regen_cooldown;

    Referee referee;

    GameObject arena;
    GameObject wreath;

    Walker walker;
    float walker_dist;

    float regen_timer;

    void GenerateBasics()
    {
        arena = Instantiate(arena_prefab);
        arena.transform.SetParent(transform);
        arena.transform.localPosition = Vector3.zero;

        // Set the arena's scale to its radius. Note that this scaling op, and all after,
        // assume that prefabs begin with a scale of (1,1,1)
        float diameter = arena_radius * 2;
        arena.transform.localScale = new Vector3(diameter, diameter, 1);

        // The wreath is a dummy object which groups ornaments
        wreath = new GameObject("Wreath");
        wreath.transform.SetParent(transform);
        wreath.transform.localPosition = Vector3.zero;
    }

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
            // Clean  up existing ornaments
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

    void Awake()
    {
        referee = GetComponent<Referee>();

        walker = FindObjectOfType<Walker>();

        GenerateBasics();
        GenerateColliders();
        GenerateOrnaments();
    }

    void Update()
    {
        if(walker_dist > inner_dist_bound){return;}

        if(!referee.in_combat)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                referee.StartCombat(arena.transform.position, arena_radius);
            }
        }
        else
        {
            // Assess win condition
            if(referee.AssessCombat() != 0)
            {
                referee.EndCombat();
            }

            // Regenerate wreath
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
    }

    void FixedUpdate()
    {
        walker_dist = Mathf.Abs(transform.position.x - walker.transform.position.x);
        float dist_range = outer_dist_bound - inner_dist_bound;
        float adjusted_dist = Mathf.Clamp(walker_dist - inner_dist_bound, 0, dist_range);
        float progress = 1-(adjusted_dist / dist_range);

        if(progress > 0)
        {
            Debug.DrawLine(walker.transform.position, new Vector3(transform.position.x, walker.transform.position.y, 0), Color.red);
            Debug.DrawLine(new Vector3(transform.position.x, walker.transform.position.y, 0), arena.transform.position, Color.red);
            Debug.DrawLine(walker.transform.position, arena.transform.position, Color.red);
        }

        transform.localScale = new Vector3(progress, progress, 1);
        wreath.transform.Rotate(Vector3.forward * Mathf.PI * rotation_speed * Mathf.Rad2Deg * Time.fixedDeltaTime * progress);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arena_radius);
        Gizmos.DrawWireSphere(transform.position, wreath_radius);
    }
}