using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Catalogue : MonoBehaviour
{
    [SerializeField]
    string save_path;
    string full_save_path;

    Enchantment[] enchantments;
    bool[] successes;
    float x_position;
    float y_rotation;

    public void AddSuccess(Enchantment enchantment)
    {
        for(int i = 0; i < enchantments.Length; i++)
        {
            if(enchantments[i] == enchantment)
            {
                successes[i] = true;
            }
        }
    }

    public bool GetSuccess(Enchantment enchantment)
    {
        for(int i = 0; i < enchantments.Length; i++)
        {
            if(enchantments[i] == enchantment)
            {
                return successes[i];
            }
        }

        return false;
    }

    public bool IsComplete()
    {
        foreach(bool success in successes)
        {
            if(!success){return false;}
        }

        return true;
    }

    public void Save()
    {
        string success_string = "";

        foreach(bool success in successes)
        {
            if(success){success_string += "1";}
            else{success_string += "0";}
        }

        x_position = transform.position.x;
        y_rotation = transform.rotation.eulerAngles.y;

        StreamWriter writer = new StreamWriter(full_save_path);
        writer.WriteLine(success_string);
        writer.WriteLine(x_position.ToString());
        writer.WriteLine(y_rotation.ToString());
        writer.Close();
    }

    public void Load()
    {
        string success_string = "";

        try
        {
            StreamReader reader = new StreamReader(full_save_path);
            success_string = reader.ReadLine();
            x_position = float.Parse(reader.ReadLine());
            y_rotation = float.Parse(reader.ReadLine());
            reader.Close();
        }
        catch
        {
            return;
        }

        if(!System.String.IsNullOrWhiteSpace(success_string))
        {
            for(int i = 0; i < success_string.Length; i++)
            {
                successes[i] = success_string[i] == '1';
            }
        }

        Vector3 position = transform.position;
        transform.position = new Vector3(x_position, position.y, position.z);

        Vector3 rotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, y_rotation, 0);
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        full_save_path = $"{Application.persistentDataPath}\\{save_path}";

        enchantments = CowTools.LoadAssets<Enchantment>("Enchantments");
        successes = new bool[enchantments.Length];

        StreamWriter writer = new StreamWriter(full_save_path);
        writer.WriteLine("");
        writer.Close();
    }

    void Awake()
    {
        full_save_path = $"{Application.persistentDataPath}\\{save_path}";

        enchantments = CowTools.LoadAssets<Enchantment>("Enchantments");
        successes = new bool[enchantments.Length];
    }
}
