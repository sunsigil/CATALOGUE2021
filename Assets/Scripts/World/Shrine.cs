using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]
[RequireComponent(typeof(Machine))]

public class Shrine : MonoBehaviour
{
    RuneGet rune_get_prefab;

    [SerializeField]
    ShrineFlag flag;
    [SerializeField]
    Rune rune;

    Usable usable;
    Machine machine;
    Arena arena;
    Referee referee;

    Logger logger;

    bool primed;
    Timeline timeline;

    bool complete => logger.GetShrine(flag);

    void Use()
    {
        if(complete){ usable.Notify("You have completed this challenge"); }
        else{ primed = true; }
    }

    void Watching(StateSignal signal)
    {
        if(!complete)
        {
            switch(signal)
            {
                case StateSignal.ENTER:
                    usable.show_prompt = true;
                    arena.Spawn();
                    arena.transform.localScale = new Vector3(0, 0, 1);
                break;

                case StateSignal.TICK:
                    arena.transform.localScale = NumTools.XY_Scale(usable.usability);
                    if(primed){ machine.Transition(Active); }
                break;

                case StateSignal.EXIT:
                    usable.show_prompt = false;
                    primed = false;
                break;
            }
        }
        else if(signal == StateSignal.ENTER)
        {
            usable.show_prompt = true;
        }
    }

    void Active(StateSignal signal)
    {
    	switch(signal)
    	{
    		case StateSignal.ENTER:
                AudioWizard._.PushMusic(arena.gameObject, "combat");
                referee.StartCombat(arena);
    		break;

    		case StateSignal.TICK:
                switch(referee.state)
                {
                    case CombatState.COMPLETE:
                        logger.AddShrine(flag);
                        AudioWizard._.PlayEffect("victory");
                        machine.Transition(Cleared);
                    break;

                    case CombatState.DORMANT:
                        machine.Transition(Watching);
                    break;
                }
    		break;

            case StateSignal.EXIT:
                arena.Clear();
                AudioWizard._.PopMusic();
            break;
    	}
    }

    void Cleared(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.ENTER:
                timeline = new Timeline(1);
            break;

            case StateSignal.TICK:
                timeline.Tick(Time.deltaTime);

                float scale = NumTools.Powstep(timeline.progress, 6, true);
                arena.transform.localScale = NumTools.XY_Scale(scale);

                if(timeline.Evaluate())
                {
                    machine.Transition(Watching);
                }
            break;

            case StateSignal.EXIT:
                logger.AddRune(rune.flag);
                rune_get_prefab.rune = rune;
                Instantiate(rune_get_prefab);
            break;
        }
    }

    void Awake()
    {
        rune_get_prefab = Resources.Load<RuneGet>("Rune Get");

        usable = GetComponentInChildren<Usable>();
        machine = GetComponentInChildren<Machine>();
        arena = GetComponentInChildren<Arena>();
        referee = GetComponentInChildren<Referee>();

        logger = FindObjectOfType<Logger>();

        machine.Transition(Watching);
    }

    void Start()
    {
        usable.on_used.AddListener(Use);
    }
}
