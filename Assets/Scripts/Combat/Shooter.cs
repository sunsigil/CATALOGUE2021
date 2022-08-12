using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player combat controller
/// which commands the CombatModes
/// attached to it
/// </summary>
[RequireComponent(typeof(Machine))]
[RequireComponent(typeof(Combatant))]
public class Shooter : Controller
{
    // Prefabs
    ProgressRing progress_ring;

    // Plugins
    [SerializeField]
    GameObject[] life_orbs;

    // Outsiders
    ModesWidget modes_widget;

    // Components
    Machine machine;
    Combatant combatant;
    CombatMode[] modes;

    // State
    int mode_shift;
    CombatMode ability => modes[1 + mode_shift];

    void HurtEffects()
    {
        int life_index = combatant.lives;
        Vector3 orb_position = life_orbs[life_index].transform.position;
        Destroy(life_orbs[life_index]);

        ProgressRing hit_ring = AssetTools.SpawnComponent(progress_ring);
        hit_ring.Initialize(Color.red, 0.1f, combatant.arena.scale * 2.5f, 0.75f);
        hit_ring.transform.position = orb_position;
    }

    void DeathEffects()
    {
        ProgressRing death_ring = AssetTools.SpawnComponent(progress_ring);
        death_ring.Initialize(Color.red, 0.05f, combatant.arena.scale * 5, 1.5f);
        death_ring.transform.position = transform.position;

        AudioWizard._.PlayEffect("death");

        Destroy(gameObject);
    }

    void Awake()
    {
        progress_ring = Resources.Load<ProgressRing>("Progress Ring");

        machine = GetComponent<Machine>();
        combatant = GetComponent<Combatant>();

        combatant.on_hurt.AddListener(HurtEffects);
        combatant.on_deplete.AddListener(DeathEffects);
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
            if(modes[i].unlocked){ mode_shift = i-1; break; }
        }

        machine.Transition(modes[0].Entry);

        modes_widget = combatant.arena.GetComponentInChildren<ModesWidget>();
    }

    void Update()
    {
        // Scroll through combat modes using [QE]
        int mode_flux = 0;

        if(Pressed(InputCode.BACK)){ mode_flux = modes.Length-2; }
        else if(Pressed(InputCode.FORTH)){ mode_flux = 1; }
        
        if(mode_flux != 0)
        {
            mode_shift = (mode_shift + mode_flux) % (modes.Length-1);
        }
        for(int i = 0; i < modes.Length-1; i++)
        {
            int real_index = 1 + ((mode_shift + i) % (modes.Length-1));
            CombatMode mode_i = modes[real_index];
            modes_widget.Refresh(i, mode_i);
        }

        // Point machine to the selected mode
        if(Pressed(InputCode.CONFIRM) && ability.unlocked && ability.ready)
        {
            machine.Transition(ability.Entry);
        }
    }
}
