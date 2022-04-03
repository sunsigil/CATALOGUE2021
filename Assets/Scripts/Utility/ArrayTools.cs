using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayTools
{
	public static void Swap<T>(T[] arr, int i, int j)
    {
        T temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }

	public static T PickRandom<T>(T[] array)
    {
        int index = Random.Range(0, array.Length);
        return array[index];
    }

	public static T[] CopyArray<T>(T[] arr)
    {
        T[] copy = new T[arr.Length];

        for(int i = 0; i < arr.Length; i++)
        {
            copy[i] = arr[i];
        }

        return copy;
    }

    public static T[] ShuffleArray<T>(T[] arr)
	{
		T[] copy = CopyArray(arr);

		for(int i = copy.Length-1; i >= 1; i--)
		{
			int j = Random.Range(0, i);
			Debug.Log(i);
			Debug.Log(j);
			Swap(copy, i, j);
		}

        return copy;
	}

	public static int Find<T>(T[] arr, T item)
	where T : class
	{
		for(int i = 0; i < arr.Length; i++)
		{
			if(arr[i] == item)
			{
				return i;
			}
		}

		return -1;
	}

	public static int FindEq<T>(T[] arr, T item)
	{
		for(int i = 0; i < arr.Length; i++)
		{
			if(arr[i].Equals(item))
			{
				return i;
			}
		}

		return -1;
	}
}
