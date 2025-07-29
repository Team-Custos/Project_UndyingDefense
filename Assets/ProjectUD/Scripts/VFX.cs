using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    private Queue<GameObject> queue;


    public void OnDisable()
    {
        if(queue.Contains(gameObject))
            return;
        queue.Enqueue(gameObject);
    }

    public void InitializePool(Queue<GameObject> q)
    {
        queue = q;
    }
}
