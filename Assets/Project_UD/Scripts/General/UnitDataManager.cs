using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitDataManager;

public class UnitDataManager : MonoBehaviour
{
    public static UnitDataManager inst;

    void Awake()
    {
        inst = this;
    }

    public class UnitData
    {
        public int Number;
        public string ID;
        public string Name;
        public int Level;
        public int Cost;
        public int Hp;
        public int AttackSpeed;
        public string DefenseType;
        public float GlobalTime;
        public int Mental;
        public int MoveSpeed;
        public int SightRange;
        public int AttackRange;
        public string TargetSelectType;
        public int CritRate;
        public int g_Skil;
        public string g_SkillName;
        public int s_Skill;
        public string s_SkillName;
        public string UnitCode;

        public UnitData(int number, string id, string name, int level, int cost, int hp, int attackSpeed, string defenseType, float globalTime, int mental, int moveSpeed,
                        int sightRange, int attackRange, string targetSelectType, int critRate, int g_skill, string g_skillName, int s_skill, string s_skillName, string unitCdoe)
        {
            Number = number;
            ID = id;
            Name = name;
            Level = level;
            Cost = cost;
            Hp = hp;
            AttackSpeed = attackSpeed;
            DefenseType = defenseType;
            GlobalTime = globalTime;
            Mental = mental;
            MoveSpeed = moveSpeed;
            SightRange = sightRange;
            AttackRange = attackRange;
            TargetSelectType = targetSelectType;
            CritRate = critRate;
            g_Skil = g_skill;
            g_SkillName = g_skillName;
            s_Skill = s_skill;
            s_SkillName = s_skillName;
            UnitCode = unitCdoe;
        }
    }


    public Dictionary<string, UnitData> unitDataDictionary = new Dictionary<string, UnitData>();



    public void SetUnitData(List<UnitData> unitDataList)
    {
        foreach (var unitData in unitDataList)
        {
            // 엑셀 데이터를 딕셔너리에 저장
            unitDataDictionary[unitData.UnitCode] = unitData;

            // 스크립터블 오브젝트로 매핑
            Ingame_UnitData ingameUnitData = CreateUnitDataObject(unitData);
        }
    }


    public UnitData GetUnitData(string unitCode)
    {

        if (unitDataDictionary.TryGetValue(unitCode, out UnitData unitData))
        {
            return unitData;
        }
        else
        {
            Debug.Log("데이터 없음");
            return null;
        }
    }

    public bool DoesUnitExist(string unitCode)
    {
        return unitDataDictionary.ContainsKey(unitCode);
    }

    public void ShowUnitData()
    {
        foreach (var unitdata in unitDataDictionary)
        {
            var unitCode = unitdata.Key;
            var unitData = unitdata.Value;
            Debug.Log($"Code : {unitCode}, Number : {unitData.Number}, ID : {unitData.ID} , Name : {unitData.Name}, Level : {unitData.Level}, Cost :{unitData.Cost} " +
                      $"HP : {unitData.Hp}, AttackSpeed : {unitData.AttackSpeed}, DefenseType : {unitData.DefenseType}, GlobalTime : {unitData.GlobalTime} " +
                      $"Mental : {unitData.Mental}, MoveSpeed : {unitData.MoveSpeed}, SightRange : {unitData.SightRange},AttackRange : {unitData.AttackRange} " +
                      $"TargetSelectType : {unitData.TargetSelectType}, crtiRate : {unitData.CritRate}, g_Skill : {unitData.g_Skil}, g_SkillName : {unitData.g_SkillName}" +
                      $"s_Skill : {unitData.s_Skill}, s_SkillName : {unitData.s_SkillName}");

        }

    }


    // 엑셀 데이터를 스크립터블 오브젝트에 매핑하는 함수
    public Ingame_UnitData CreateUnitDataObject(UnitData unitData)
    {
        Ingame_UnitData ingameUnitData = ScriptableObject.CreateInstance<Ingame_UnitData>();

        // 매핑
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
        ingameUnitData.unitType = (UnitType)System.Enum.Parse(typeof(UnitType), unitData.ID);
        ingameUnitData.defenseType = (DefenseType)System.Enum.Parse(typeof(DefenseType), unitData.DefenseType);
        ingameUnitData.targetSelectType = (TargetSelectType)System.Enum.Parse(typeof(TargetSelectType), unitData.TargetSelectType);
=======
=======
>>>>>>> parent of 98fb097 (Merge branch 'Release' of https://github.com/Team-Custos/Project_UndyingDefense into Release)
        //ingameUnitData.unitType = (UnitType)System.Enum.Parse(typeof(UnitType), unitData.ID);
        //ingameUnitData.defenseType = (DefenseType)System.Enum.Parse(typeof(DefenseType), unitData.DefenseType);
        //ingameUnitData.targetSelectType = (TargetSelectType)System.Enum.Parse(typeof(TargetSelectType), unitData.TargetSelectType);
>>>>>>> parent of 98fb097 (Merge branch 'Release' of https://github.com/Team-Custos/Project_UndyingDefense into Release)
=======
        ingameUnitData.unitType = (UnitType)System.Enum.Parse(typeof(UnitType), unitData.ID);
        ingameUnitData.defenseType = (DefenseType)System.Enum.Parse(typeof(DefenseType), unitData.DefenseType);
        ingameUnitData.targetSelectType = (TargetSelectType)System.Enum.Parse(typeof(TargetSelectType), unitData.TargetSelectType);
>>>>>>> parent of 48d20c1 (Merge branch 'LoPol' into Release)
        //ingameUnitData.modelType = unitData.Number;
        ingameUnitData.maxHP = unitData.Hp;
        ingameUnitData.mental = unitData.Mental;
        ingameUnitData.moveSpeed = unitData.MoveSpeed;
        //ingameUnitData.attackPoint = unitData.AttackSpeed;
        ingameUnitData.critChanceRate = unitData.CritRate;
        ingameUnitData.generalSkillCode = unitData.g_Skil;
        ingameUnitData.specialSkillCode = unitData.s_Skill;
        ingameUnitData.weaponCooldown = unitData.GlobalTime;
        ingameUnitData.sightRange = unitData.SightRange;
        ingameUnitData.attackRange = unitData.AttackRange;
        ingameUnitData.unitCode = unitData.UnitCode;
        ingameUnitData.s_SkillName = unitData.s_SkillName;
        ingameUnitData.g_SkillName = unitData.g_SkillName;
        ingameUnitData.level = unitData.Level;
        ingameUnitData.cost = unitData.Cost;
        ingameUnitData.name = unitData.Name;

        return ingameUnitData;
    }


}
