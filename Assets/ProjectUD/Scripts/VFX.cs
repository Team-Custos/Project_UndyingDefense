using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] float durationTime;
    private Unit unit;
    private Queue<GameObject> queue;
    private Transform vfxParent;
    private float activeTimer = 0;

    // ObjectPoolWithList 사용 변수
    private ObjectPoolWithList<GameObject> pool;

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

        // ObjectPoolWithList 사용 변수
        //pool.Pool.Release(gameObject);
        //pool.List.Remove(gameObject);
    }

    public void OnEnable()
    {
        activeTimer = durationTime;
    }

    public void InitializePool(Queue<GameObject> q, Transform parent, Unit unit)
    {
        queue = q;
        vfxParent = parent;
        this.unit = unit;
    }

    public void InitializePool(ObjectPoolWithList<GameObject> pool, Transform parent)   // ObjectPoolWithList용 대리자 메서드
    {
        this.pool = pool;
        vfxParent = parent;
    }

}
