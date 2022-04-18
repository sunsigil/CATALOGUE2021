using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnQueue : MonoBehaviour
{
    [SerializeField]
    GameObject[] initials;

    Queue<GameObject> queue;
    GameObject last;

    public void Add(GameObject entry)
    {
        queue.Enqueue(entry);
    }

    void Awake()
    {
        queue = new Queue<GameObject>();

        foreach(GameObject initial in initials)
        {
            Add(initial);
        }
    }

    void Update()
    {
        if(last == null && queue.Count > 0)
        { last = Instantiate(queue.Dequeue()); }
    }
}
