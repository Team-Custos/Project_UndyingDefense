using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSkill_FireOilCtrl : MonoBehaviour
{
    [SerializeField] private GameObject OilBottlePrefab;
    [SerializeField] private Transform[] OilBottlePos;
    private int oilBottleIdx = 0;

    [SerializeField] private float OilBottleWaitTime;
    private float OilBottleWaitTimeCheck = 0f;


    private bool isSpawned = true;
    private List<float> RandomNumExistList = new List<float>();

    public void SpawnStart()
    {
        OilBottleWaitTimeCheck = 0f;
        oilBottleIdx = 0;
        isSpawned = false;
    }


    private void Update()
    {
        if (!isSpawned)
        {
            if (OilBottleWaitTimeCheck > 0)
            {
                OilBottleWaitTimeCheck -= Time.deltaTime;
            }
            else
            {
                OilBottleWaitTimeCheck = OilBottleWaitTime;
                SpawnOilBottles(OilBottlePos[oilBottleIdx]);
                oilBottleIdx++;
                if (oilBottleIdx >= OilBottlePos.Length)
                {
                    oilBottleIdx = 0;
                    isSpawned = true;
                }
            }
        }
    }
    private void SpawnOilBottles(Transform Pos)
    {
        GameObject bottleObject 
            = Instantiate(OilBottlePrefab, Pos.position, Quaternion.identity);
        Destroy(bottleObject, 2f);
    }



}
