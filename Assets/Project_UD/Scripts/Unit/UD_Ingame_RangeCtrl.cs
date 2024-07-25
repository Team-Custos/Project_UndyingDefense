using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UD_Ingame_RangeCtrl : MonoBehaviour
{
    Transform Parent;
    //public List<GameObject> ObjInRange = new List<GameObject>();
    public float radius = 5;

    public List<GameObject> detectedObjects = new List<GameObject>();
    public List<GameObject> ignoreList = new List<GameObject>();

    public GameObject Obj_Nearest = null;
    


    // Start is called before the first frame update
    void Awake()
    {
        Parent = GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SphereCollider>().radius = radius;
    }

    public GameObject NearestObjectSearch(float attackRange, bool isParentEnemy)
    {

        if (detectedObjects.Count > 0)
        {
            Obj_Nearest = detectedObjects[0];
            float Obj_Distance_Nearest = Vector3.Distance(Obj_Nearest.transform.position, transform.position);
            if (detectedObjects.Count > 1)
            {
                for (int i = 1; i < detectedObjects.Count - 1; i++)
                {
                    float Obj_Distance = Vector3.Distance(detectedObjects[i].transform.position, transform.position);

                    if (Obj_Distance_Nearest > Obj_Distance)
                    {
                        Obj_Nearest = detectedObjects[i].gameObject;
                    }
                }
            }

            if (Obj_Distance_Nearest <= attackRange)
            {
                return Obj_Nearest;
            }
            else
            { 
                return null;
            }
        }
        else
        {
            return null;
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out UD_Ingame_EnemyCtrl unit))
        {
            if (ignoreList.Exists(x => x.gameObject == other.transform.root))
                return;

            detectedObjects.Add(unit.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.TryGetComponent(out UD_Ingame_EnemyCtrl unit))
        {
            if (ignoreList.Exists(x => x.gameObject == other.transform.root))
                return;

            detectedObjects.Remove(unit.gameObject);
        }
    }


}
