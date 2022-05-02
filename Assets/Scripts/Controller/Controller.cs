using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    [SerializeField]
    protected bool _unmanaged;
    public bool unmanaged => _unmanaged;

    [SerializeField]
    ControlLayer _control_layer;
    public ControlLayer control_layer => _control_layer;

    [SerializeField]
    protected ControlScheme _scheme;
    public ControlScheme scheme
    {
        get => _scheme;
        set => _scheme = value;
    }

    protected bool _is_registered;
    public bool is_registered
    {
        get => _is_registered;
        set => _is_registered = value;
    }

    protected bool _is_current;
    public bool is_current
    {
        get => _is_current;
        set => _is_current = value;
    }

    bool is_operable => _unmanaged || (_is_registered && _is_current) && _scheme != null;

    public bool Pressed(InputCode code)
    {
        return is_operable && Input.GetKeyDown(scheme.GetKeyCode(code));
    }

    public bool Released(InputCode code)
    {
        return is_operable && Input.GetKeyUp(scheme.GetKeyCode(code));
    }

    public bool Held(InputCode code)
    {
        return is_operable && Input.GetKey(scheme.GetKeyCode(code));
    }

    public float InputValue(string axis, bool raw=false)
    {
        return is_operable ? (raw ? Input.GetAxisRaw(axis) : Input.GetAxis(axis)) : 0;
    }

    public void Commandeer(Subcontroller subcontroller)
    {
        subcontroller.Obey(this);
    }

    protected virtual void OnEnable()
    {
        if(_unmanaged){return;}

        if(!ControllerRegistry._){return;}

        ControllerRegistry._.Register(this);
    }

    protected virtual void OnDisable()
    {
        if(_unmanaged){return;}

        if(!ControllerRegistry._){return;}

        if(_is_registered)
        {
            ControllerRegistry._.Deregister(this);
        }
    }
}
