using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public static UD_Ingame_UnitSpawnManager inst;

    public UnitSpawnData[] spawnData;

    public GameObject Test_Ally;
    public GameObject Test_Enemy;

    public int unitToSpawn = 0;

    public Transform SpawnPos;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
    }
    // Update is called once per frame
    void Update()
    {
        //UD_Ingame_UnitCtrl[] allUnits = FindObjectsOfType<UD_Ingame_UnitCtrl>();
        //foreach (UD_Ingame_UnitCtrl unit in allUnits)
        //{
        //    if (unit.unitStateChangeTime > 0)
        //    {
        //        unit.unitStateChangeTime -= Time.deltaTime;
        //    }
        //}
    }

    //유닛 소환
    public GameObject UnitSpawn(int unitType,float X, float Y)
    {
        GameObject Obj = null;

        //Debug.Log(new Vector3(X, 0, Y));

        Obj = Instantiate(Test_Ally);
        Obj.transform.position = new Vector3(X, 0, Y);
        Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);
        Obj.GetComponent<UD_Ingame_UnitCtrl>().UnitInit(spawnData[unitType]);

        return Obj;
    }
}
