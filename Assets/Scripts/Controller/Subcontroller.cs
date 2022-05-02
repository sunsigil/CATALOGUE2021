using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subcontroller : MonoBehaviour
{
    Controller commander;

    public void Obey(Controller commander){ this.commander = commander; }

    public bool Pressed(InputCode code){ return commander.Pressed(code); }

    public bool Released(InputCode code){ return commander.Released(code); }

    public bool Held(InputCode code){ return commander.Held(code); }

    public float InputValue(string axis, bool raw=false){ return commander.InputValue(axis, raw); }
}
