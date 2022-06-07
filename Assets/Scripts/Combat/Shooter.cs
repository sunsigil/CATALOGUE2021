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
    int mode_shift;

    float real_scale => transform.lossyScale.x;

    void HitEffects()
    {
        int life_index = (int)(combatant.life * 3);
        Vector3 orb_position = life_orbs[life_index].transform.position;
        Destroy(life_orbs[life_index]);

        ProgressRing hit_ring = AssetTools.SpawnComponent(progress_ring);
        hit_ring.Initialize(Color.red, 0.05f, real_scale * 5, 0.75f);
        hit_ring.transform.position = orb_position;
    }

    void DeathEffects()
    {
        ProgressRing death_ring = AssetTools.SpawnComponent(progress_ring);
        death_ring.Initialize(Color.red, 0.1f, real_scale * 10, 1.5f);
        death_ring.transform.position = transform.position;

        Destroy(gameObject);
    }

    void Awake()
    {
        progress_ring = Resources.Load<ProgressRing>("Progress Ring");

        machine = GetComponent<Machine>();
        combatant = GetComponent<Combatant>();

        combatant.on_hit.AddListener(HitEffects);
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

        mode_shift = 0;
        machine.Transition(modes[0].Entry);
    }

    void Update()
    {
        int mode_flux = 0;

        if(Pressed(InputCode.BACK)){ mode_flux = -1; }
        else if(Pressed(InputCode.FORTH)){ mode_flux = 1; }

        if(mode_flux != 0)
        {
            mode_shift = (mode_shift + mode_flux) % (modes.Length-1);
        }

        print(mode_shift);

        if(Pressed(InputCode.CONFIRM))
        {
            machine.Transition(modes[1 + mode_shift].Entry);
        }
    }
}
