using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private List<UnitData> unitDatas = new List<UnitData>();

    public void SetUnitData(List<UnitData> unitData)
    {
        unitDatas = unitData;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        ShowUnitData();
    }

    
    void ShowUnitData()
    {
        foreach (var unit in unitDatas)
        {
            Debug.Log($"Name: {unit.Name}, Type: {unit.Type}, Level: {unit.Level}, HP: {unit.HP}, Material: {unit.Material}");
        }
    }
}
