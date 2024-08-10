using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]

public class UnitSpawnData
{
    //public Mesh Tier2Mesh;
    //public Mesh Tier3Mesh;

    //public Material Tier2Material;
    //public Material Tier3Material;

    public int modelType;
    public float spawnTime;
    public int HP;
    public float speed;
    public int atk;
    public float sightRange;
    public float attackRange;

    public int generalSkill;
    public int specialSkill;
    public UnitType unitType;
}



public enum UnitType
{
    Warrior,
    Archer,
}


public class UD_Ingame_UnitSpawnManager : MonoBehaviour
{
    UD_Ingame_GridManager GRIDMANAGER;
    public static UD_Ingame_UnitSpawnManager inst;

    public int unitType = 0;
    public UnitSpawnData[] spawnData;

    public GameObject Test_Ally;

    // Start is called before the first frame update
    void Start()
    {
        GRIDMANAGER = UD_Ingame_GameManager.inst.gridManager;
        inst = this;

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //¿Ø¥÷ º“»Ø
    public GameObject UnitSpawn(float X, float Y)
    {
        GameObject Obj = null;

        Debug.Log(new Vector3(X, 0, Y));


        Obj = Instantiate(Test_Ally);
        Obj.transform.position = new Vector3(X, 0, Y);
        Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2 ((int)(X/2), (int)(Y/2));
        Obj.GetComponent<UD_Ingame_UnitCtrl>().UnitInit(spawnData[unitType]);

        return Obj;
    }
}
