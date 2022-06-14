using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Machine))]
[RequireComponent(typeof(Combatant))]

public class Shooter : Controller
{
    ProgressRing progress_ring;
    [SerializeField]
    GameObject[] life_orbs;

    Machine machine;
    Combatant combatant;

    CombatMode[] modes;
    Timeline[] mode_cooldowns;
    int mode_shift;
    CombatMode ability => modes[1 + mode_shift];

    void HitEffects()
    {
        int life_index = (int)(combatant.life * 3);
        Vector3 orb_position = life_orbs[life_index].transform.position;
        Destroy(life_orbs[life_index]);

        ProgressRing hit_ring = AssetTools.SpawnComponent(progress_ring);
        hit_ring.Initialize(Color.red, 0.1f, combatant.arena_scale * 2.5f, 0.75f);
        hit_ring.transform.position = orb_position;
    }

    void DeathEffects()
    {
        ProgressRing death_ring = AssetTools.SpawnComponent(progress_ring);
        death_ring.Initialize(Color.red, 0.05f, combatant.arena_scale * 5, 1.5f);
        death_ring.transform.position = transform.position;

        Destroy(gameObject);
    }

    void Awake()
    {
        progress_ring = Resources.Load<ProgressRing>("Progress Ring");

        machine = GetComponent<Machine>();
        combatant = GetComponent<Combatant>();

        combatant.on_hurt.AddListener(HitEffects);
        combatant.on_die.AddListener(DeathEffects);
    }

    void Start()
    {
        modes = GetComponentsInChildren<CombatMode>();

        foreach(CombatMode mode in modes)
        {
            Commandeer(mode);
            mode.BindDefault(modes[0]);
        }

        for(int i = 1; i < modes.Length; i++)
        {
            if(modes[i].unlocked){ mode_shift = i-1; }
        }

        machine.Transition(modes[0].Entry);
    }

    void Update()
    {
        int mode_flux = 0;

        if(Pressed(InputCode.BACK)){ mode_flux = modes.Length-2; }
        else if(Pressed(InputCode.FORTH)){ mode_flux = 1; }

        if(mode_flux != 0)
        {
            int old_shift = mode_shift;
            do
            {
                mode_shift = (mode_shift + mode_flux) % (modes.Length-1);
            }
            while(!ability.unlocked && mode_shift != old_shift);
        }

        if(Pressed(InputCode.CONFIRM) && ability.unlocked && ability.ready)
        {
            machine.Transition(ability.Entry);
        }
    }
}
