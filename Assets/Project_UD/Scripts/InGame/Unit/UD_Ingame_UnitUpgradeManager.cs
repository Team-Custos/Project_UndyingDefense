using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UD_UnitDataManager;

public class UD_Ingame_UnitUpgradeManager : MonoBehaviour
{
    private Ingame_UIManager uiManager; // UI와의 상호작용을 위해


    // Start is called before the first frame update
    void Start()
    {
        uiManager = Ingame_UIManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<string> GetUpgradeOptions(string unitCode)
    {
        List<string> upgradeOptions = new List<string>();

        // 업그레이드 로직 : ID 뒤에 1 또는 2를 추가
        string option1 = unitCode + "1";
        string option2 = unitCode + "2";

        if(UD_UnitDataManager.inst.DoesUnitExist(option1))
        {
            upgradeOptions.Add(option1);
        }
        
        if(UD_UnitDataManager.inst.DoesUnitExist(option2))
        {
            upgradeOptions.Add(option2);
        }

        if(upgradeOptions.Count == 0)
        {
            Debug.Log("업그레이드 없음");
        }

        return upgradeOptions;
    }

    //public void PerformUpgrade(Ingame_UnitCtrl selectedUnit, string newUnitCode)
    //{
    //    //UnitData newUnitData = UD_UnitDataManager.inst.GetUnitData(newUnitCode);
    //    UnitSpawnData newUnitData = Ingame_UnitSpawnManager.inst.GetUnitSpawnData(newUnitCode);

    //    if(newUnitCode != null)
    //    {
    //        selectedUnit.UnitInit(newUnitData);
    //        Debug.Log(selectedUnit.unitName + "업그레이드 완료");

    //        uiManager.UpdateUnitInfoPanel(selectedUnit, selectedUnit.unitCode);
    //    }
    //    else
    //    {
    //        Debug.Log("유닛 데이터 없음");
    //    }
    //}


}
