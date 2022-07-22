using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logger : MonoBehaviour
{
    [SerializeField]
    string save_path;
    string full_save_path => $"{Application.persistentDataPath}\\{save_path}";
    [SerializeField]
    float save_cooldown;

    int shrines;
    int runes;
    int cards;

    float x_position;
    float y_rotation;

    string satchel_log;

    Timeline timeline;

    public void AddShrine(ShrineFlag shrine){ shrines = EnumTools.Flag(shrines, shrine); }
    public void AddShrine(int index){ shrines |= (1 << index); }
    public bool GetShrine(ShrineFlag shrine){ return EnumTools.IsFlagged(shrines, shrine); }
    public bool GetShrine(int index){ return (shrines & (1 << index)) > 0; }
    public string ShrineDump(){ return EnumTools.FlagView<ShrineFlag>(shrines); }

    public void AddRune(RuneFlag rune){ runes = EnumTools.Flag(runes, rune); }
    public void AddRune(int index){ runes |= (1 << index); }
    public bool GetRune(RuneFlag rune){ return EnumTools.IsFlagged(runes, rune); }
    public bool GetRune(int index){ return (runes & (1 << index)) > 0; }
    public string RuneDump(){ return EnumTools.FlagView<RuneFlag>(runes); }

    public void AddCard(CardFlag card){ cards = EnumTools.Flag(cards, card); }
    public void AddCard(int index){ cards |= (1 << index); }
    public bool GetCard(CardFlag card){ return EnumTools.IsFlagged(cards, card); }
    public bool GetCard(int index){ return (cards & (1 << index)) > 0; }
    public string CardDump(){ return EnumTools.FlagView<CardFlag>(cards); }

    [ContextMenu("Clear Saved Data")]
    public void Clear()
    {
        StreamWriter writer = new StreamWriter(full_save_path);
        writer.WriteLine("");
        writer.Close();
    }

    public void Save()
    {
        StreamWriter writer = new StreamWriter(full_save_path);

        writer.WriteLine(shrines.ToString());
        writer.WriteLine(runes.ToString());
        writer.WriteLine(cards.ToString());

        Walker walker = FindObjectOfType<Walker>();
        if(walker)
        {
            x_position = walker.transform.position.x;
            y_rotation = walker.transform.rotation.eulerAngles.y;
        }
        writer.WriteLine(x_position.ToString());
        writer.WriteLine(y_rotation.ToString());

        Satchel satchel = FindObjectOfType<Satchel>();
        if(satchel)
        {
            satchel_log = satchel.IngredientDump().Trim(' ');
        }
        writer.WriteLine(satchel_log);

        writer.Close();
    }

    public void Load()
    {
        try
        {
            StreamReader reader = new StreamReader(full_save_path);
            shrines = int.Parse(reader.ReadLine());
            runes = int.Parse(reader.ReadLine());
            cards = int.Parse(reader.ReadLine());
            x_position = float.Parse(reader.ReadLine());
            y_rotation = float.Parse(reader.ReadLine());
            satchel_log = reader.ReadLine();
            reader.Close();
        }
        catch
        {
            print("catching");
            shrines = 0;
            runes = 0;
            cards = 0;
            x_position = 0;
            y_rotation = 0;
            satchel_log = "";
        }

        Walker walker = FindObjectOfType<Walker>();
        if(walker)
        {
            Vector3 position = walker.transform.position;
            walker.transform.position = new Vector3(x_position, position.y, position.z);
            Vector3 rotation = walker.transform.rotation.eulerAngles;
            walker.transform.rotation = Quaternion.Euler(0, y_rotation, 0);
        }
    }

    public void Restart()
    {
        Save();
        SceneManager.LoadScene("Island");
    }

    void Awake()
    {
        Load();
    }

    void Start()
    {
        Satchel satchel = FindObjectOfType<Satchel>();
        if(satchel)
        {
            foreach(string ing_name in satchel_log.Split(' '))
            {
                Ingredient ing = Resources.Load<Ingredient>($"Ingredients/{ing_name}");
                if(ing != null){ satchel.Add(ing); }
            }
        }
    }

    void Update()
    {
        if(timeline == null || timeline.Evaluate())
        {
            timeline = new Timeline(save_cooldown);
            Save();
        }

        timeline.Tick(Time.deltaTime);
    }

    void OnDestroy()
    { Save(); }

    void OnApplicationQuit()
    { Save(); }
}
