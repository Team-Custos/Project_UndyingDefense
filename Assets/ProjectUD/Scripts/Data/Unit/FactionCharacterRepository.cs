using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionCharacterRepository : MonoBehaviour
{
    private UnitData[] allySO;
    private UnitData[] moorSO;
    private UnitData[] pioneerSO;
    private UnitData[] summonSO;

    private List<UnitData[]> factions;

    private Dictionary<string, UnitData[]> factionDic;

    private void Start()
    {
        allySO = Resources.LoadAll<UnitData>("UnitData/Ally/AllyArchive");
        moorSO = Resources.LoadAll<UnitData>("UnitData/Enemy/Level1/moorArchive");
        pioneerSO = Resources.LoadAll<UnitData>("UnitData/Enemy/Level1/pioneerArchive");
        summonSO = Resources.LoadAll<UnitData>("UnitData/Enemy/Level1/summonArchive");

        factions.Add(allySO);
        factions.Add(moorSO);
        factions.Add(pioneerSO);
        factions.Add(summonSO);
    }

    public void SetRosterDic()
    {
        for (int i = 0; i < factions.Count; i++)
        {
            string faction = factions[i].ToString();    // 이거 아님 고쳐야함
            //factionDic.Add();
        }
    }
}
