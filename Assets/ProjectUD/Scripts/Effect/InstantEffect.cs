using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantEffect : MonoBehaviour
{
    protected ObjectPoolWithList<InstantEffect> pool;

    [SerializeField] private float duration;
    private float durationCheck;

    public void Initialize(ObjectPoolWithList<InstantEffect> pool)
    {
        this.pool = pool;
        durationCheck = duration;
    }

    protected virtual void Update()
    {
        durationCheck -= Time.deltaTime;
        if (durationCheck <= 0f)
        {
            durationCheck = duration;
            transform.SetParent(null);
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (pool == null)
            return;

        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);

        pool.List.Remove(this);
        pool.Pool.Release(this);
    }
}
