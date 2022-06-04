using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : Controller
{
    PopupBar popup_bar_prefab;

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
        popup_bar_prefab = Resources.Load<PopupBar>("Popup Bar");

        camera_follow = FindObjectOfType<CameraFollow>();

        satchel = GetComponent<Satchel>();
        logger = GetComponent<Logger>();
        walker = GetComponent<Walker>();

        Commandeer(walker);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(start_menu_prefab);
        camera_follow.Snap();

        PopupBar popup_bar = AssetTools.SpawnComponent(popup_bar_prefab);
        popup_bar.message = "Game Start!";
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
            Instantiate(log_menu_prefab);
        }

        if(Input.GetKeyDown(KeyCode.Minus))
        {
            logger.Save();
            Application.Quit();
        }
    }
}
