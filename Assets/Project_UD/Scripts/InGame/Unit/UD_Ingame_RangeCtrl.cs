using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WSWhitehouse.TagSelector;

public class UD_Ingame_RangeCtrl : MonoBehaviour
{
    [TagSelector] public string tagToSearch;

    public float radius = 5;

    public List<GameObject> detectedObjects = new List<GameObject>();
    public List<GameObject> ignoreList = new List<GameObject>();

    public GameObject Obj_Nearest = null;
    



    // Update is called once per frame
    void Update()
    {
        GetComponent<SphereCollider>().radius = radius;
    }

    public GameObject NearestObjectSearch(float attackRange, bool isParentEnemy)
    {
        if (Obj_Nearest == null)
        {
            ListRefresh();
        }

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

            return Obj_Nearest;
        }
        else
        {
            return null;
        }

        
    }

    public void ListRefresh()
    {
        if (detectedObjects.Contains(Obj_Nearest))
        {
            detectedObjects.Remove(Obj_Nearest);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out UD_Ingame_UnitCtrl unit))
        {
            if (other.CompareTag(tagToSearch.ToString()))
            {
                detectedObjects.Add(unit.gameObject);
            }
            else return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.TryGetComponent(out UD_Ingame_UnitCtrl unit))
        {
            if (other.CompareTag(tagToSearch))
            {
                detectedObjects.Remove(unit.gameObject);
                if (Obj_Nearest == unit.gameObject)
                {
                    Obj_Nearest = null;
                }
                
            }
            else return;
        }
    }


}
