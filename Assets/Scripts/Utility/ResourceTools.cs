using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceTools
{
	public static T GetDefault<T>() where T : Object
    {
        return Resources.Load<T>("default");
    }

    public static Dictionary<string, T> MapResources<T>(string path) where T : Object
    {
        Dictionary<string, T> dict = new Dictionary<string, T>();

        object[] asset_objects = Resources.LoadAll(path, typeof(T));
        T[] assets = (T[]) asset_objects;

        foreach(T asset in assets)
        {
            dict.Add(asset.name, asset);
        }

        return dict;
    }

    public static Dictionary<string, T> MapResources<T>(string[] names, string path) where T : Object
    {
        Dictionary<string, T> dict = new Dictionary<string, T>();

        T[] assets = Resources.LoadAll<T>(path);
        T default_asset = Resources.Load<T>("default");

        foreach(T asset in assets)
        {
            if(asset.name.Equals("default"))
            {
                default_asset = asset;
                break;
            }
        }

        foreach(string name in names)
        {
            T match = default_asset;

            foreach(T asset in assets)
            {
                if(asset.name.Equals(name))
                {
                    match = asset;
                    break;
                }
            }

            dict.Add(name, match);
        }

        return dict;
    }

    public static Dictionary<A, B> MapResources<A, B>(string path)
    where A : System.Enum
    where B : Object
    {
        A[] items = EnumTools.EnumArray<A>();
        Dictionary<A, B> dict = new Dictionary<A, B>();

        B[] assets = Resources.LoadAll<B>(path);
        B default_asset = Resources.Load<B>("default");

        foreach(B asset in assets)
        {
            if(asset.name.Equals("default"))
            {
                default_asset = asset;
                break;
            }
        }

        foreach(A item in items)
        {
            B match = default_asset;

            foreach(B asset in assets)
            {
                if(asset.name.Equals(System.Enum.GetName(typeof(A), item).ToLower()))
                {
                    match = asset;
                    break;
                }
            }

            dict.Add(item, match);
        }

        return dict;
    }

    public static Dictionary<A, B> MapResources<A, B>(A[] items, string path)
    where A : Object
    where B : Object
    {
        Dictionary<A, B> dict = new Dictionary<A, B>();

        B[] assets = Resources.LoadAll<B>(path);
        B default_asset = Resources.Load<B>("default");

        foreach(B asset in assets)
        {
            if(asset.name.Equals("default"))
            {
                default_asset = asset;
                break;
            }
        }

        foreach(A item in items)
        {
            B match = default_asset;

            foreach(B asset in assets)
            {
                if(asset.name.Equals(item.name.ToLower()))
                {
                    match = asset;
                    break;
                }
            }

            dict.Add(item, match);
        }

        return dict;
    }
}
