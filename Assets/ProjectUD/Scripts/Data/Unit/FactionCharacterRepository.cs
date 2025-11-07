using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionCharacterRepository : MonoBehaviour
{
    private Dictionary<string, UnitData[]> factionDic = new Dictionary<string, UnitData[]>();

    private void Start()
    {
        SetFactionDic();
    }

    public void SetFactionDic()
    {
        factionDic.Add("ally", Resources.LoadAll<UnitData>("UnitData/Ally/AllyArchive"));
        factionDic.Add("moor", Resources.LoadAll<UnitData>("UnitData/Enemy/moor"));
        factionDic.Add("pioneer", Resources.LoadAll<UnitData>("UnitData/Enemy/pioneer"));
        factionDic.Add("summon", Resources.LoadAll<UnitData>("UnitData/Enemy/summon"));
        //factionDic.Add("empire", Resources.LoadAll<UnitData>("UnitData/Enemy/empire"));

    }

    public UnitData[] GetFactionArray(string fName)
    {
        return factionDic[fName];
    }

    public UnitData GetUnitData(string fName, int i)
    {
        return factionDic[fName][i];
    }
}
