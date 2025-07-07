using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RosterButtonUI : MonoBehaviour
{
    [SerializeField] private UnitData unitData;
    private string unitId;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Sprite popupSprite;
    [SerializeField] private GameObject popUpPanel;
    [SerializeField] private UnitDataLoader unitDataLoader;
    private Unit unit;
    private UnitStats unitStats;


    [Header("■ PopUp")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image popupImage;
    [SerializeField] private Image[] tierImages;
    [SerializeField] private Image hp;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image attackTypeImage;
    [SerializeField] private TextMeshProUGUI attackTypeText;
    [SerializeField] private TextMeshProUGUI attackTypeInfoText;
    [SerializeField] private Image defenseTypeImage;
    [SerializeField] private TextMeshProUGUI defenseTypeText;
    [SerializeField] private TextMeshProUGUI defenseTypeInfoText;
    [SerializeField] private TextMeshProUGUI crtiText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI attackRangeText;
    [SerializeField] private TextMeshProUGUI mentalText;
    [SerializeField] private Image gSkillImage;
    [SerializeField] private TextMeshProUGUI gSkillText;
    [SerializeField] private TextMeshProUGUI gSkillInfoText;
    [SerializeField] private Image sSkillImage;
    [SerializeField] private TextMeshProUGUI sSkillText;
    [SerializeField] private TextMeshProUGUI sSkillInfoText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    

    private void Start()
    {
        unitImage.sprite = unitData.Icon;
        unitNameText.text = unitData.Name;

        unit = unitData.Prefab.GetComponent<Unit>();
        unitId = unit.UnitId;
        unitStats = unitDataLoader.GetUnitDataById(unitId, unit);
    }

    public void UpdatePopUpInfo()
    {
        nameText.text = unitData.Name;

        popupImage.sprite = popupSprite;

        SetUnitTierImage(unitData.Tier);

        hp.fillAmount = unitStats.maxHp / 500f;
        hpText.text = $"{unitStats.maxHp}";

        attackTypeImage.sprite = unitData.AtTypeIcon;
        //attackTypeText.text = unitData.AttackType;
        attackTypeInfoText.text = GetAttackTypeInfo(unitData); //.AttackType;

        defenseTypeImage.sprite = unitData.DfTypeIcon;
        defenseTypeText.text = ConvertDefenseName(unitData.ArmorType.ToString());
        defenseTypeInfoText.text = GetDefenseTypeInfo(unitData); //.ArmorType.ToString();

        crtiText.text = "치명타율 : " + unitStats.critChance;
        moveSpeedText.text = "이동속도 : " + unitStats.moveSpeed;
        attackSpeedText.text = "공격속도 : " + unitStats.attackSpeed;
        mentalText.text = "정신력 : " + unitStats.mental;
        attackRangeText.text = "공격범위 : " + unitStats.attackRange/2 + "칸";

        Unit unit = unitData.Prefab.GetComponent<Unit>();

        gSkillImage.sprite = unit.GeneralSkill.Data.Icon;
        gSkillText.text = unit.GeneralSkill.Data.Name;
        gSkillInfoText.text = unit.GeneralSkill.Data.Description;

        if (unit.SpecialSkill != null)
        {
            sSkillImage.sprite = unit.SpecialSkill.Data.Icon;
            sSkillText.text = unit.SpecialSkill.Data.Name;
            sSkillInfoText.text = unit.SpecialSkill.Data.Description;
        }
        else if (unit.PassiveSkill == null)
        {
            sSkillImage.sprite = unit.PassiveSkill.Data.Icon;
            sSkillText.text = unit.PassiveSkill.Data.Name;
            sSkillInfoText.text = unit.PassiveSkill.Data.Description;
        }

        descriptionText.text = unitData.Description;

        popUpPanel.SetActive(true);
    }

    private void SetUnitTierImage(int tier)
    {
        for (int i = 0; i < tierImages.Length; i++)
        {
            if (i < tier)
            {
                tierImages[i].gameObject.SetActive(true);
            }
            else
            {
                tierImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void ClosePopUp()
    {
        popUpPanel.SetActive(false);
    }

    private string GetAttackTypeInfo(UnitData unitData)
    {
        string attackTypeInfo = "";

        //if (unitData.AttackType == "베기")
        //{
        //    attackTypeInfo = "철갑에 약하다. 철갑을 입은 대상에게 주는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        //}
        //else if (unitData.AttackType == "찌르기")
        //{
        //    attackTypeInfo = "방탄갑에 약하다. 방탄갑을 입은 대상에게 주는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        //}
        //else if (unitData.AttackType == "때리기")
        //{
        //    attackTypeInfo = "완충갑에 약하다. 완충갑을 입은 대상에게 주는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        //}

        return attackTypeInfo;
    }

    private string GetDefenseTypeInfo(UnitData unitData)
    {
        string defensTypeInfo = "";

        if (unitData.ArmorType == Unit.ArmorType.STEELPLATED)
        {
            defensTypeInfo = "베기에 강하다. 베기 공격에 받는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        }
        else if (unitData.ArmorType == Unit.ArmorType.PADDED)
        {
            defensTypeInfo = "때리기에 강하다. 때리기 공격에 받는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        }
        else if (unitData.ArmorType == Unit.ArmorType.ANTIPIERCING)
        {
            defensTypeInfo = "찌르기에 강하다. 찌르기 공격에 받는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        }

        return defensTypeInfo;
    }

    private string ConvertDefenseName(string armorType)
    {
        if (armorType == Unit.ArmorType.PADDED.ToString())
        {
            return "완충갑";
        }
        else if (armorType == Unit.ArmorType.ANTIPIERCING.ToString())
        {
            return "방탄갑";
        }
        else if (armorType == Unit.ArmorType.STEELPLATED.ToString())
        {
            return "철갑";
        }
        else
            return "정보없음";
    }
}
