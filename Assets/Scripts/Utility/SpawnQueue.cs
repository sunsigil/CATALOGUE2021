using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnQueue : MonoBehaviour
{
    Queue<GameObject> queue;
    GameObject last;
    GameObject hold;

    public void WaitFor(GameObject hold)
    {
        this.hold = hold;
    }

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
        if(hold == null && last == null && queue.Count > 0)
        { last = Instantiate(queue.Dequeue()); }
    }
}
