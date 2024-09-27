using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitDebuffData
{
    public UnitDebuff name;
    public int stackLimit;
    public float Time;
}

public class UnitStatusCtrl : MonoBehaviour
{

    public UnitDebuffData[] debuffDatas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
