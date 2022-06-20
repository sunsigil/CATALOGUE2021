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

    void Use()
    {
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
                if(logger.GetShrine(flag))
                {
                    machine.Transition(null);
                }
                else
                {
                    arena.Spawn();
                }
            break;

    		case StateSignal.TICK:
                arena.transform.localScale = NumTools.XY_Scale(usable.usability);
    		break;
    	}
    }

    void Active(StateSignal signal)
    {
    	switch(signal)
    	{
    		case StateSignal.ENTER:
                referee.StartCombat(arena);
    		break;

    		case StateSignal.TICK:
                switch(referee.EvaluateCombat())
                {
                    case 1:
                        arena.Clear();
                        machine.Transition(Cleared);
                    break;

                    case -1:
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
                logger.AddShrine(flag);

                timeline = new Timeline(1);
            break;

            case StateSignal.TICK:
                timeline.Tick(Time.fixedDeltaTime);

                float scale = NumTools.Powstep(timeline.progress, 6, true);
                arena.transform.localScale = NumTools.XY_Scale(scale);

                if(timeline.Evaluate())
                {
                    machine.Transition(null);
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
