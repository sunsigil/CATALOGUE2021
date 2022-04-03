using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Satchel : MonoBehaviour
{
    [SerializeField]
    int size;

    List<Ingredient> _contents;
    public List<Ingredient> contents => _contents;

    UnityEvent _on_add;
    public UnityEvent on_add => _on_add;
    UnityEvent _on_remove;
    public UnityEvent on_remove => _on_remove;

    public void Add(Ingredient ingredient)
    {
        if(_contents.Count == size){ return; }

        print(ingredient.name);
        _contents.Add(ingredient);
        _on_add.Invoke();
    }

    public void Remove(Ingredient ingredient)
    {
        if(_contents.Count == 0){ return; }

        _contents.Remove(ingredient);
        _on_remove.Invoke();
    }

    public void Clear()
    {
        _contents.Clear();
        _on_remove.Invoke();
    }

    public bool Contains(Ingredient ingredient)
    {
        for(int i = 0; i < _contents.Count; i++)
        {
            if(_contents[i] == ingredient){ return true; }
        }

        return false;
    }

    void Awake()
    {
        _contents = new List<Ingredient>();
        _on_add = new UnityEvent();
        _on_remove = new UnityEvent();
    }
}
