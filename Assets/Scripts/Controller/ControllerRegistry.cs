using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerRegistry : MonoBehaviour
{
    static ControllerRegistry instance;
    public static ControllerRegistry _ => instance;

    [SerializeField]
    TextAsset scheme_file;

    ControlScheme _scheme;
    public ControlScheme scheme => _scheme;

    List<Controller> controllers;
    int index;

    static int CompareByControlLayer(Controller a, Controller b)
    {
        if(a == null && b == null){return 0;}
        if(b == null){return 1;}
        if(a == null){return -1;}

        if(a.control_layer > b.control_layer){return 1;}
        if(a.control_layer < b.control_layer){return -1;}

        return 0;
    }

    public void Register(Controller controller)
    {
        if(!controllers.Contains(controller))
        {
            controller.scheme = _scheme;

            if(controllers.Count > 0)
            {
                controllers[index].is_current = false;
            }

            controllers.Add(controller);
            index++;

            controller.is_registered = true;

            controllers.Sort(CompareByControlLayer);

            controllers[index].is_current = true;
        }
    }

    public void Deregister(Controller controller)
    {
        if
        (
            controllers.Count > 0 &&
            controllers[index] == controller
        )
        {
            controller.is_current = false;

            while
            (
                controllers.Count > 0 &&
                (controllers[index] == null ||
                !controllers[index].gameObject.activeSelf ||
                controllers[index] == controller)
            )
            {
                controllers[index].is_registered = false;
                controllers.RemoveAt(index--);
            }

            if(controllers.Count > 0)
            {
                controllers.Sort(CompareByControlLayer);

                controllers[index].is_current = true;
            }
        }
    }

    void Awake()
    {
        if(!instance){instance = this;}
        else{Destroy(this);}

        _scheme = new ControlScheme(scheme_file);

        controllers = new List<Controller>();
        index = -1;

        foreach(Controller controller in FindObjectsOfType<Controller>())
        {
            if(!controller.unmanaged)
            {
                Register(controller);
            }
            else
            {
                controller.scheme = _scheme;
            }
        }
    }
}
