using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using static UD_UnitDataManager;

public class UD_UnitDataManager : MonoBehaviour
{
    public enum UnitClass
    {
        Militiaman,
        Spearman,
        DolGyukByeong,
        Archer,
        Crossbowman,
        Hunter,
        Taoist,
        Shamanist,
        FengShui
    }


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

    private List<UnitData> unitDatas = new List<UnitData>();

    private Dictionary<UnitClass, UnitData> unitDataDictionary = new Dictionary<UnitClass, UnitData>();

    public void SetUnitData(List<UnitData> unitData)
    {
        unitDatas = unitData;
        ShowUnitData();
    }



    //public void SetUnitData(UnitClass unitClass, UnitData unitData)
    //{
    //    unitDataDictionary[unitClass] = unitData;
    //    Debug.Log($"Set data for {unitClass}: Name={unitData.Name}, Level={unitData.Level}, HP={unitData.HP}, Material={unitData.Material}");
    //}

    public UnitData GetUnitData(UnitClass unitClass)
    {
        if (unitDataDictionary.TryGetValue(unitClass, out UnitData unitData))
        {
            return unitData;
        }
        else
        {
            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    
    void ShowUnitData()
    {
        if (unitDatas.Count == 0)
        {
            return;
        }

        foreach (var unit in unitDatas)
        {
            Debug.Log($"Name: {unit.Name}, Type: {unit.Type}, Level: {unit.Level}, HP: {unit.HP}, Material: {unit.Material}");
        }
    }

    public UnitClass ParseUnitClass(string unitClass)
    {
        switch (unitClass)
        {
            case "민병":
                return UnitClass.Militiaman;
            case "창병":
                return UnitClass.Spearman;
            case "돌격병":
                return UnitClass.DolGyukByeong;
            case "궁수":
                return UnitClass.Archer;
            case "노병":
                return UnitClass.Crossbowman;
            case "사냥꾼":
                return UnitClass.Hunter;
            case "도사":
                return UnitClass.Taoist;
            case "주술도사":
                return UnitClass.Shamanist;
            case "풍수도사":
                return UnitClass.FengShui;
            default:
                return UnitClass.Militiaman;
        }
    }

}
