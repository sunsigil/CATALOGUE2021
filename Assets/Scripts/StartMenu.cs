using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenu : Controller
{
    [SerializeField]
    GameObject lock_object;
    [SerializeField]
    GameObject key_start;
    [SerializeField]
    GameObject key_end;

    LineRenderer shaft_renderer;
    float initial_shaft_width;

    BubbleScreen bubble;
    Timeline timeline;

    void Charging(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(1);
            break;

            case StateSignal.TICK:
                if(Pressed(InputCode.ACTION) || Held(InputCode.ACTION))
                {
                    timeline.Tick(Time.fixedDeltaTime);
                }
                else
                {
                    timeline.Tick(-Time.fixedDeltaTime);
                }

                float progress = NumTools.Hillstep(timeline.progress, -3);
                Vector3 destination = Vector3.Lerp(key_start.transform.position, key_end.transform.position, progress);
                shaft_renderer.SetPosition(0, key_start.transform.position);
                shaft_renderer.SetPosition(1, destination);

                if(timeline.Evaluate())
                {
                    bubble.Chain(Unlocking);
                }
            break;
        }
    }

    void Unlocking(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(1);
            break;

            case StateSignal.TICK:
                timeline.Tick(Time.fixedDeltaTime);

                Vector3 key_position = Vector3.Lerp(key_start.transform.position, key_end.transform.position, timeline.progress);
                key_start.transform.position = key_position;
                shaft_renderer.SetPosition(0, key_position);

                if(timeline.Evaluate())
                {
                    bubble.Detach();
                }
            break;
        }
    }

    void Awake()
    {
        bubble = GetComponent<BubbleScreen>();
        shaft_renderer = key_start.GetComponent<LineRenderer>();

        bubble.Attach(Charging);

        initial_shaft_width = shaft_renderer.widthCurve[0].value;
    }

    void FixedUpdate()
    {
        if(Held(InputCode.CANCEL))
        {
            SceneManager.LoadScene("Display");
        }

        float shaft_width = initial_shaft_width * (transform.localScale.x / 8);
        shaft_renderer.SetWidth(shaft_width, shaft_width);
    }
}
