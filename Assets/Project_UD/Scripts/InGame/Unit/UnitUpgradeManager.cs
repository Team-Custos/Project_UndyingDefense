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
        }

        if (UnitDataManager.inst.DoesUnitExist(option2))
        {
            upgradeOptions.Add(option2);
        }

        
        if (upgradeOptions.Count == 0)
        {
            return upgradeOptions; 
        }


        return upgradeOptions;
    }



<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
    // 업그레이드 실행
=======
=======
>>>>>>> parent of 98fb097 (Merge branch 'Release' of https://github.com/Team-Custos/Project_UndyingDefense into Release)
=======
>>>>>>> parent of 98fb097 (Merge branch 'Release' of https://github.com/Team-Custos/Project_UndyingDefense into Release)
=======
>>>>>>> parent of 98fb097 (Merge branch 'Release' of https://github.com/Team-Custos/Project_UndyingDefense into Release)

    // 실제 업그레이드를 수행하는 함수
>>>>>>> parent of 98fb097 (Merge branch 'Release' of https://github.com/Team-Custos/Project_UndyingDefense into Release)
=======
    // 업그레이드 실행
>>>>>>> parent of 48d20c1 (Merge branch 'LoPol' into Release)
=======
    // 업그레이드 실행
>>>>>>> parent of 48d20c1 (Merge branch 'LoPol' into Release)
    public void PerformUpgrade(Ingame_UnitCtrl selectedUnit, string unitCode)
    {
        // Dictionary에서 unitCode로 업그레이드된 유닛 데이터를 가져옴
        if (UnitDataManager.inst.unitDataDictionary.TryGetValue(unitCode, out UnitData upgradedUnitData))
        {
            // 유닛의 데이터를 새로운 데이터로 업데이트

            selectedUnit.unitData.g_SkillName = upgradedUnitData.g_SkillName;
            selectedUnit.unitData.s_SkillName = upgradedUnitData.s_SkillName;
            selectedUnit.unitData.unitCode = upgradedUnitData.UnitCode;

            selectedUnit.level = upgradedUnitData.Level;
            selectedUnit.cost = upgradedUnitData.Cost;
            selectedUnit.name = upgradedUnitData.Name;
            selectedUnit.unitCode = upgradedUnitData.UnitCode;
            selectedUnit.HP = upgradedUnitData.Hp;
            //selectedUnit.unitCode = upgradedUnitData.UnitCode;

            // UI 업데이트
            Ingame_UIManager.instance.UpdateUnitInfoPanel(selectedUnit, selectedUnit.unitCode);
        }
        else
        {
            Debug.LogError("업그레이드 실패");
        }
    }


}