using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// BubbleScreen menu which
/// displays the player's
/// rune collection
/// </summary>
[RequireComponent(typeof(BubbleScreen))]
public class LogMenu : Controller
{
    // Plugins
    [SerializeField]
    GameObject display_object;
    
    // Outsiders
    Logger logger;

    // Components
    BubbleScreen bubble;

    // State
    GameObject[] display_covers;

    void Main(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.TICK:
                display_object.transform.Rotate(-Vector3.forward * 11.25f * Time.deltaTime);

                if(Pressed(InputCode.JOURNAL) || Pressed(InputCode.CANCEL))
                { bubble.Detach(); }
            break;
        }
    }

    void Awake()
    {
        bubble = GetComponent<BubbleScreen>();
        logger = FindObjectOfType<Logger>();

        // Disable the sprite covering the rune marks according
        // to whether or not the player has unlocked the corresponding runes
        display_covers = new GameObject[display_object.transform.childCount];
        for(int i = 0; i < display_object.transform.childCount; i++)
        {
            display_covers[i] = display_object.transform.GetChild(i).gameObject;
            display_covers[i].SetActive(!logger.GetRune(i));
        }

        bubble.Attach(Main);
    }
}
