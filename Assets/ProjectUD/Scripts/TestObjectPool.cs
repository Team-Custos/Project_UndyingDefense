using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TestObjectPool : MonoBehaviour 
{
    [SerializeField] Transform vfxParent;

    private VFX v;

    private Dictionary<GameObject, ObjectPoolWithList<GameObject>> vfxDic = new Dictionary<GameObject, ObjectPoolWithList<GameObject>>();

    public GameObject GetVFX(GameObject vfx)
    {
        if(vfxDic.ContainsKey(vfx))
        {
            if(vfxDic[vfx].Pool.CountInactive <= 0)
            {
                CreateVFX(vfx);
            }
        }
        else
        {
            vfxDic.Add(vfx, new ObjectPoolWithList<GameObject>(() => CreateVFX(vfx)));
        }

        GameObject obj = vfxDic[vfx].Pool.Get();
        vfxDic[vfx].List.Add(obj);
        return vfx;
    }

    public GameObject CreateVFX(GameObject vfx)
    {
        GameObject obj = Instantiate(vfx);
        obj.transform.SetParent(vfxParent);
        v = obj.GetComponent<VFX>();
        v.InitializePool(vfxDic[vfx], vfxParent);
        obj.SetActive(false);
        return obj;
    }
}
