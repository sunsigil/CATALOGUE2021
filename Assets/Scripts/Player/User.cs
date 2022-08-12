using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The player's commander controller,
/// which also governs
/// in-world interaction and menu
/// opening
/// </summary>
public class User : Controller
{
    // Prefabs
    [SerializeField]
    GameObject start_menu_prefab;
    [SerializeField]
    GameObject log_menu_prefab;

    // Outsiders
    CameraFollow camera_follow;
    AudioWizard audio_wizard;

    // Components
    Walker walker;

    void Awake()
    {
        camera_follow = FindObjectOfType<CameraFollow>();
        audio_wizard = FindObjectOfType<AudioWizard>();

        walker = GetComponent<Walker>();

        Commandeer(walker);
    }

    // Start is called before the first frame update
    void Start()
    {
        audio_wizard.PushMusic(gameObject, "ambience 1");

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

            // Select nearest usable
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
            { best_usable.RequestUse(); }
        }

        if(Pressed(InputCode.JOURNAL))
        { Instantiate(log_menu_prefab); }

        if(Held(InputCode.CANCEL))
        { SceneManager.LoadScene("Island"); }
    }
}
