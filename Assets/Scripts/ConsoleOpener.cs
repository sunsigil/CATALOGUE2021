using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleOpener : Controller
{
    [SerializeField]
    CommandConsole console_prefab;
    CommandConsole console_instance;

    // Update is called once per frame
    void Update()
    {
        if(Pressed(InputCode.CONSOLE))
        {
            if(console_instance == null){ console_instance = AssetTools.SpawnComponent(console_prefab); }
            else{ Destroy(console_instance.gameObject); }
        }
    }
}
