using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] float durationTime;
    private Queue<GameObject> queue;
    private Transform vfxParent;
    private float activeTimer = 0;

    private void Update()
    {
        if (activeTimer > 0)
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0)
            {
                transform.SetParent(vfxParent);
                gameObject.SetActive(false);
            }
        }
    }

    public void OnDisable()
    {
        if(queue.Contains(gameObject))
            return;
        queue.Enqueue(gameObject);
    }

    public void OnEnable()
    {
        activeTimer = durationTime;
    }

    public void InitializePool(Queue<GameObject> q, Transform parent)
    {
        queue = q;
        vfxParent = parent;
    }

}
