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
    Timeline exit_timeline;

    void Charging(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(1);
                exit_timeline = new Timeline(1);
            break;

            case StateSignal.TICK:
                if(Pressed(InputCode.ACTION) || Held(InputCode.ACTION))
                {
                    timeline.Tick(Time.deltaTime);
                }
                else if(Pressed(InputCode.CANCEL) || Held(InputCode.CANCEL))
                {
                    exit_timeline.Tick(Time.deltaTime);
                }
                else
                {
                    timeline.Tick(-Time.deltaTime);
                    exit_timeline.Tick(-Time.deltaTime * 2);
                }

                float progress = NumTools.Hillstep(timeline.progress, -3);
                Vector3 destination = Vector3.Lerp(key_start.transform.position, key_end.transform.position, progress);
                shaft_renderer.SetPosition(0, key_start.transform.position);
                shaft_renderer.SetPosition(1, destination);

                key_start.transform.localScale = Vector3.Lerp(NumTools.XY_Scale(1), NumTools.XY_Scale(0), exit_timeline.progress);

                if(timeline.Evaluate())
                {
                    AudioWizard._.PlayEffect("start");
                    bubble.Chain(Unlocking);
                }
                else if(exit_timeline.Evaluate())
                {
                    Application.Quit();
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
                timeline.Tick(Time.deltaTime);

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

        initial_shaft_width = shaft_renderer.widthCurve[0].value;

        bubble.Attach(Charging);
    }

    void Start()
    {
        AudioWizard._.PushMusic(gameObject, "start menu");
    }

    void FixedUpdate()
    {
        float shaft_width = initial_shaft_width * (transform.localScale.x / 8);
        shaft_renderer.SetWidth(shaft_width, shaft_width);
    }
}
