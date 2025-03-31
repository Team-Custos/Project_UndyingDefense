using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMenuUI : MonoBehaviour
{
    [SerializeField] private Button modeChangeBtn;
    [SerializeField] private Button upgradeBtn;

    //public void PerformUpgrade(AllyUnit allyUnit, AllyUnitData allyUnitData, int index)
    //{
    //    upgradeBtn.onClick.RemoveAllListeners();
    //    upgradeBtn.onClick.AddListener(() =>
    //    {
    //        Debug.Log("업그레이드");
    //        //allyUnit.Upgrade(allyUnitData, index);
    //        gameObject.SetActive(false);
    //    });
    //}

    //public void PerformModeChange(AllyUnit unit)
    //{
    //    modeChangeBtn.onClick.RemoveAllListeners(); // 기존 이벤트 제거
    //    modeChangeBtn.onClick.AddListener(() =>
    //    {
    //        Debug.Log("모드 변경");
    //        unit.ChangeMode(AllyUnit.Mode.CHANGE);
    //        gameObject.SetActive(false);
    //    });
    //}
}
