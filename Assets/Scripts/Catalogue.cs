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
    int wayshrines;

    float x_position;
    float y_rotation;
    float furthest_x;

    public void AddShrine(ShrineToken shrine)
    {
        shrines |= (1 << (int)shrine);
    }

    public bool GetShrine(ShrineToken shrine)
    {
        return (shrines & (1 << (int)shrine)) > 0;
    }

    public bool GetShrine(int index)
    {
        return (shrines & (1 << index)) > 0;
    }

    public void AddWayshrine(WayshrineToken wayshrine)
    {
        wayshrines |= (1 << (int)wayshrine);
    }

    public bool GetWayshrine(WayshrineToken wayshrine)
    {
        return (wayshrines & (1 << (int)wayshrine)) > 0;
    }

    public bool GetWayshrine(int index)
    {
        return (wayshrines & (1 << index)) > 0;
    }

    public bool IsComplete()
    {
        return (shrines == 0b11111111) && (wayshrines == 0b11111111);
    }

    public void Save()
    {
        x_position = transform.position.x;
        y_rotation = transform.rotation.eulerAngles.y;
        if(x_position > furthest_x){furthest_x = x_position;}

        StreamWriter writer = new StreamWriter(full_save_path);
        writer.WriteLine(shrines.ToString());
        writer.WriteLine(wayshrines.ToString());
        writer.WriteLine(x_position.ToString());
        writer.WriteLine(y_rotation.ToString());
        writer.WriteLine(furthest_x.ToString());
        writer.Close();
    }

    public void Load()
    {
        try
        {
            StreamReader reader = new StreamReader(full_save_path);
            shrines = int.Parse(reader.ReadLine());
            wayshrines = int.Parse(reader.ReadLine());
            x_position = float.Parse(reader.ReadLine());
            y_rotation = float.Parse(reader.ReadLine());
            furthest_x = float.Parse(reader.ReadLine());
            reader.Close();
        }
        catch
        {
            return;
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
