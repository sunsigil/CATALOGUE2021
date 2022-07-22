using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandConsole : Controller
{
    TextMeshProUGUI log;
    TMP_InputField prompt;

    string log_string;
    Stack<string> command_stack;

    List<string> command_record;
    int record_index;

#region OP_FUNCS
    void Help()
    {
        Log("[OPERATIONS]");
        Log("clear");
        Log("save");
        Log("load");
        Log("quit");
        Log("reset [WARNING: WILL DELETE SAVE DATA]");
        Log("give_shrine <idx: int>");
        Log("shrines");
        Log("give_rune <idx: int>");
        Log("runes");
        Log("give_card <name: string>");
        Log("cards");
        Log("give_ingredient <name: string>");
        Log("ingredients");
        Log("toggle_music <toggle: bool>");
        Log("play_clip <name: string>");
    }

    void Clear()
    {
        log_string = "";
        log.text = "";
    }

    void Save()
    {
        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }

        Log("Saving...");
        logger.Save();
    }

    void Load()
    {
        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }

        Log("Load...");
        logger.Load();
    }

    void Quit()
    {
        Log("Quitting...");
        Application.Quit();
    }

    void Reset()
    {
        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }

        Log("Clearing save data...");
        logger.Clear();
        Log("Restarting...");
        logger.Restart();
    }

    void GiveShrine(Stack<string> arg_stack)
    {
        string idx_str = "";
        try{ idx_str = arg_stack.Pop(); }
        catch{ Log("Operation give_shrine requires one argument"); return; }

        int idx = -1;
        try{ idx = Int32.Parse(idx_str); }
        catch{ Log("Argument idx must be of integer type"); return; }
        if(idx < 0 || idx > 3){ Log("Argument idx must be within range [0, 3]"); return; }

        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }

        logger.AddShrine(idx);
    }

    void Shrines()
    {
        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }
        Log(logger.ShrineDump());
    }

    void GiveRune(Stack<string> arg_stack)
    {
        string idx_str = "";
        try{ idx_str = arg_stack.Pop(); }
        catch{ Log("Operation give_rune requires one argument"); return; }

        int idx = -1;
        try{ idx = Int32.Parse(idx_str); }
        catch{ Log("Argument idx must be of integer type"); return; }
        if(idx < 0 || idx > 7){ Log("Argument idx must be within range [0, 7]"); return; }

        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }

        logger.AddRune(idx);
    }

    void Runes()
    {
        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }
        Log(logger.RuneDump());
    }

    void GiveCard(Stack<string> arg_stack)
    {
        string card_str = "";
        try{ card_str = arg_stack.Pop(); }
        catch{ Log("Operation give_card requires one argument"); return; }

        Card card = Resources.Load<Card>($"Cards/{card_str}");
        if(card == null){ Log($"Error: card {card_str} not found"); return; }

        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }

        logger.AddCard(card.flag);
    }

    void Cards()
    {
        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }
        Log(logger.CardDump());
    }

    void GiveIngredient(Stack<string> arg_stack)
    {
        string ing_str = "";
        try{ ing_str = arg_stack.Pop(); }
        catch{ Log("Operation give_ingredient requires one argument"); return; }

        Ingredient ing = Resources.Load<Ingredient>($"Ingredients/{ing_str}");
        if(ing == null){ Log($"Error: ingredient {ing_str} not found"); return; }

        Satchel satchel = FindObjectOfType<Satchel>();
        if(satchel == null){ Log("Error: satchel not found"); return; }

        satchel.Add(ing);
    }

    void Ingredients()
    {
        Satchel satchel = FindObjectOfType<Satchel>();
        if(satchel == null){ Log("Error: satchel not found"); return; }
        Log(satchel.IngredientDump());
    }

    void ToggleMusic(Stack<string> arg_stack)
    {
        string tog_str = "";
        try{ tog_str = arg_stack.Pop(); }
        catch{ Log("Operation toggle_music requires one argument"); return; }

        bool tog = false;
        try{ tog = Boolean.Parse(tog_str); }
        catch{ Log("Argument idx must be of boolean type {True, False}"); return; }

        if(AudioWizard._ == null){ Log("Error: audio wizard not found"); return; }

        AudioWizard._.ToggleMusic(tog);
    }

    void PlayClip(Stack<string> arg_stack)
    {
        string clip_str = "";
        try{ clip_str = arg_stack.Pop(); }
        catch{ Log("Operation play_clip requires one argument"); return; }

        if(AudioWizard._ == null){ Log("Error: audio wizard not found"); return; }

        if(!AudioWizard._.PlayEffect(clip_str)){ Log($"Error: clip {clip_str} not found"); }
    }
#endregion

#region ONSUBMIT_FUNCS
    void Record(string command)
    {
        if(command_record.Count == 0 || command_record[command_record.Count-1] != command)
        {
            command_record.Add(command);
            record_index = command_record.Count;
        }
    }

    void Log(string text)
    {
        log_string += $"{text}\n";
        log.text = log_string;
        log.ForceMeshUpdate();

        if(log.isTextTruncated)
        {
            log_string = $"{text}\n";
            log.text = log_string;
        }
    }

    void Evaluate(string command)
    {
        string[] command_arr = command.ToLower().Split(' ');
        Array.Reverse(command_arr);
        command_stack = new Stack<string>(command_arr);

        string op = command_stack.Pop();

        switch(op)
        {
            case "help":
                Help();
                break;
            case "clear":
                Clear();
                break;
            case "save":
                Save();
                break;
            case "load":
                Load();
                break;
            case "quit":
                Quit();
                break;
            case "reset":
                Reset();
                break;
            case "give_shrine":
                GiveShrine(command_stack);
                break;
            case "shrines":
                Shrines();
                break;
            case "give_rune":
                GiveRune(command_stack);
                break;
            case "runes":
                Runes();
                break;
            case "give_card":
                GiveCard(command_stack);
                break;
            case "cards":
                Cards();
                break;
            case "give_ingredient":
                GiveIngredient(command_stack);
                break;
            case "ingredients":
                Ingredients();
                break;
            case "toggle_music":
                ToggleMusic(command_stack);
                break;
            case "play_clip":
                PlayClip(command_stack);
                break;
            default:
                Log($"Unknown operation: {op}");
                break;
        }
    }

    public void Process(string command)
    {
        if(String.IsNullOrWhiteSpace(command)){ prompt.text = ""; return; }

        command = command.Trim(new char[]{' ', '\n'});
        if(command.StartsWith("#")){ prompt.text = ""; return; }

        Record(command);
        Log(command);
        prompt.text = "";

        Evaluate(command);
    }
#endregion

#region PUBLIC_FUNCS
    void Awake()
    {
        log = GetComponentInChildren<TextMeshProUGUI>();
        prompt = GetComponentInChildren<TMP_InputField>();

        command_record = new List<string>();
        record_index = 0;

        prompt.onSubmit.AddListener(Process);
    }

    void Update()
    {
        if(is_operable && command_record.Count != 0)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                record_index--;
                if(record_index < 0){ record_index++; }
                prompt.text = command_record[record_index];
            }
            else if(is_operable && Input.GetKeyDown(KeyCode.DownArrow))
            {
                record_index++;
                if(record_index > command_record.Count-1)
                {
                    record_index--;
                    prompt.text = "";
                }
                else
                {
                    prompt.text = command_record[record_index];
                }
            }
        }
    }
#endregion
}
