using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleHandler : Controller
{
    [SerializeField]
    CommandConsole console_prefab;
    [SerializeField]
    TextAsset startup_file;

    CommandConsole console_instance;

    void Start()
    {
        string[] commands = startup_file.text.Split('\n');

        if(console_instance != null){ Destroy(console_instance.gameObject); }
        console_instance = AssetTools.SpawnComponent(console_prefab);

        foreach(string command in commands)
        {
            console_instance.Process(command);
        }

        Destroy(console_instance.gameObject);
    }

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
