using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DungeonMenu : Controller
{
    PopupBar popup_bar_prefab;
    RuneGet rune_get_prefab;

    [SerializeField]
    CardWidget[] card_widgets;

    [SerializeField]
    Dungeon _dungeon;
    public Dungeon dungeon
    {
        get => _dungeon;
        set => _dungeon = value;
    }

    BubbleScreen bubble;
    Timeline timeline;

    Logger logger;
    SpawnQueue spawn_queue;

    CardWidget selected_widget;

    public void OnSelect(CardWidget widget)
    {
        if(selected_widget == null)
        {
            selected_widget = widget;
            selected_widget.ToggleMark(true);

            return;
        }

        int a_index = ArrayTools.Find(card_widgets, selected_widget);
        int b_index = ArrayTools.Find(card_widgets, widget);
        ArrayTools.Swap(card_widgets, a_index, b_index);

        NumTools.BoogieWoogie(selected_widget.transform, widget.transform);

        selected_widget.ToggleMark(false);
        selected_widget = null;

        for(int i = 0; i < card_widgets.Length; i++)
        {
            if(card_widgets[i].card != dungeon.cards[i])
            {
                return;
            }
        }

        rune_get_prefab.rune = dungeon.rune;
        spawn_queue.Add(rune_get_prefab.gameObject);

        AudioWizard._.PlayEffect("victory");
        bubble.Detach();
    }

    void Main(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(1);
            break;

            case StateSignal.TICK:
                if(Pressed(InputCode.CANCEL))
                {
                    bubble.Detach();
                }
            break;
        }
    }

    void Awake()
    {
        popup_bar_prefab = Resources.Load<PopupBar>("Popup Bar");
        rune_get_prefab = Resources.Load<RuneGet>("Rune Get");

        bubble = GetComponent<BubbleScreen>();
        spawn_queue = FindObjectOfType<SpawnQueue>();
        logger = FindObjectOfType<Logger>();

        bubble.Attach(Main);
        spawn_queue.WaitFor(gameObject);
    }

    void Start()
    {
        int[] scramble = ArrayTools.ShuffleArray(new int[]{0, 1, 2, 3});
        int widget_index = 0;

        for(int i = 0; i < 4; i++)
        {
            int index = scramble[i];
            Card card = _dungeon.cards[index];

            if(logger.GetCard(card.flag))
            {
                card_widgets[widget_index].Bind(card);
                widget_index++;
            }
        }
        for(int i = widget_index; i < 4; i++)
        {
            card_widgets[i].gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        FindObjectOfType<CameraFollow>().Snap();
    }
}
