using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuUI : MonoBehaviour
{
    [SerializeField] private SelectedUnitManager selectedUnitManager;

    [Header(" ■ 선택된 유닛")]
    [SerializeField] private Image selectedUnitBackImage;
    [SerializeField] private Image selectedUnitImage;
    [SerializeField] private Text selectedUnitNameText;
    [SerializeField] private Image selectedUnitAtTypeImage;
    [SerializeField] private Image selectedUnitDfTypeImage;

    [Header(" ■ 첫번째 업그레이드 유닛")]
    [SerializeField] private Image firstUpgradeUnitBackImage;
    [SerializeField] private Image firstUpgradeUnitImage;
    [SerializeField] private Button firstUpgradeBtn;
    [SerializeField] private Text firstUpgradeUnitNameText;
    [SerializeField] private Image firstUpgradeUnitAtTypeImage;
    [SerializeField] private Image firstUpgradeUnitDfTypeImage;

    [Header(" ■ 두번째 업그레이드 유닛")]
    [SerializeField] private Image secondUpgradeUnitBackImage;
    [SerializeField] private Button secondUpgradeBtn;
    [SerializeField] private Image secondUpgradeUnitImage;
    [SerializeField] private Text secondUpgradeUnitNameText;
    [SerializeField] private Image secondUpgradeUnitAtTypeImage;
    [SerializeField] private Image secondUpgradeUnitDfTypeImage;

    private UnitData firstUnitData;
    private UnitData secondUnitData;



    [SerializeField] private Text upgradeCostTxt;
    [SerializeField] private Button upgradePerformBtn;

    [SerializeField] private Sprite[] unitBackImage;

    private int upgradeIndex = -1;

    //public void UpgradeToFirstUnit()
    //{
    //    selectedUnitManager.UpgradeSelectedUnit(0);
    //}

    //public void UpgradeToSecondUnit()
    //{
    //    selectedUnitManager.UpgradeSelectedUnit(1);
    //}

    public void ToggleUpgradeUnit(int index)
    {

        if (upgradeIndex == index)
        {
            upgradeIndex = -1;
            upgradePerformBtn.interactable = false;
        }
        else
        {
            upgradeIndex = index;
            upgradePerformBtn.interactable = true;
            Debug.Log($"업그레이드 인덱스 선택됨: {upgradeIndex}");
        }

    }

    public void PerformUpgrade()
    {
        if (upgradeIndex == -1)
            return;
        selectedUnitManager.UpgradeSelectedUnit(upgradeIndex);
    }

    public void SetUnitUpgradeMenu(Unit selectedUnit)
    {
        upgradePerformBtn.interactable = false;

        // 현재 선택된 유닛
        selectedUnitImage.sprite = selectedUnit.Data.Icon;
        selectedUnitNameText.text = selectedUnit.Data.Name;
        selectedUnitAtTypeImage.sprite = selectedUnit.Data.AtTypeIcon;
        selectedUnitDfTypeImage.sprite = selectedUnit.Data.DfTypeIcon;

        AllyUnitData allyUnitData = (AllyUnitData)selectedUnit.Data;

        UnitData firstUpgradeUnitData = allyUnitData.UpgradeUnits[0];
        UnitData secondUpgradeUnitData = allyUnitData.UpgradeUnits[1];

        firstUnitData = firstUpgradeUnitData;
        secondUnitData = secondUpgradeUnitData;

        if (firstUpgradeUnitData != null)
        {
            // 첫번째 업그레이드 유닛
            firstUpgradeUnitImage.sprite = firstUpgradeUnitData.Icon;
            firstUpgradeUnitNameText.text = firstUpgradeUnitData.Name;
            firstUpgradeUnitAtTypeImage.sprite = firstUpgradeUnitData.AtTypeIcon;
            firstUpgradeUnitDfTypeImage.sprite = firstUpgradeUnitData.DfTypeIcon;
        }
        
        if(secondUpgradeUnitData != null)
        {
            // 두번째 업그레이드 유닛
            secondUpgradeUnitImage.sprite = secondUpgradeUnitData.Icon;
            secondUpgradeUnitNameText.text = secondUpgradeUnitData.Name;
            secondUpgradeUnitAtTypeImage.sprite = secondUpgradeUnitData.AtTypeIcon;
            secondUpgradeUnitDfTypeImage.sprite = secondUpgradeUnitData.DfTypeIcon;
        }

        

        AllyUnitData upgradeUnitData = (AllyUnitData)firstUpgradeUnitData;

        // 승급 비용
        upgradeCostTxt.text = upgradeUnitData.Cost.ToString();

        SetUnitBackImage(selectedUnit);
    }

    private void SetUnitBackImage(Unit selectedUnit)
    {
        if(selectedUnit.Data.Tier == 1)
        {
            selectedUnitBackImage.sprite = unitBackImage[0];
            firstUpgradeUnitBackImage.sprite = unitBackImage[1];
            secondUpgradeUnitBackImage.sprite = unitBackImage[1];
        }
        else if (selectedUnit.Data.Tier == 2)
        {
            selectedUnitBackImage.sprite = unitBackImage[1];
            firstUpgradeUnitBackImage.sprite = unitBackImage[2];
            secondUpgradeUnitBackImage.sprite = unitBackImage[2];
        }
        else if(selectedUnit.Data.Tier == 3)
        {
            selectedUnitBackImage.sprite = unitBackImage[2];
            firstUpgradeUnitBackImage.sprite = unitBackImage[3];
            secondUpgradeUnitBackImage.sprite = unitBackImage[3];
        }
    }
}
