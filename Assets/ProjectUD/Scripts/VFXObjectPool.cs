using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



public class VFXObjectPool : MonoBehaviour
{
    [SerializeField] Transform vfxParent;
    private Unit unit;

    private Dictionary<GameObject, Queue<GameObject>> vfxDic = new Dictionary<GameObject, Queue<GameObject>>();
    private VFX v;


    public void InstantiateVFX(GameObject gameObject, Queue<GameObject> queue, Unit unit)
    {
        GameObject obj = Instantiate(gameObject);
        obj.transform.SetParent(vfxParent);
        queue.Enqueue(obj);
        v = obj.GetComponent<VFX>();
        v.InitializePool(queue, vfxParent, unit);
        obj.SetActive(false);
        //Debug.Log(queue.Count);
    }

    #region 20250730_기존 InitializeVFXPool()
    //public void InitializeVFXPool()
    //{
    //    for(int i = 0; i< vfxDatas.Count; i++)
    //    {
    //        string name = vfxDatas[i].vfxName;
    //        Queue<GameObject> q = new Queue<GameObject>();
    //        vfxDic.Add(name, q );

    //        for (int j = 0; j < vfxDatas[i].count; j++)
    //        {
    //            InstantiateVFX(vfxDatas[i].vfx, q);
    //            //GameObject obj = Instantiate (vfxDatas[i].vfx);
    //            //obj.SetActive (false);
    //            //obj.transform.SetParent(vfxParent);
    //            //q.Enqueue (obj);
    //            //v = obj.GetComponent<VFX> ();
    //            //v.InitializePool(q);

    //        }
    //    }
    //}
    #endregion

    public GameObject GetVFX(GameObject vfx, Unit unit)
    {
        #region 250730_기존 코드
        //if (vfxDic.ContainsKey(name))
        //{
        //    Queue<GameObject> q = vfxDic[name];

        //    if (q.Count <= 0)
        //    {
        //        //VFXData result = null;
        //        //for(int i =0; i< vfxDatas.Count; i++)
        //        //{
        //        //    if(vfxDatas[i].vfxName == name)
        //        //    {
        //        //        result = vfxDatas[i];
        //        //    }
        //        //}

        //        VFXData result = vfxDatas.Find(item => item.vfxName == name);

        //        if(result != null)
        //        {
        //            InstantiateVFX(result.vfx, q);
        //            //GameObject obj = Instantiate(result.vfx);
        //            //obj.SetActive(false);
        //            //obj.transform.SetParent(vfxParent);
        //            //q.Enqueue(obj);
        //            //v = obj.GetComponent<VFX>();
        //            //v.InitializePool(q);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }

        //    return q.Dequeue();

        //}
        //return null;
        #endregion

        this.unit = unit;

        if (unit.IsDead)
            return null;

        if (vfxDic.ContainsKey(vfx))
        {
            Queue<GameObject> q = vfxDic[vfx];

            if (q.Count <= 0)
            {
                InstantiateVFX(vfx, q, unit);
            }

            return q.Dequeue();
        }
        else
        {
            Queue<GameObject> q = new Queue<GameObject>();
            vfxDic.Add(vfx, q);
            InstantiateVFX(vfx, q, unit);

            return q.Dequeue();
        }
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }
}
