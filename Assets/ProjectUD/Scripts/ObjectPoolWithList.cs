using System;
using System.Collections.Generic;
using UnityEngine.Pool;

public class ObjectPoolWithList<T> where T : class
{
    public ObjectPool<T> Pool { private set; get; }
    public List<T> List { private set; get; }

    public ObjectPoolWithList(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
    {
        Pool = new ObjectPool<T>(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck);
        List = new List<T>();
    }
}
