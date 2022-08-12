using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains and manages the Controllers
/// in a scene. Controllers are registered
/// in order of activation time and sorted by
/// control layer. The controller at the end of
/// the registry's sorted list is the active
/// controller.
/// </summary>
public class ControllerRegistry : MonoBehaviour
{
    // Settings
    [SerializeField]
    TextAsset scheme_file;

    // State
    bool _primed;
    public bool primed => _primed;

    ControlScheme _scheme;
    public ControlScheme scheme => _scheme;

    List<Controller> controllers;
    int index;

    Controller _current;
    public Controller current => _current;

    static int CompareByControlLayer(Controller a, Controller b)
    {
        if(a == null && b == null){return 0;}
        if(b == null){return 1;}
        if(a == null){return -1;}

        if(a.control_layer > b.control_layer){return 1;}
        if(a.control_layer < b.control_layer){return -1;}

        return 0;
    }

    /// <summary>
    /// Register an unregistered controller
    /// and determine controller priority
    /// accordingly
    /// </summary>
    /// <param name="controller"></param>
    public void Register(Controller controller)
    {
        if(!controllers.Contains(controller))
        {
            controller.scheme = _scheme;

            if(controllers.Count > 0)
            {
                // Deactive the previous active controller
                // NOTE: if the previous active controller
                // keeps precedence by the end of the registration
                // process, it will be reactivated by nature
                // of the process
                _current = null;
                controllers[index].is_current = false;
            }

            controllers.Add(controller);
            controller.is_registered = true;
            index++;

            controllers.Sort(CompareByControlLayer);

            // The active controller is always
            // at the end of the list;
            _current = controllers[index];
            controllers[index].is_current = true;
        }
    }

    /// <summary>
    /// Deregister a registered controller,
    /// cleaning out all other invalid controllers
    /// in the registry as well and determining
    /// controller priority accordingly
    /// </summary>
    /// <param name="controller"></param>
    public void Deregister(Controller controller)
    {
        for(int i = 0; i < controllers.Count; i++)
        {
            if(
                controllers[i] == null ||
                !controllers[i].gameObject.activeSelf ||
                controllers[i] == controller
            )
            {
                controllers[i].is_registered = false;
                controllers.RemoveAt(i);
                i--; index--;
            }
        }
        
        if(controllers.Count > 0)
        {
            _current = controllers[index];
            controllers[index].is_current = true;
        }
        
        /*if
        (
            controllers.Count > 0 &&
            controllers[index] == controller
        )
        {
            _current = null;
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

                _current = controllers[index];
                controllers[index].is_current = true;
            }
        }*/
    }

    void Awake()
    {
        _primed = true;

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
