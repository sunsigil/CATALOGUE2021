using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Maintains the collection of ingredients
/// gathered by the player.
/// </summary>
public class Satchel : MonoBehaviour
{
    // Settings
    [SerializeField]
    int size;

    // State
    List<Ingredient> _contents;
    public List<Ingredient> contents => _contents;

    // Events
    UnityEvent _on_add;
    public UnityEvent on_add => _on_add;
    UnityEvent _on_remove;
    public UnityEvent on_remove => _on_remove;

    /// <summary>
    /// If the satchel is not full,
    /// adds the ingredient and returns true.
    /// Otherwise, adds nothing and returns false.
    /// </summary>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    public bool Add(Ingredient ingredient)
    {
        if(ingredient == null){ return false; }
        if(_contents.Count == size){ return false; }

        _contents.Add(ingredient);
        _on_add.Invoke();
        return true;
    }

    /// <summary>
    /// Removes the ingredient from the satchel
    /// and returns true if the ingredient was present.
    /// Otherwise, returns false.
    /// </summary>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    public bool Remove(Ingredient ingredient)
    {
        if(ingredient == null){ return false; }
        if(_contents.Count == 0){ return false; }

        _contents.Remove(ingredient);
        _on_remove.Invoke();
        return true;
    }

    public void Clear()
    {
        _contents.Clear();
        _on_remove.Invoke();
    }

    public bool Contains(Ingredient ingredient)
    {
        if(ingredient == null){ return false; }

        for(int i = 0; i < _contents.Count; i++)
        {
            if(_contents[i] == ingredient){ return true; }
        }

        return false;
    }

    public string IngredientDump()
    {
        string result = "";
        foreach(Ingredient ingredient in _contents)
        {
            result += $"{ingredient.name} ";
        }
        return result;
    }

    void Awake()
    {
        _contents = new List<Ingredient>();
        _on_add = new UnityEvent();
        _on_remove = new UnityEvent();
    }
}
