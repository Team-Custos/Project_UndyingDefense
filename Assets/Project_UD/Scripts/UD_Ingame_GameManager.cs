using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_GameManager : MonoBehaviour
{
    public static UD_Ingame_GameManager inst;
    public UD_Ingame_GridManager gridManager;

    public GameObject[] Unit;
    public GameObject[] Enemy;

    public GameObject Base;
   

    public bool UnitSetMode = false;
    public bool AllyUnitSetMode = false;
    public bool EnemyUnitSetMode = false;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UnitSetMode = !UnitSetMode;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (AllyUnitSetMode)
            {
                AllyUnitSetMode = false;
                EnemyUnitSetMode = true;
            }
            else if (EnemyUnitSetMode)
            {
                EnemyUnitSetMode = false;
                AllyUnitSetMode = true;
            }
        }


    }

    
}
