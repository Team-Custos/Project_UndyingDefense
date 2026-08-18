using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSkill_FireOilCtrl : MonoBehaviour
{
    [SerializeField] private GameObject OilBottlePrefab;
    [SerializeField] private Transform[] OilBottlePos;
    [SerializeField] private GameObject fireArea;
    [SerializeField] private Transform fireAreaPos;
    [SerializeField] private AudioClip sfx;
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
        GenerateFireArea();
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

    private void GenerateFireArea()
    {
        GameObject fireAreaObject = Instantiate(fireArea, fireAreaPos.position, Quaternion.identity);
        Destroy(fireAreaObject, 5f);
    }


}
