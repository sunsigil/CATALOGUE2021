using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleHandler : Controller
{
    PopupBar popup_bar_prefab;

    [SerializeField]
    CommandConsole console_prefab;
    [SerializeField]
    TextAsset startup_file;
    [SerializeField]
    bool enable_commands;

    CommandConsole console_instance;
    int open_attempts;

    void Awake()
    {
        popup_bar_prefab = Resources.Load<PopupBar>("Popup Bar");
    }

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
            if(enable_commands)
            {
                if(console_instance == null){ console_instance = AssetTools.SpawnComponent(console_prefab); }
                else{ Destroy(console_instance.gameObject); }
            }
            else
            {
                open_attempts++;

                if(open_attempts >= 10)
                {
                    PopupBar popup_bar = AssetTools.SpawnComponent(popup_bar_prefab);
                    popup_bar.message = "No, you really can't use console commands";
                    open_attempts = 0;
                }
            }
        }
    }
}
