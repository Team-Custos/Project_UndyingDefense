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

    public void SetFactionDic()
    {
        for (int i = 0; i < factions.Count; i++)
        {
            UnitData uData = factions[i][0];
            string faction = uData.CampName;    

            if(!factionDic.ContainsKey(faction))
            {
                factionDic.Add(faction, factions[i]);
            }
        }
    }
}
