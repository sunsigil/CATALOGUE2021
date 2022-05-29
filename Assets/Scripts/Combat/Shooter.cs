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
    Slingshot slingshot;
    Cannon cannon;

    Machine.MachineState[] modes;
    int mode_index;

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
        slingshot = GetComponent<Slingshot>();
        cannon = GetComponent<Cannon>();

        combatant.on_hit.AddListener(HitEffects);
        combatant.on_die.AddListener(DeathEffects);

        Commandeer(slingshot);
        Commandeer(cannon);

        modes = new Machine.MachineState[]{slingshot.Anticipation, cannon.Anticipation};
        mode_index = 0;
        machine.Transition(modes[mode_index]);
    }

    void Update()
    {
        if(Pressed(InputCode.CONFIRM))
        {
            ProgressRing cancel_ring = AssetTools.SpawnComponent(progress_ring);
            cancel_ring.Initialize(Color.black, 0.33f, real_scale * 2.5f, 0.25f);
            cancel_ring.transform.position = transform.position;

            mode_index = (mode_index+1) % modes.Length;
            machine.Transition(modes[mode_index]);
        }
    }
}
