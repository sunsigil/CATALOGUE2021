using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Machine))]
[RequireComponent(typeof(Combatant))]

public class Shooter : Controller
{
    [SerializeField]
    GameObject death_ring_prefab;
    [SerializeField]
    GameObject[] life_orbs;

    Machine machine;
    Combatant combatant;
    Slingshot slingshot;
    Cannon cannon;

    Machine.MachineState[] modes;
    int mode_index;

    void HitEffects()
    {
        int life_index = (int)(combatant.life * 3);
        Vector3 orb_position = life_orbs[life_index].transform.position;
        Destroy(life_orbs[life_index]);

        GameObject hit_ring_object = Instantiate(death_ring_prefab);
        hit_ring_object.transform.localScale = transform.lossyScale * 3;
        hit_ring_object.transform.position = orb_position;
    }

    void DeathEffects()
    {
        GameObject death_ring_object = Instantiate(death_ring_prefab);
        death_ring_object.transform.localScale = transform.lossyScale * 10;
        death_ring_object.transform.position = transform.position;

        Destroy(gameObject);
    }

    void Awake()
    {
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
            mode_index = (mode_index+1) % modes.Length;
            machine.Transition(modes[mode_index]);
        }
    }
}
