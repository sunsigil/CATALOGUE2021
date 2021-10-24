using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    [SerializeField]
    protected bool _unmanaged;
    public bool unmanaged => _unmanaged;

    [SerializeField]
    ControlLayer _controlLayer;
    public ControlLayer controlLayer => _controlLayer;

    [SerializeField]
    protected ControlScheme _scheme;
    public ControlScheme scheme
    {
        get => _scheme;
        set => _scheme = value;
    }

    protected bool _isRegistered;
    public bool isRegistered
    {
        get => _isRegistered;
        set => _isRegistered = value;
    }

    protected bool _isCurrent;
    public bool isCurrent
    {
        get => _isCurrent;
        set => _isCurrent = value;
    }

    bool isOperable => _unmanaged || (_isRegistered && _isCurrent) && _scheme != null;

    protected bool Pressed(InputCode code)
    {
        return isOperable && Input.GetKeyDown(scheme.Convert(code));
    }

    protected bool Released(InputCode code)
    {
        return isOperable && Input.GetKeyUp(scheme.Convert(code));
    }

    protected bool Held(InputCode code)
    {
        return isOperable && Input.GetKey(scheme.Convert(code));
    }

    protected float InputValue(string axis)
    {
        return isOperable ? Input.GetAxis(axis) : 0;
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

        if(_isRegistered)
        {
            ControllerRegistry._.Deregister(this);
        }
    }
}
