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

#region OP_FUNCS
    void ClearLog()
    {
        log_string = "";
        log.text = log_string;
    }

    void GiveCard(Stack<string> arg_stack)
    {
        string idx_str = "";
        try{ idx_str = arg_stack.Pop(); }
        catch{ Log("Operation give_card requires one argument"); return; }

        int idx = -1;
        try{ idx = Int32.Parse(idx_str); }
        catch{ Log("Argument idx must be of integer type"); return; }
        if(idx < 0 || idx > 15){ Log("Argument idx must be within range [0, 15]"); return; }

        Logger logger = FindObjectOfType<Logger>();
        if(logger == null){ Log("Error: logger not found"); return; }

        logger.AddCard(idx);
        Log(logger.CardDump());
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

    void GiveIngredient(Stack<string> arg_stack)
    {
        string ing_str = "";
        try{ ing_str = arg_stack.Pop(); }
        catch{ Log("Operation give_ingredient requires one argument"); return; }

        Ingredient ing = null;
        try{ ing = Resources.Load<Ingredient>(ing_str); }
        catch{ Log($"Error: ingredient {ing_str} not found"); return; }

        Satchel satchel = FindObjectOfType<Satchel>();
        if(satchel == null){ Log("Error: satchel not found"); return; }

        satchel.Add(ing);
    }
#endregion

#region ONSUBMIT_FUNCS
    void Log(string text)
    {
        log_string += $"{text}\n";
        log.text = log_string;
    }

    void Evaluate(string command)
    {
        string[] command_arr = command.ToLower().Split(' ');
        Array.Reverse(command_arr);
        command_stack = new Stack<string>(command_arr);

        string op = command_stack.Pop();

        switch(op)
        {
            case "give_card":
                GiveCard(command_stack);
                break;
            case "give_rune":
                GiveRune(command_stack);
                break;
            case "give_shrine":
                GiveShrine(command_stack);
                break;
            case "give_ingredient":
                GiveIngredient(command_stack);
                break;
            case "clear":
                ClearLog();
                break;
            default:
                Log($"Unknown operation: {op}");
                break;
        }
    }

    void ClearPrompt(string discard)
    {
        prompt.text = "";
    }
#endregion

    void Awake()
    {
        log = GetComponentInChildren<TextMeshProUGUI>();
        prompt = GetComponentInChildren<TMP_InputField>();

        prompt.onSubmit.AddListener(Log);
        prompt.onSubmit.AddListener(Evaluate);
        prompt.onSubmit.AddListener(ClearPrompt);
    }
}
