using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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

    [Header(" ■ 업그레이드할 유니의 정보")]
    [SerializeField]private GameObject infoPanel;
    [SerializeField] private Text infoText;
    [SerializeField] private Image beforeHp;
    [SerializeField] private Image afterHp;
    [SerializeField] private Text infoHpText;
    [SerializeField] private Text infoCrtiText;
    [SerializeField] private Text infoMoveSpeedText;
    [SerializeField] private Text infoAttackSpeedText;
    [SerializeField] private Image infoGSkillImage;
    [SerializeField] private Text infoGSkillText;
    [SerializeField] private Image infoSSkillImage;
    [SerializeField] private Text infoSSkillText;



    private UnitData currentUnitData;
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
            infoPanel.SetActive(false);
            upgradePerformBtn.interactable = false;

        }
        else
        {
            infoPanel.SetActive(true);
            upgradeIndex = index;
            upgradePerformBtn.interactable = true;
            Debug.Log($"업그레이드 인덱스 선택됨: {upgradeIndex}");

            if(index == 0)
            {
                infoText.text = firstUnitData.Name;
                infoCrtiText.text = "치명타율 : " + firstUnitData.CritChance.ToString();
                infoMoveSpeedText.text = "이동속도 : " + firstUnitData.MoveSpeed.ToString();
                infoAttackSpeedText.text = "공격속도 : " + firstUnitData.AttackSpeed.ToString();
                infoHpText.text = currentUnitData.MaxHp.ToString() + " + " + (firstUnitData.MaxHp - currentUnitData.MaxHp).ToString();
                beforeHp.fillAmount = currentUnitData.MaxHp / 500; //firstUnitData.MaxHp;
                afterHp.fillAmount = firstUnitData.MaxHp / 500; // firstUnitData.MaxHp;


                //infoGSkillImage.sprite = firstUnitData.GeneralSkill.Icon;
                //infoGSkillText.text = secondUnitData.GSkillName;
                //infoSSkillImage.sprite = secondUnitData.SSkillIcon;
                //infoSSkillText.text = secondUnitData.SSkillName;
            }
            else if (index == 1)
            {
                infoText.text = secondUnitData.Name;
                infoCrtiText.text = "치명타율 : " + secondUnitData.CritChance.ToString();
                infoMoveSpeedText.text = "이동속도 : " + secondUnitData.MoveSpeed.ToString();
                infoAttackSpeedText.text = "공격속도 : " + secondUnitData.AttackSpeed.ToString();
                infoHpText.text = currentUnitData.MaxHp.ToString() + " + " + (secondUnitData.MaxHp - currentUnitData.MaxHp).ToString();
                beforeHp.fillAmount = currentUnitData.MaxHp / 500;// secondUnitData.MaxHp;
                afterHp.fillAmount = secondUnitData.MaxHp / 500; // secondUnitData.MaxHp;
            }
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
        upgradeIndex = -1;

        currentUnitData = selectedUnit.Data;

        upgradePerformBtn.interactable = false;

        // 현재 선택된 유닛
        selectedUnitImage.sprite = selectedUnit.Data.Icon;
        selectedUnitNameText.text = selectedUnit.Data.Name;
        selectedUnitAtTypeImage.sprite = selectedUnit.Data.AtTypeIcon;
        selectedUnitDfTypeImage.sprite = selectedUnit.Data.DfTypeIcon;

        AllyUnitData allyUnitData = (AllyUnitData)selectedUnit.Data;

        if(currentUnitData.Tier < 3)
        {
            secondUpgradeBtn.interactable = true;

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

            if (secondUpgradeUnitData != null)
            {
                // 두번째 업그레이드 유닛
                secondUpgradeUnitImage.sprite = secondUpgradeUnitData.Icon;
                secondUpgradeUnitNameText.text = secondUpgradeUnitData.Name;
                secondUpgradeUnitAtTypeImage.sprite = secondUpgradeUnitData.AtTypeIcon;
                secondUpgradeUnitDfTypeImage.sprite = secondUpgradeUnitData.DfTypeIcon;
            }
            AllyUnitData upgradeUnitData = (AllyUnitData)firstUpgradeUnitData;
            upgradeCostTxt.text = upgradeUnitData.Cost.ToString();
        }
        else
        {
            secondUpgradeBtn.interactable = false;

            UnitData firstUpgradeUnitData = allyUnitData.UpgradeUnits[0];
            firstUnitData = firstUpgradeUnitData;
            if (firstUpgradeUnitData != null)
            {
                // 첫번째 업그레이드 유닛
                firstUpgradeUnitImage.sprite = firstUpgradeUnitData.Icon;
                firstUpgradeUnitNameText.text = firstUpgradeUnitData.Name;
                firstUpgradeUnitAtTypeImage.sprite = firstUpgradeUnitData.AtTypeIcon;
                firstUpgradeUnitDfTypeImage.sprite = firstUpgradeUnitData.DfTypeIcon;

                // 승급 비용

                AllyUnitData upgradeUnitData = (AllyUnitData)firstUpgradeUnitData;
                upgradeCostTxt.text = upgradeUnitData.Cost.ToString();
            }
        }

        

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
