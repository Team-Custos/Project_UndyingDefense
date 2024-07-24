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
        public int Level;
        public int HP;
        public string Material;

        public UnitData(string name, string type, int level, int hp, string material)
        {
            Name = name;
            Type = type;
            Level = level;
            HP = hp;
            Material = material;
        }
    }


    private Dictionary<string, UnitData> unitDataDictionary = new Dictionary<string, UnitData>();


    void Start()
    {
        ShowUnitData();

        UnitData unit = GetUnitData("민병");
        if (unit != null)
        {
            Debug.Log($"Fetched Unit Data -> Name: {unit.Name}, Class: {unit.Type}, Level: {unit.Level}, HP: {unit.HP}, Material: {unit.Material}");
        }
    }

    public void SetUnitData(List<UnitData> unitDataList)
    {
        foreach(var  unitData in unitDataList)
        {
            unitDataDictionary[unitData.Name] = unitData;
            Debug.Log($"UnitName: {name}, Class: {unitData.Type}, Level: {unitData.Level}, HP: {unitData.HP}, Material: {unitData.Material}");
        }

        
    }


    public UnitData GetUnitData(string unitType)
    {
        if(unitDataDictionary.TryGetValue(unitType, out UnitData unitData))
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
        foreach(var unitdata in unitDataDictionary)
        {
            var unitType = unitdata.Key;
            var unitData = unitdata.Value;
            Debug.Log($"UnitName: {name}, Class: {unitData.Type}, Level: {unitData.Level}, HP: {unitData.HP}, Material: {unitData.Material}");
        }

    }

}
