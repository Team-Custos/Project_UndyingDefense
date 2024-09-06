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
    public float moveSpeed;
    public int attackPoint;
    public float attackSpeed;
    public float skillCooldown;
    public int critChanceRate;

    public float sightRange;
    public float attackRange;

    public int generalSkill;
    public int specialSkill;
    public UnitType unitType;
	public DefenseType defenseType;
    public TargetSelectType targetSelectType;

    // LoPol추가
    public string unitID; // 나중에 int의 유닛 ID로 처리.
    public string unitName; 
    //public string defenseType; //enum으로 처리
    //public int critRate;
	//스킬 코드로 처리해야하는가 아님 string으로 처리해야하는가
    public string gSkillName; 
    public string sSkillName;
    public int cost;
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

        GameObject Obj;

        string unitID = GetUnitIDByType(unitType);

      
         //UD_UnitDataManager.UnitData unitData = UD_UnitDataManager.inst.GetUnitData(unitID);


        Obj = Instantiate(Test_Ally);
        Obj.transform.position = new Vector3(X, 0, Y);
        Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);
        Obj.GetComponent<UD_Ingame_UnitCtrl>().UnitInit(spawnData[unitType]); //엑셀 데이터는 spawnData쪽으로 불러온뒤 Unit Initial할때 spawnData에서.

        return Obj;
    }

    private string GetUnitIDByType(int unitType)
    {
        switch (unitType)
        {
            case 0:
                return "1";
            case 1:
                return "2";
            default:
                return "1";
        }
    }
}
