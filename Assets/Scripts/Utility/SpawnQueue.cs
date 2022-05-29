using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnQueue : MonoBehaviour
{
    Queue<GameObject> queue;
    GameObject last;

    public void Add(GameObject entry)
    {
        queue.Enqueue(entry);
    }

    void Awake()
    {
        queue = new Queue<GameObject>();
    }

    void Update()
    {
        if(last == null && queue.Count > 0)
        { last = Instantiate(queue.Dequeue()); }
    }
}
