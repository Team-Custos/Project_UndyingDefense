using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UpgradeUnitPanel : MonoBehaviour
{
    [Header("NameTextTable")]
    [SerializeField] private FactionNameTextTable fNameTextTable;

    [SerializeField] private Image firstUpgradeUnitImage;
    [SerializeField] private Button firstUpgradeBtn;
    [SerializeField] private TextMeshProUGUI firstUpgradeUnitNameText;
    [SerializeField] private Image firstUpgradeUnitAtTypeImage;
    [SerializeField] private Image firstUpgradeUnitDfTypeImage;
    [SerializeField] private Image[] firstUpgradeUnitTierImage;
    [SerializeField] private TextMeshProUGUI firstUpgradeUnitHpText;
    [SerializeField] private TextMeshProUGUI firstUpgradeUnitMentalText;
    [SerializeField] private TextMeshProUGUI firstUpgradeUnitAttackRangeText;
    [SerializeField] private Image firstUpgradeUnitGSkillImage;
    [SerializeField] private Image firstGSkillAttackTypeIcon;
    [SerializeField] private Image firstUpgradeUnitSSkillImage;
    [SerializeField] private Image firstSSkillAttackTypeIcon;
    [SerializeField] private Image firstUpgradeUnitAbilityImage;

    [Header("승급버튼")]
    [SerializeField] private Button upgradePerformBtn;
    [SerializeField] private TextMeshProUGUI upgradeCostTxt;

    private UnitData firstUpgradeUnitData;
    private Unit firstUpgradeUnit;
    private Unit currentUnit;

    private float cost;

    private void FieldNameLocalization(UnitData data, TextMeshProUGUI text)
    {
        text.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{data.Id}_name", LocalizationSettings.SelectedLocale);
    }

    // 매개변수 업글유닛, 인덱스, 현재 선택된 유닛maxHp, 현재 골드 
    // UpgradeMenuUI에서 호출할 함수용으로 만들기
    public void SetUnitUpgradeMenu(Unit selectedUnit)
    {
        if (selectedUnit is EnemyUnit)
            return;

        currentUnit = selectedUnit;
        AllyUnitData currentUnitData = (AllyUnitData)selectedUnit.Data;


        firstUpgradeUnitData = currentUnitData.UpgradeUnits[0]; // 업그레이드 유닛이 UnitData로 들어가 있음

        upgradePerformBtn.interactable = false;
        firstUpgradeUnit = firstUpgradeUnitData.Prefab.GetComponent<Unit>();

        firstUpgradeUnitImage.sprite = firstUpgradeUnitData.Icon;
        FieldNameLocalization(firstUpgradeUnitData, firstUpgradeUnitNameText);
        firstUpgradeUnitDfTypeImage.sprite = firstUpgradeUnitData.DfTypeIcon;

        //---리디자인으로 인한 추가

        firstUpgradeUnitHpText.text =
            firstUpgradeUnit.UnitStats.maxHp.ToString() + " (+ " + (firstUpgradeUnit.UnitStats.maxHp - currentUnit.Maxhp).ToString() + ")";
        firstUpgradeUnitMentalText.text = firstUpgradeUnit.UnitStats.mental.ToString();
        firstUpgradeUnitAttackRangeText.text = firstUpgradeUnit.UnitStats.attackRange.ToString();

        firstUpgradeUnitGSkillImage.sprite = firstUpgradeUnit.GeneralSkill.Data.Icon;
        firstUpgradeUnitSSkillImage.sprite = firstUpgradeUnit.SpecialSkill.Data.Icon;
        firstUpgradeUnitAbilityImage.sprite = firstUpgradeUnit.SpecialAbility.Icon;

        AttackSkillData gSkillData = firstUpgradeUnit.GeneralSkill.Data as AttackSkillData;
        firstGSkillAttackTypeIcon.sprite = gSkillData.Info.TypeIcon;
        AttackSkillData sSkillData = firstUpgradeUnit.SpecialSkill.Data as AttackSkillData;
        firstSSkillAttackTypeIcon.sprite = sSkillData.Info.TypeIcon;

        for (int i = 0; i < firstUpgradeUnitTierImage.Length; i++)
        {
            if (i < firstUpgradeUnitData.Tier)
                firstUpgradeUnitTierImage[i].gameObject.SetActive(true); // 켜기
            else
                firstUpgradeUnitTierImage[i].gameObject.SetActive(false); // 끄기
        }

        AllyUnitData upgradeUnitData = (AllyUnitData)firstUpgradeUnitData;

        cost = upgradeUnitData.Cost;
        upgradeCostTxt.text = cost.ToString();

        //UpdateGoldTextColorAndPerformBtn(inGameManager.inGameGold);

    }

    public void UpdateGoldTextColorAndPerformBtn(float gold)
    {
        if (gold < cost)
        {
            upgradePerformBtn.interactable = false;
            upgradeCostTxt.color = Color.red;
        }
        else
        {
            upgradePerformBtn.interactable = true;
            upgradeCostTxt.color = Color.white;
        }
    }
}
