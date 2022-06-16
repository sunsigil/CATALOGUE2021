using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardGet : Controller
{
    [SerializeField]
    CardWidget card_widget;

    [SerializeField]
    Card _card;
    public Card card
    {
        get => _card;
        set => _card = value;
    }

    BubbleScreen bubble;
    Timeline timeline;

    Logger logger;

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
                timeline.Tick(Time.fixedDeltaTime);

                card_widget.transform.localScale = NumTools.XY_Scale(Mathf.Lerp(scale_i, scale_f, timeline.progress));
                card_widget.transform.rotation = NumTools.XY_Quat(Mathf.Lerp(theta_i, theta_f, timeline.progress));

                if(timeline.Evaluate())
                {
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
                if
                (
                    Pressed(InputCode.CONFIRM) ||
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
        logger = FindObjectOfType<Logger>();

        scale_f = card_widget.transform.localScale.x;
        scale_i = scale_f * 0.05f;
        theta_i = Random.Range(0, 2 * Mathf.PI);
        theta_f = 4 * (2 * Mathf.PI) + Random.Range(-Mathf.PI / 6, Mathf.PI / 6);

        card_widget.transform.localScale = NumTools.XY_Scale(scale_i);
        card_widget.transform.rotation = NumTools.XY_Quat(theta_i);
        card_widget.Bind(_card);

        bubble.Attach(Intro);
    }

    void OnDestroy()
    {
        FindObjectOfType<CameraFollow>().Snap();
    }
}
