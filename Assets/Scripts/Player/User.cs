using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : Controller
{
    [SerializeField]
    GameObject start_menu_prefab;
    [SerializeField]
    GameObject log_menu_prefab;

    CameraFollow camera_follow;

    Satchel satchel;
    Logger logger;
    Walker walker;

    void Awake()
    {
        camera_follow = FindObjectOfType<CameraFollow>();

        satchel = GetComponent<Satchel>();
        logger = GetComponent<Logger>();
        walker = GetComponent<Walker>();

        Commandeer(walker);
    }

    // Start is called before the first frame update
    void Start()
    {
        camera_follow.Snap();
        Instantiate(start_menu_prefab);
    }

    // Update is called once per frame
    void Update()
    {
        if(Pressed(InputCode.CONFIRM))
        {
            float max_grab_dist = 4;
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, max_grab_dist);

            Usable best_usable = null;
            float best_grab_dist = max_grab_dist;

            foreach (Collider2D col in cols)
            {
                Usable usable = col.transform.GetComponent<Usable>();

                if(usable != null)
                {
                    float grab_dist = Mathf.Abs(usable.transform.position.x - transform.position.x);

                    if(grab_dist < best_grab_dist)
                    {
                        best_usable = usable;
                        best_grab_dist = grab_dist;
                    }
                }
            }

            if(best_usable != null)
            {
                best_usable.RequestUse();
            }
        }
        else if(Pressed(InputCode.JOURNAL))
        {
            camera_follow.Snap();
            Instantiate(log_menu_prefab);
        }
    }
}
