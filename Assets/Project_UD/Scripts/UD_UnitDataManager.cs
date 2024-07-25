using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UD_UnitDataManager;

public class UD_UnitDataManager : MonoBehaviour
{

    public class UnitData
    {
        public string Name;
        public string Type;
        public int Tier;
        public string Weapon;
        public string SkillType;
        public string Skill;
        public string Description;
        public string AttackType;
        public int Damage;
        public int TargetCount;
        public string HitShape;
        public int CritRate;
        public string CritEffect1;
        public string CritEffect2;

        public UnitData(string name, string type, int tier, string weapon, string skillType, string skill, string description, string attackType, int damage, int targetCount, string hitShape, int critRate, string critEffect1, string critEffect2)
        {
            Name = name;
            Type = type;
            Tier = tier;
            Weapon = weapon;
            SkillType = skillType;
            Skill = skill;
            Description = description;
            AttackType = attackType;
            Damage = damage;
            TargetCount = targetCount;
            HitShape = hitShape;
            CritRate = critRate;
            CritEffect1 = critEffect1;
            CritEffect2 = critEffect2;
        }
    }


    private Dictionary<string, UnitData> unitDataDictionary = new Dictionary<string, UnitData>();


    void Start()
    {

    }

    public void SetUnitData(List<UnitData> unitDataList)
    {
        foreach (var unitData in unitDataList)
        {
            unitDataDictionary[unitData.Name] = unitData;
            Debug.Log($"Name: {unitData.Name}, Class: {unitData.Type}, Level: {unitData.Tier}, HP: {unitData.Damage}, Material: {unitData.Weapon}");
        }

        // 모든 유닛 데이터를 출력하는 메서드 호출
        ShowUnitData();
    }


    public UnitData GetUnitData(string unitType)
    {
        if (unitDataDictionary.TryGetValue(name, out UnitData unitData))
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
            Debug.Log($"Name: {unitName}, Class: {unitData.Type}, Tier: {unitData.Tier}, Weapon: {unitData.Weapon}, SkillType: {unitData.SkillType}, Skill: {unitData.Skill}, Description: {unitData.Description}, AttackType: {unitData.AttackType}, Damage: {unitData.Damage}, TargetCount: {unitData.TargetCount}, HitShape: {unitData.HitShape}, CritRate: {unitData.CritRate}, CritEffect1: {unitData.CritEffect1}, CritEffect2: {unitData.CritEffect2}");
        }

    }

}
