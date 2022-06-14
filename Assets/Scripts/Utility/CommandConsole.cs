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
    void QuitGame()
    {
        Log("Quitting...");
        Application.Quit();
    }

    void ClearLog()
    {
        log_string = "";
        log.text = log_string;
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
        Log(logger.RuneDump());
    }

    void GiveCard(Stack<string> arg_stack)
    {
        string card_str = "";
        try{ card_str = arg_stack.Pop(); }
        catch{ Log("Operation give_card requires one argument"); return; }

        Card card = null;
        card = Resources.Load<Card>($"Cards/{card_str}");
        if(card == null){ Log($"Error: card {card_str} not found"); return; }

        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }

        logger.AddCard(card.flag);
        Log(logger.CardDump());
    }

    void GiveIngredient(Stack<string> arg_stack)
    {
        string ing_str = "";
        try{ ing_str = arg_stack.Pop(); }
        catch{ Log("Operation give_ingredient requires one argument"); return; }

        Ingredient ing = null;
        ing = Resources.Load<Ingredient>($"Ingredients/{ing_str}");
        if(ing == null){ Log($"Error: ingredient {ing_str} not found"); return; }

        Satchel satchel = FindObjectOfType<Satchel>();
        if(satchel == null){ Log("Error: satchel not found"); return; }

        satchel.Add(ing);
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
    }

    void Evaluate(string command)
    {
        if(String.IsNullOrWhiteSpace(command)){ return; }

        Record(command);
        Log(command);
        prompt.text = "";

        string[] command_arr = command.ToLower().Split(' ');
        Array.Reverse(command_arr);
        command_stack = new Stack<string>(command_arr);

        string op = command_stack.Pop();

        switch(op)
        {
            case "quit_game":
                QuitGame();
                break;
            case "clear":
                ClearLog();
                break;
            case "give_shrine":
                GiveShrine(command_stack);
                break;
            case "give_rune":
                GiveRune(command_stack);
                break;
            case "give_card":
                GiveCard(command_stack);
                break;
            case "give_ingredient":
                GiveIngredient(command_stack);
                break;
            default:
                Log($"Unknown operation: {op}");
                break;
        }
    }
#endregion

    void Awake()
    {
        log = GetComponentInChildren<TextMeshProUGUI>();
        prompt = GetComponentInChildren<TMP_InputField>();

        command_record = new List<string>();
        record_index = 0;

        prompt.onSubmit.AddListener(Evaluate);
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
}
