using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_GameManager : MonoBehaviour
{
    public static UD_Ingame_GameManager inst;
    public UD_Ingame_GridManager gridManager;

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
        if (UnitSetMode && AllyUnitSetMode)
        {
            // 타일 색상 업데이트
            UD_Ingame_GridTile[] allTiles = FindObjectsOfType<UD_Ingame_GridTile>();
            foreach (var tile in allTiles)
            {
                tile.ShowPlacementColors(true);
            }
        }
        else
        {
            // 타일 색상 업데이트
            UD_Ingame_GridTile[] allTiles = FindObjectsOfType<UD_Ingame_GridTile>();
            foreach (var tile in allTiles)
            {
                tile.ShowPlacementColors(false);
            }
        }


    }

    

    
}
