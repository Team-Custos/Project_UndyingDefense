using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UpgradeUnitPanel : MonoBehaviour
{
    [Header("NameTextTable")]
    [SerializeField] private FactionNameTextTable fNameTextTable;

    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitDfTypeText;
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

    //private Unit upgradeUnit;
    //private UnitData upgradeUnitData;

    private float cost;

    private void FieldNameLocalization(UnitData data, TextMeshProUGUI text)
    {
        text.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{data.Id}_name", LocalizationSettings.SelectedLocale);
    }

    private void FieldStatLocalization()
    {

    }

    // 매개변수 업글유닛데이터, 유닛 스탯, 유닛의 인덱스, 현재 선택된 유닛maxHp, 현재 골드 
    // UpgradeMenuUI에서 호출할 함수용으로 만들기
    public void SetUpgradeUnitPanel(UnitData _upgradeUnitData, UnitStats upgradeUnitStats, int index, float selectedUnitMaxHp, float gold)
    {
        if (_upgradeUnitData is EnemyUnitData)
            return;

        AllyUnitData upgradeUnitData = (AllyUnitData)_upgradeUnitData;

        Debug.Log($"unitImage null? {unitImage == null} / Data null? {upgradeUnitData == null}", this);

        upgradePerformBtn.interactable = false; //? 

        //---unit image / name / dfType icon---
        unitImage.sprite = upgradeUnitData.Icon;
        FieldNameLocalization(upgradeUnitData, unitNameText);
        string dfTypeId = fNameTextTable.GetName(upgradeUnitData.ArmorType.ToString());
        unitDfTypeText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", dfTypeId, LocalizationSettings.SelectedLocale);
        unitDfTypeImage.sprite = upgradeUnitData.DfTypeIcon;

        //---리디자인으로 인한 추가
        //---stats---               // unitDataLoader 필요
        hpValue.text =
            upgradeUnitStats.maxHp.ToString() + " (+ " + (upgradeUnitStats.maxHp - selectedUnitMaxHp).ToString() + ")";
        mentalValue.text = upgradeUnitStats.mental.ToString();
        attackRangeValue.text = upgradeUnitStats.attackRange.ToString();

        //---skill and ability icons--- // unitData 필요
        generalSkillImage.sprite = upgradeUnitData.GeneralSkill.Icon;
        specialSkillImage.sprite = upgradeUnitData.SpecialSkill.Icon;
        if (upgradeUnitData.SpecialAbility == null)
        {
            abilityImage.sprite = null;
            abilityImage.gameObject.SetActive(false);
        }
        else
        {
            abilityImage.sprite = upgradeUnitData.SpecialAbility.Icon;
            abilityImage.gameObject.SetActive(true);
        }

        //---skill attack type icon---  // unitData>SkillData>AttackSkillData로 캐스팅 필요
        AttackSkillData gSkillData = upgradeUnitData.GeneralSkill as AttackSkillData;
        generalSkillAttackTypeIcon.sprite = gSkillData.Info.TypeIcon;
        // specialSkillData가 AttackSkillData인지 확인 후 캐스팅 ( attack 이외에 다른 스킬인 경우가 있음 )
        if(upgradeUnitData.SpecialSkill is AttackSkillData)
        {
            AttackSkillData sSkillData = upgradeUnitData.SpecialSkill as AttackSkillData;
            specialSkillAttackTypeIcon.sprite = sSkillData.Info.TypeIcon;
            specialSkillAttackTypeIcon.gameObject.SetActive(true); // AttackSkillData인 경우 아이콘 활성화
        }
        else
        {
            specialSkillAttackTypeIcon.gameObject.SetActive(false); // AttackSkillData가 아닌 경우 아이콘 비활성화
            specialSkillAttackTypeIcon.sprite = null; // null로 설정하거나 기본 아이콘으로 설정
        }

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

        UpdateCostTextColorAndPerformBtn(gold);

    }

    // 골드에 따라 승급버튼 활성화 여부와 골드 텍스트 색상 변경
    public void UpdateCostTextColorAndPerformBtn(float gold)
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
