using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]
[RequireComponent(typeof(Obscurable))]

public class Shrine : MonoBehaviour
{
    enum ShrineState
    {
        DORMANT,
        WATCHING,
        ACTIVE,
        SHUTDOWN
    }
    ShrineState state;

    [SerializeField]
    GameObject arena_prefab;

    [SerializeField]
    ShrineFlag flag;

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
    float rotation_speed;

    [SerializeField]
    float shutdown_duration;
    float shutdown_timer;

    Usable usable;
    Obscurable obscurable;
    Referee referee;

    GameObject arena;
    GameObject wreath;

    Logger logger;

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

    void Cleanup()
    {
        Destroy(arena);
        Destroy(wreath);
    }

    void Use()
    {
        if(state == ShrineState.DORMANT){return;}
        if(referee.in_combat){return;}

        referee.StartCombat(arena.transform.position, arena_radius);

        state = ShrineState.ACTIVE;
    }

    void Awake()
    {
        usable = GetComponent<Usable>();
        obscurable = GetComponent<Obscurable>();
        referee = GetComponent<Referee>();

        logger = FindObjectOfType<Logger>();
    }

    void Start()
    {
        usable.on_used.AddListener(Use);

        if(logger.GetShrine(flag))
        {
            state = ShrineState.DORMANT;
        }
        else
        {
            GenerateBasics();
            GenerateColliders();
            GenerateOrnaments();

            state = ShrineState.WATCHING;
        }
    }

    void Update()
    {
        if(state == ShrineState.WATCHING)
        {
            if(usable.usability >= 1)
            {
                InputPrompter._.Request(InputCode.CONFIRM, transform.position);
            }
        }
        else if(state == ShrineState.ACTIVE)
        {
            // Assess win condition
            if(referee.AssessCombat() != 0)
            {
                if(referee.AssessCombat() == 1)
                {
                    logger.AddShrine(flag);

                    state = ShrineState.SHUTDOWN;
                }
                else
                {
                    state = ShrineState.WATCHING;
                }

                referee.EndCombat();
            }
        }
    }

    void FixedUpdate()
    {
        if(state == ShrineState.WATCHING)
        {
            transform.localScale = NumTools.XY_Scale(usable.usability);
        }
        else if(state == ShrineState.ACTIVE)
        {
            wreath.transform.Rotate(NumTools.PinwheelVelocity(rotation_speed) * Time.fixedDeltaTime);
        }
        else if(state == ShrineState.SHUTDOWN)
        {
            float progress = shutdown_timer / shutdown_duration;
            float scalar = 1-Mathf.Pow(progress, 6);

            if(progress >= 1){Destroy(gameObject);}

            transform.localScale = NumTools.XY_Scale(scalar);

            shutdown_timer += Time.fixedDeltaTime;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arena_radius);
        Gizmos.DrawWireSphere(transform.position, wreath_radius);
    }
}
