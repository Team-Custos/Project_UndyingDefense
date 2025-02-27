using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static UnitDataManager;
using static UnitExcelDataManager;

public class UnitUpgradeManager : MonoBehaviour
{
    public static UnitUpgradeManager Instance;

    public List<Ingame_UnitData> allUnitData;  // 모든 유닛 데이터
    private Dictionary<string, Ingame_UnitData> unitMap;  // 코드 매핑

    public bool isUpgrade = false;

    private void Awake()
    {
        Instance = this;
        InitializeUnitMap();
    }


    // 모든 유닛 데이터를 unitCode 기반으로 매핑
    private void InitializeUnitMap()
    {
        unitMap = new Dictionary<string, Ingame_UnitData>();
        foreach (var unit in allUnitData)
        {
            if (!unitMap.ContainsKey(unit.unitCode))
            {
                unitMap[unit.unitCode] = unit;
            }
        }
    }


    // 업그레이드 수행
    public void PerformUpgrade(Ingame_UnitCtrl targetUnit, string upgradeOption)
    {
        isUpgrade = true;

        string upgradedCode = targetUnit.unitData.unitCode + upgradeOption;

        if (unitMap.TryGetValue(upgradedCode, out var nextUnitData))
        {
            targetUnit.SetUnitData(nextUnitData);
            targetUnit.ModelSwap();
            targetUnit.StatsInit();
            Debug.Log($"업그레이드 성공! 새로운 유닛: {nextUnitData.name}");
        }
        else
        {
            Debug.Log("업그레이드 가능한 데이터가 없습니다.");
        }


        Ingame_ParticleManager.Instance.UnitUpgradeEffect(targetUnit.transform);
        SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_upgrade);

        InGameManager.inst.gold -= nextUnitData.cost;

        isUpgrade = false;
    }
}