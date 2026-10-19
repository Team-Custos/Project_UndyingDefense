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
    [SerializeField] private Button upgradeBtn;     // 승급버튼?
    [SerializeField] private Image unitAtTypeImage;     // 필요없음

    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image unitDfTypeImage;

    [SerializeField] private Image[] unitTierImage;

    [SerializeField] private TextMeshProUGUI hpValue;
    [SerializeField] private TextMeshProUGUI mentalValue;
    [SerializeField] private TextMeshProUGUI attackRangeValue;

    [SerializeField] private Image generalSkillImage;
    [SerializeField] private Image generalSkillAttackTypeIcon;
    [SerializeField] private Image specialSkillImage;
    [SerializeField] private Image specialSkillAttackTypeIcon;
    [SerializeField] private Image abilityImage;

    [Header("승급버튼")]
    [SerializeField] private Button upgradePerformBtn;
    [SerializeField] private TextMeshProUGUI upgradeCostTxt;

    private Unit upgradeUnit;

    private float cost;

    private void FieldNameLocalization(UnitData data, TextMeshProUGUI text)
    {
        text.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{data.Id}_name", LocalizationSettings.SelectedLocale);
    }

    private void FieldStatLocalization()
    {

    }

    // 매개변수 업글유닛, 유닛의 인덱스, 현재 선택된 유닛maxHp, 현재 골드 
    // UpgradeMenuUI에서 호출할 함수용으로 만들기
    public void SetUpgradeUnitPanel(Unit _upgradeUnit, int index, float selectedUnitMaxHp)
    {
        if (_upgradeUnit is EnemyUnit)
            return;

        upgradeUnit = _upgradeUnit;
        AllyUnitData upgradeUnitData = (AllyUnitData)_upgradeUnit.Data;


        upgradePerformBtn.interactable = false; //? 

        //---unit image / name / dfType icon---
        unitImage.sprite = upgradeUnitData.Icon;
        FieldNameLocalization(upgradeUnitData, unitNameText);
        unitDfTypeImage.sprite = upgradeUnitData.DfTypeIcon;

        //---리디자인으로 인한 추가
        //---stats---               // unit 필요
        hpValue.text =
            upgradeUnit.UnitStats.maxHp.ToString() + " (+ " + (upgradeUnit.UnitStats.maxHp - selectedUnitMaxHp).ToString() + ")";
        mentalValue.text = upgradeUnit.UnitStats.mental.ToString();
        attackRangeValue.text = upgradeUnit.UnitStats.attackRange.ToString();

        //---skill and ability icons--- // unit 필요
        generalSkillImage.sprite = upgradeUnit.GeneralSkill.Data.Icon;
        specialSkillImage.sprite = upgradeUnit.SpecialSkill.Data.Icon;
        abilityImage.sprite = upgradeUnit.SpecialAbility.Icon;

        //---skill attack type icon---  // unit 필요
        AttackSkillData gSkillData = upgradeUnit.GeneralSkill.Data as AttackSkillData;
        generalSkillAttackTypeIcon.sprite = gSkillData.Info.TypeIcon;
        AttackSkillData sSkillData = upgradeUnit.SpecialSkill.Data as AttackSkillData;
        specialSkillAttackTypeIcon.sprite = sSkillData.Info.TypeIcon;

        //---tier image---
        for (int i = 0; i < unitTierImage.Length; i++)
        {
            if (i < upgradeUnitData.Tier)
                unitTierImage[i].gameObject.SetActive(true); // 켜기
            else
                unitTierImage[i].gameObject.SetActive(false); // 끄기
        }

        //---upgrade cost---
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
