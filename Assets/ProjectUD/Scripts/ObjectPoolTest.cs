using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class VFXData
{
    public string vfxName;
    public GameObject vfx;
    public int count;
}

public class ObjectPoolTest : MonoBehaviour
{
    //[SerializeField] private List<GameObject> objectpool = new List<GameObject>();

    //private ObjectPool<GameObject> pools = new ObjectPool<GameObject>(null, null);
    //private List<GameObject> objects = new List<GameObject>();
    //// vfx를 불러올때 선별해서 골라와야하기 때문에 Dic 선언
    //private Dictionary <string, List<GameObject>> vfxDic = new Dictionary<string, List<GameObject>>();
    //// 추가용
    //private Dictionary<string, GameObject> addVFXDic = new Dictionary<string, GameObject>();

    [SerializeField] private List<VFXData> vfxDatas = new List<VFXData>();

    private Dictionary<string, Queue<GameObject>> vfxDic = new Dictionary<string, Queue<GameObject>>();

    private VFX v;


    public void Start()
    {
        InstantiateVFX();
    }

    public void InstantiateVFX()
    {
        for(int i = 0; i< vfxDatas.Count; i++)
        {
            string name = vfxDatas[i].vfxName;
            Queue<GameObject> q = new Queue<GameObject>();
            vfxDic.Add(name, q );

            for (int j = 0; j < vfxDatas[i].count; j++)
            {
                GameObject vfx = Instantiate (vfxDatas[i].vfx);
                // 비활성화
                q.Enqueue (vfx);
                v = vfx.GetComponent<VFX> ();
                v.InitializePool(q);

            }
        }
    }

    public GameObject GetVFX(string name)
    {
        if (vfxDic.ContainsKey(name))
        {
            Queue<GameObject> q = vfxDic[name];

            if (q.Count <= 0)
            {
                //VFXData result = null;
                //for(int i =0; i< vfxDatas.Count; i++)
                //{
                //    if(vfxDatas[i].vfxName == name)
                //    {
                //        result = vfxDatas[i];
                //    }
                //}
                VFXData result = vfxDatas.Find(item => item.vfxName == name);
                if(result != null)
                {
                    GameObject obj = Instantiate(result.vfx);
                    q.Enqueue(obj);
                    v = obj.GetComponent<VFX>();
                    v.InitializePool(q);
                }
                else
                {
                    return null;
                }
            }

            return q.Dequeue();

        }
        return null;
    }
}
