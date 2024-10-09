using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
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
    public string unitCode;
    public int level;
    public float globalTime;
    public int mental;

    public void InitFromUnitData(UD_UnitDataManager.UnitData unitData)
    {
        unitCode = unitData.UnitCode;
        unitName = unitData.Name;
        level = unitData.Level;
        HP = unitData.Hp;
        attackSpeed = unitData.AttackSpeed;
        moveSpeed = unitData.MoveSpeed;
        globalTime = unitData.GlobalTime;
        mental = unitData.Mental;
        sightRange = unitData.SightRange;
        attackRange = unitData.AttackRange;
        critChanceRate = unitData.CritRate;
        gSkillName = unitData.g_SkillName;
        sSkillName = unitData.s_SkillName;
        generalSkill = unitData.g_Skil;
        specialSkill = unitData.s_Skill;
        skillCooldown = unitData.GlobalTime;



    }
}



public enum UnitType
{
    Warrior,
    Archer,
}


public class UnitSpawnManager : MonoBehaviour
{
    public List<Ingame_UnitData> unitDatas;
    [SerializeField]
    private GameObject unitPrefab;

    public static UnitSpawnManager inst;

    public UnitSpawnData[] spawnData;

    public GameObject Test_Ally;

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

    }

    //유닛 소환
    public GameObject UnitSpawn(int unitType,float X, float Y)
    {
        GameObject Obj;

        string unitCode = GetUnitIDByType(unitType);

        UnitSpawnData spawnData = GetUnitSpawnData(unitCode);
        Obj = Instantiate(Test_Ally);
        Obj.transform.position = new Vector3(X, 0, Y);
        //Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);
        Obj.GetComponent<Ingame_UnitCtrl>().unitData = unitDatas[unitType];

        if (spawnData != null)
        {
            Obj = Instantiate(Test_Ally);
            Obj.transform.position = new Vector3(X, 0, Y);
            Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);

            Obj.GetComponent<UD_Ingame_UnitCtrl>().UnitInit(spawnData);

            return Obj;
        }

        return null;
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

    public UnitSpawnData GetUnitSpawnData(string unitCode)
    {
        UD_UnitDataManager.UnitData unitData = UD_UnitDataManager.inst.GetUnitData(unitCode);

        if(unitData != null)
        {
            UnitSpawnData spawnData = new UnitSpawnData();

            spawnData.InitFromUnitData(unitData);
            return spawnData;
        }

        return null;
        
    }
}
