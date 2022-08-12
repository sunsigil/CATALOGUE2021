using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneGet : Controller
{
    [SerializeField]
    RuneWidget rune_widget;

    [SerializeField]
    Rune _rune;
    public Rune rune
    {
        get => _rune;
        set => _rune = value;
    }

    BubbleScreen bubble;
    Timeline timeline;

    float scale_i;
    float scale_f;
    float theta_i;
    float theta_f;

    void Intro(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(0.75f);
            break;

            case StateSignal.TICK:
                timeline.Tick(Time.deltaTime);

                rune_widget.transform.localScale = NumTools.XY_Scale(Mathf.Lerp(scale_i, scale_f, timeline.progress));
                rune_widget.transform.rotation = NumTools.XY_Quat(Mathf.Lerp(theta_i, theta_f, timeline.progress));

                if(timeline.Evaluate())
                {
                    AudioWizard._.PlayEffect("rune get");
                    bubble.Chain(Main);
                }
            break;
        }
    }

    void Main(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.TICK:
                if(Pressed(InputCode.CONFIRM) || Pressed(InputCode.CANCEL))
                { bubble.Detach(); }
            break;
        }
    }

    void Awake()
    {
        bubble = GetComponent<BubbleScreen>();

        scale_f = rune_widget.transform.localScale.x;
        scale_i = scale_f * 0.05f;
        theta_i = Random.Range(0, 2 * Mathf.PI);
        theta_f = 4 * (2 * Mathf.PI) + Random.Range(-Mathf.PI / 6, Mathf.PI / 6);

        rune_widget.transform.localScale = NumTools.XY_Scale(scale_i);
        rune_widget.transform.rotation = NumTools.XY_Quat(theta_i);
        rune_widget.Bind(_rune);

        bubble.Attach(Intro);
    }
}
