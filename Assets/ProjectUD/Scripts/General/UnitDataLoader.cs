using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats   // 엑셀로 불러온 유닛 데이터 클래스
{
    public string id;
    public int cost;
    public int tier;
    public string unitName;
    public float maxHp;
    public float moveSpeed;
    public float attackSpeed;
    public float sightRange;
    public float attackRange;
    public float mental;
    public float critChance;
    public string role;
    public float interval;

}

public class UnitDataLoader : MonoBehaviour
{
    [SerializeField] private TextAsset unitDataTable;   // 유닛 데이터가 저장된 CSV 파일

    private Dictionary<string, UnitStats> unitDataDictionary = new Dictionary<string, UnitStats>();     // 유닛 데이터 딕셔너리
    public Dictionary<string, UnitStats> UnitDataDictionary => unitDataDictionary;

    private void Start()
    {
        LoadUnitData();
    }

    private void LoadUnitData()
    {
        if (unitDataTable == null)
        {
            Debug.Log("데이터 없음");
            return;
        }

        string[] lines = unitDataTable.text.Split('\n');

        for(int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            string[] values = line.Split(',');

            if (string.IsNullOrWhiteSpace(line)) continue;

            UnitStats unitStats = new UnitStats
            {
                id = values[0],                                                            
                unitName = values[1],                                                      
                tier = int.Parse(values[2]),                                               
                maxHp = float.Parse(values[3]),                                              
                cost = int.Parse(values[4]),                                               
                moveSpeed = float.Parse(values[5]),                                        
                attackSpeed = float.Parse(values[6]),
                sightRange = float.Parse(values[7]),
                attackRange = float.Parse(values[8]),
                mental = float.Parse(values[9]),
                critChance = float.Parse(values[10]),
                role = values[11],
                interval = float.Parse(values[12])
            };

            unitDataDictionary.Add(unitStats.id, unitStats);

            
        }
    }

    public UnitStats GetUnitDataById(string id, Unit unit)
    {
        unit.SetUnitDataLoader(this);

        if (unitDataDictionary.ContainsKey(id))
            return unitDataDictionary[id];
        else
        {
            Debug.Log("해당 아이디 없음");
            return null;
        }
            
    }
}