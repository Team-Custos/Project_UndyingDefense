using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitExcelDataManager;

public class UnitExcelDataManager : MonoBehaviour
{
    public static UnitExcelDataManager inst;

    void Awake()
    {
        inst = this;
    }

    public class UnitExcelData
    {
        public string unitCode;
        public string name;
        public int level;
        public int cost;
        public string g_SkillName;
        public string s_SkillName;
        public string g_SkillInfo;
        public string s_SkillInfo;

        public UnitExcelData(string UnitCode, string Name, int Level, int Cost, 
                        string G_skillName, string S_skillName, string G_skillInfo, string S_SkillInfo)
        {
            unitCode = UnitCode;
            name = Name;
            level = Level;
            cost = Cost;
            g_SkillName = G_skillName;
            s_SkillName = S_skillName;
            g_SkillInfo = G_skillInfo;
            s_SkillInfo = S_SkillInfo;

        }
    }


    public Dictionary<string, UnitExcelData> unitDataDictionary = new Dictionary<string, UnitExcelData>();



    public void SetUnitData(List<UnitExcelData> unitExcelDataList)
    {
        foreach (var unitExcelData in unitExcelDataList)
        {
            // 엑셀 데이터를 딕셔너리에 저장
            unitDataDictionary[unitExcelData.unitCode] = unitExcelData;

        }

        //ShowUnitData();
    }


    public UnitExcelData GetUnitData(string unitCode)
    {

        if (unitDataDictionary.TryGetValue(unitCode, out UnitExcelData unitData))
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
        foreach (var unitExcelDataDictionary in unitDataDictionary)
        {
            var unitCode = unitExcelDataDictionary.Key;
            var unitData = unitExcelDataDictionary.Value;
            Debug.Log($"Code : {unitCode},Name : {unitData.name}, Level : {unitData.level}, Cost :{unitData.cost} " +
                      $"g_Skill : {unitData.g_SkillName}, g_SkillName : {unitData.g_SkillInfo} s_Skill : {unitData.s_SkillName}," +
                      $"s_SkillName : {unitData.s_SkillInfo}");
        }

    }




}