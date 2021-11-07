using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CowTools
{
    public static T[] LoadAssets<T>(string path)
	{
		object[] assetObjects = Resources.LoadAll(path, typeof(T));

		T[] assets = new T[assetObjects.Length];

		for(int i = 0; i < assetObjects.Length; i++)
		{
			assets[i] = (T)assetObjects[i];
		}

		return assets;
	}

    public static int GetEnumLength<TEnum>()
    {
        return System.Enum.GetNames(typeof(TEnum)).Length;
    }

    public static TEnum StringToEnum<TEnum>(string s)
    {
        return (TEnum)System.Enum.Parse(typeof(TEnum), s, true);
    }

    public static TEnum[] EnumArray<TEnum>()
    {
        return (TEnum[])System.Enum.GetValues(typeof(TEnum));
    }

    public static Quaternion Vec2Rot(Vector2 v, float offset=0)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + offset);
    }
}
