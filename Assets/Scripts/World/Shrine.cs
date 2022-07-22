using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]
[RequireComponent(typeof(Machine))]

public class Shrine : MonoBehaviour
{
    [SerializeField]
    ShrineFlag flag;

    Usable usable;
    Machine machine;
    Arena arena;
    Referee referee;

    Logger logger;

    Timeline timeline;

    bool complete => logger.GetShrine(flag);

    void Use()
    {
        if(complete){ usable.Notify("You have completed this challenge"); return; }

        if(machine.InState(Watching))
        {
            machine.Transition(Active);
        }
    }

    void Watching(StateSignal signal)
    {
    	switch(signal)
    	{
            case StateSignal.ENTER:
                usable.show_prompt = true;

                if(complete){ return; }
                arena.Spawn();
                arena.transform.localScale = new Vector3(0, 0, 1);
            break;

    		case StateSignal.TICK:
                if(complete){ return; }
                arena.transform.localScale = NumTools.XY_Scale(usable.usability);
    		break;

            case StateSignal.EXIT:
                usable.show_prompt = false;
            break;
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
                switch(referee.EvaluateCombat())
                {
                    case 1:
                        logger.AddShrine(flag);
                        AudioWizard._.PopMusic();
                        AudioWizard._.PlayEffect("victory");
                        arena.Clear();
                        machine.Transition(Cleared);
                    break;

                    case -1:
                        AudioWizard._.PopMusic();
                        arena.Clear();
                        machine.Transition(Watching);
                    break;
                }
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
        }
    }

    void Awake()
    {
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
