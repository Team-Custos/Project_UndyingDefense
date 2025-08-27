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
        factionDic.Add("moor", Resources.LoadAll<UnitData>("UnitData/Enemy/Level1/moorArchive"));
        factionDic.Add("pioneer", Resources.LoadAll<UnitData>("UnitData/Enemy/Level1/pioneerArchive"));
        factionDic.Add("summon", Resources.LoadAll<UnitData>("UnitData/Enemy/Level1/summonArchive"));

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
