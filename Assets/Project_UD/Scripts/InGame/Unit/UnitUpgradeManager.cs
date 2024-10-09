using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnitDataManager;

public class UnitUpgradeManager : MonoBehaviour
{
    private Ingame_UIManager uiManager; // UI���� ��ȣ�ۿ��� ����


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

        // ���׷��̵� ���� : ID �ڿ� 1 �Ǵ� 2�� �߰�
        string option1 = unitCode + "1";
        string option2 = unitCode + "2";

        if(UnitDataManager.inst.DoesUnitExist(option1))
        {
            upgradeOptions.Add(option1);
        }
        
        if(UnitDataManager.inst.DoesUnitExist(option2))
        {
            upgradeOptions.Add(option2);
        }

        if(upgradeOptions.Count == 0)
        {
            Debug.Log("���׷��̵� ����");
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
    //        Debug.Log(selectedUnit.unitName + "���׷��̵� �Ϸ�");

    //        uiManager.UpdateUnitInfoPanel(selectedUnit, selectedUnit.unitCode);
    //    }
    //    else
    //    {
    //        Debug.Log("���� ������ ����");
    //    }
    //}


}
