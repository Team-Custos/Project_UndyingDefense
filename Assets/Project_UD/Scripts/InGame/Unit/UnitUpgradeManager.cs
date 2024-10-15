using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnitDataManager;

public class UnitUpgradeManager : MonoBehaviour
{

    public List<string> GetUpgradeOptions(string unitCode)
    {
        List<string> upgradeOptions = new List<string>();

        string option1 = unitCode + "1";
        string option2 = unitCode + "2";

        if (UnitDataManager.inst.DoesUnitExist(option1))
        {
            upgradeOptions.Add(option1);
            Debug.Log($"Option 1: {option1} 추가됨");
        }

        if (UnitDataManager.inst.DoesUnitExist(option2))
        {
            upgradeOptions.Add(option2);
            Debug.Log($"Option 2: {option2} 추가됨");
        }

        if (upgradeOptions.Count == 0)
        {
            Debug.LogError("업그레이드 가능한 옵션이 없습니다.");
        }

        return upgradeOptions;
    }



    // 업그레이드 실행
    public void PerformUpgrade(Ingame_UnitCtrl selectedUnit, string unitCode)
    {
        // UnitDataManager에서 업그레이드된 유닛 데이터를 가져옴
        UnitData upgradedUnitData = UnitDataManager.inst.GetUnitData(unitCode);

        if (upgradedUnitData != null)
        {
            // 유닛의 데이터를 새로운 데이터로 업데이트
            selectedUnit.unitData.maxHP = upgradedUnitData.Hp;
            selectedUnit.unitData.critChanceRate = upgradedUnitData.CritRate;
            selectedUnit.unitData.generalSkillCode = upgradedUnitData.g_Skil;
            selectedUnit.unitData.specialSkillCode = upgradedUnitData.s_Skill;
            selectedUnit.unitData.moveSpeed = upgradedUnitData.MoveSpeed;
            selectedUnit.unitData.sightRange = upgradedUnitData.SightRange;
            selectedUnit.unitData.attackRange = upgradedUnitData.AttackRange;
            selectedUnit.unitData.g_SkillName = upgradedUnitData.g_SkillName;
            selectedUnit.unitData.s_SkillName = upgradedUnitData.s_SkillName;
            selectedUnit.unitData.level = upgradedUnitData.Level;
            selectedUnit.unitData.cost = upgradedUnitData.Cost;
            selectedUnit.unitData.name = upgradedUnitData.Name;

            selectedUnit.HP = upgradedUnitData.Hp;

            Debug.Log($"{selectedUnit.unitName} 유닛이 {upgradedUnitData.Name}으로 업그레이드되었습니다.");

            // UI를 업데이트하여 새 데이터를 반영
            Ingame_UIManager.instance.UpdateUnitInfoPanel(selectedUnit, unitCode);
        }
        else
        {
            Debug.LogError("업그레이드 실패: 업그레이드할 유닛 데이터를 찾을 수 없습니다.");
        }
    }


}
