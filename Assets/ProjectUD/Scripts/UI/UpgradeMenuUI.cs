using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuUI : MonoBehaviour
{
    [SerializeField] private SelectedUnitManager selectedUnitManager;

    [SerializeField] private Button FirstUpgradeBtn;
    [SerializeField] private Button SecondUpgradeBtn;

    public void UpgradeFirstIndex()
    {
        selectedUnitManager.UpgradeSelectedUnit(0);
    }

    public void UpgradeSecondIndex()
    {
        selectedUnitManager.UpgradeSelectedUnit(1);
    }
}
