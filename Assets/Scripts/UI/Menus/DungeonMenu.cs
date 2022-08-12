using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// BubbleScreen menu which presents
/// a minigame played with the cards specified
/// by the Dungeon provided the
/// player has collected them
/// </summary>
[RequireComponent(typeof(BubbleScreen))]
public class DungeonMenu : Controller
{
    // Prefabs
    RuneGet rune_get_prefab;

    // Plugins
    [SerializeField]
    CardWidget[] card_widgets;

    // Outsiders
    Logger logger;
    SpawnQueue spawn_queue;

    // Components
    BubbleScreen bubble;

    // State
    [SerializeField]
    Dungeon _dungeon;
    public Dungeon dungeon
    {
        get => _dungeon;
        set => _dungeon = value;
    }

    Timeline timeline;

    CardWidget selected_widget;

    /// <summary>
    /// Called when a card is selected:
    /// If it is the only card selected, mark it as such.
    /// If another card is selected, swap that card with this one
    /// and deselect both
    /// </summary>
    /// <param name="widget"></param>
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

        AudioWizard._.PlayEffect("victory");

        logger.AddRune(dungeon.rune.flag);
        rune_get_prefab.rune = dungeon.rune;
        spawn_queue.Add(rune_get_prefab.gameObject);

        bubble.Detach();
    }

    void Main(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.TICK:
                if(Pressed(InputCode.CANCEL) || Pressed(InputCode.CONFIRM))
                {  bubble.Detach(); }
            break;
        }
    }

    void Awake()
    {
        rune_get_prefab = Resources.Load<RuneGet>("Rune Get");

        bubble = GetComponent<BubbleScreen>();
        spawn_queue = FindObjectOfType<SpawnQueue>();
        logger = FindObjectOfType<Logger>();

        bubble.Attach(Main);
        spawn_queue.WaitFor(gameObject);
    }

    void Start()
    {
        // Shuffle the card widgets so they can be rearranged by the player
        int[] scramble = ArrayTools.ShuffleArray(new int[]{0, 1, 2, 3});
        int widget_index = 0;

        for(int i = 0; i < 4; i++)
        {
            int index = scramble[i];
            Card card = _dungeon.cards[index];

            // Only bind and activate cards the player has
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

        AudioWizard._.PushMusic(gameObject, "dungeon");
    }
}
