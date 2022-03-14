using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Catalogue : MonoBehaviour
{
    [SerializeField]
    string save_path;
    string full_save_path;

    int shrines;
    int runes;

    float x_position;
    float y_rotation;

    public void AddShrine(ShrineFlag shrine)
    {
        shrines |= (1 << (int)shrine);
    }

    public bool GetShrine(ShrineFlag shrine)
    {
        return (shrines & (1 << (int)shrine)) > 0;
    }

    public bool GetShrine(int index)
    {
        return (shrines & (1 << index)) > 0;
    }

    public void AddRune(RuneFlag rune)
    {
        runes |= (1 << (int)rune);
    }

    public bool GetRune(RuneFlag rune)
    {
        return (runes & (1 << (int)rune)) > 0;
    }

    public bool GetRune(int index)
    {
        return (runes & (1 << index)) > 0;
    }

    public void Save()
    {
        x_position = transform.position.x;
        y_rotation = transform.rotation.eulerAngles.y;

        StreamWriter writer = new StreamWriter(full_save_path);
        writer.WriteLine(shrines.ToString());
        writer.WriteLine(runes.ToString());
        writer.WriteLine(x_position.ToString());
        writer.WriteLine(y_rotation.ToString());
        writer.Close();
    }

    public void Load()
    {
        try
        {
            StreamReader reader = new StreamReader(full_save_path);
            shrines = int.Parse(reader.ReadLine());
            runes = int.Parse(reader.ReadLine());
            x_position = float.Parse(reader.ReadLine());
            y_rotation = float.Parse(reader.ReadLine());
            reader.Close();
        }
        catch
        {
            shrines = 0;
            runes = 0;
            x_position = -9.5f;
            y_rotation = 0;
        }

        Vector3 position = transform.position;
        transform.position = new Vector3(x_position, position.y, position.z);

        Vector3 rotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, y_rotation, 0);
    }

    [ContextMenu("Clear Saved Data")]
    public void Clear()
    {
        full_save_path = $"{Application.persistentDataPath}\\{save_path}";

        StreamWriter writer = new StreamWriter(full_save_path);
        writer.WriteLine("");
        writer.Close();
    }

    void Awake()
    {
        full_save_path = $"{Application.persistentDataPath}\\{save_path}";

        Load();
    }

    void OnDestroy()
    {
        Save();
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
