using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CatalogueMenu : Controller
{
    [SerializeField]
    GameObject display_object;
    GameObject[] display_covers;

    BubbleScreen bubble;
    Timeline timeline;

    Catalogue catalogue;

    void Main(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(1);
            break;

            case StateSignal.TICK:
                display_object.transform.Rotate(-Vector3.forward * 11.25f * Time.fixedDeltaTime);

                if
                (
                    Pressed(InputCode.JOURNAL) ||
                    Pressed(InputCode.CANCEL)
                )
                {
                    bubble.Detach();
                }
            break;
        }
    }

    void Awake()
    {
        bubble = GetComponent<BubbleScreen>();
        catalogue = FindObjectOfType<Catalogue>();

        bubble.Attach(Main);

        display_covers = new GameObject[display_object.transform.childCount];
        for(int i = 0; i < display_object.transform.childCount; i++)
        {
            display_covers[i] = display_object.transform.GetChild(i).gameObject;
            display_covers[i].SetActive(!catalogue.GetRune(i));
        }
    }

    void OnDestroy()
    {
        FindObjectOfType<CameraFollow>().Snap();
    }
}
