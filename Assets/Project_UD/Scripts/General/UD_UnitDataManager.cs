using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UD_UnitDataManager;

public class UD_UnitDataManager : MonoBehaviour
{
    public static UD_UnitDataManager inst;

    void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(gameObject);
        }
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
        public string g_Skil;
        public string g_SkillName;
        public string s_Skill;
        public string s_SkillName;

        public UnitData(int number, string id, string name, int level, int cost, int hp, int attackSpeed, string defenseType, float globalTime, int mental, int moveSpeed,
                        int sightRange, int attackRange, string targetSelectType, int critRate, string g_skill, string g_skillName, string s_skill, string s_skillName)
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
        }
    }


    public Dictionary<string, UnitData> unitDataDictionary = new Dictionary<string, UnitData>();


    void Start()
    {

    }

    public void SetUnitData(List<UnitData> unitDataList)
    {
        foreach (var unitData in unitDataList)
        {
            unitDataDictionary[unitData.Name] = unitData;
        }

        //ShowUnitData();
    }


    public UnitData GetUnitData(string unitName)
    {
        if (unitDataDictionary.TryGetValue(unitName, out UnitData unitData))
        {
            return unitData;
        }
        else
        {
            Debug.Log("데이터 없음");
            return null;
        }
    }


    public void ShowUnitData()
    {
        foreach (var unitdata in unitDataDictionary)
        {
            var unitName = unitdata.Key;
            var unitData = unitdata.Value;
            Debug.Log($"Name : {unitName}, Number : {unitData.Number}, ID : {unitData.ID}, Level : {unitData.Level}, Cost :{unitData.Cost} " +
                      $"HP : {unitData.Hp}, AttackSpeed : {unitData.AttackSpeed}, DefenseType : {unitData.DefenseType}, GlobalTime : {unitData.GlobalTime} " +
                      $"Mental : {unitData.Mental}, MoveSpeed : {unitData.MoveSpeed}, SightRange : {unitData.SightRange},AttackRange : {unitData.AttackRange} " +
                      $"TargetSelectType : {unitData.TargetSelectType}, crtiRate : {unitData.CritRate}, g_Skill : {unitData.g_Skil}, g_SkillName : {unitData.g_SkillName}" +
                      $"s_Skill : {unitData.s_Skill}, s_SkillName : {unitData.s_SkillName}");

        }

    }

    //public void AssignUnitData(GameObject unitObject, string unitName)
    //{
    //    if(unitDataDictionary.TryGetValue(unitName, out UnitData unitData))
    //    {
    //        UD_Ingame_UnitCtrl unitCtrl = unitObject.GetComponent<UD_Ingame_UnitCtrl>();

    //        if(unitCtrl != null)
    //        {
    //            unitCtrl.SetUnitData(unitData);
    //        }
    //    }
    //}

}
