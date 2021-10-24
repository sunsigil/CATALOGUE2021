using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class ControllerRegistry : MonoBehaviour
{
    static ControllerRegistry instance;
    public static ControllerRegistry _ => instance;

    [SerializeField]
    TextAsset schemeFile;

    ControlScheme scheme;

    List<Controller> controllers;
    int index;

    static int CompareByControlLayer(Controller a, Controller b)
    {
        if(a == null && b == null){return 0;}
        if(b == null){return 1;}
        if(a == null){return -1;}

        if(a.controlLayer > b.controlLayer){return 1;}
        if(a.controlLayer < b.controlLayer){return -1;}

        return 0;
    }

    public void Register(Controller controller)
    {
        if(!controllers.Contains(controller))
        {
            controller.scheme = scheme;

            if(controllers.Count > 0)
            {
                controllers[index].isCurrent = false;
            }

            controllers.Add(controller);
            index++;

            controller.isRegistered = true;

            controllers.Sort(CompareByControlLayer);

            controllers[index].isCurrent = true;
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
            controller.isCurrent = false;

            while
            (
                controllers.Count > 0 &&
                (controllers[index] == controller ||
                !controllers[index].gameObject.activeSelf)
            )
            {
                controllers[index].isRegistered = false;
                controllers.RemoveAt(index--);
            }

            if(controllers.Count > 0)
            {
                controllers.Sort(CompareByControlLayer);

                controllers[index].isCurrent = true;
            }
        }
    }

    void Awake()
    {
        if(!instance){instance = this;}
        else{Destroy(this);}

        scheme = new ControlScheme(XDocument.Parse(schemeFile.text));

        controllers = new List<Controller>();
        index = -1;

        foreach(Controller controller in FindObjectsOfType<Controller>())
        {
            if(!controller.unmanaged)
            {
                Register(controller);
            }
        }
    }
}
