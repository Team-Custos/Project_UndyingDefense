using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMenuUI : MonoBehaviour
{
    [SerializeField] private Button modeChangeBtn;
    [SerializeField] private Button upgradeBtn;

    private AllyUnit allyUnit;
    public void PerformUpgrade(AllyUnit unit)
    {
        upgradeBtn.onClick.RemoveAllListeners();
        upgradeBtn.onClick.AddListener(() =>
        {
            Debug.Log("업그레이드");
            unit.Upgrade();
        });
    }

    public void PerformModeChange(AllyUnit unit)
    {
        modeChangeBtn.onClick.RemoveAllListeners(); // 기존 이벤트 제거
        modeChangeBtn.onClick.AddListener(() =>
        {
            Debug.Log("모드 변경");
            unit.ChangeMode(AllyUnit.Mode.CHANGE);
        });
    }
}
