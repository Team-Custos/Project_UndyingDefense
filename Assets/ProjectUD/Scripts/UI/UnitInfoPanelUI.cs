using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unit;

public class UnitInfoPanelUI : MonoBehaviour
{
    [Header("UnitDataLoader")]
    [SerializeField] private UnitDataLoader loader;

    [Header("UnitDataLoader")]
    [SerializeField] private FactionNameTextTable fNameTextTable;

    private UnitData unitData;
    private UnitStats stats;

    [Header("Unit")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI tier;

    [Header("UnitDatails")]
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI tendency;   // Enemy 만
    [SerializeField] private TextMeshProUGUI role;

    [Header("Skill")]
    [SerializeField] private Image generalIcon;
    [SerializeField] private Image specialIcon;
    [SerializeField] private Image passiveIcon;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI attackSpeed;
    [SerializeField] private TextMeshProUGUI armorType;
    [SerializeField] private TextMeshProUGUI critChance;
    [SerializeField] private TextMeshProUGUI moveSpeed;
    [SerializeField] private TextMeshProUGUI mental;
    [SerializeField] private TextMeshProUGUI attackRange;

    [Header("Story")]
    [SerializeField] private TextMeshProUGUI story;


    public void SetUnitData(UnitData unitData)
    {
        this.unitData = unitData;
        stats = loader.GetUnitDataById(unitData.Id);

        if(unitData is AllyUnitData)
        {
            SetAllyInfo();
        }
        else
        {
            SetEnemyInfo();
        }
    }

    public void ConvertInfo()
    {
        armorType.text = unitData.ArmorType.ToString();
        Debug.Log(armorType.text);
    }

    public void SetBasicInfo()
    {
        // unit
        icon.sprite = unitData.Icon;
        unitName.text = unitData.Name;
        tier.text = unitData.Tier.ToString();
        role.text = stats.role.ToString();

        // skill
        generalIcon.sprite = unitData.GeneralSkill.Icon;
        specialIcon.sprite = unitData.SpecialSkill.Icon;
        // passiveSkill

        // stats
        hp.text = stats.maxHp.ToString();
        attackSpeed.text = stats.attackSpeed.ToString();

        string armor = unitData.ArmorType.ToString();
        armorType.text = fNameTextTable.GetName(armor);
        //armorType.text = unitData.ArmorType.ToString();
        
        critChance.text = stats.critChance.ToString();
        moveSpeed.text = stats.moveSpeed.ToString();
        mental.text = stats.mental.ToString();
        attackRange.text = stats.attackRange.ToString();

        // story
        story.text = unitData.Description;

    }

    public void SetAllyInfo()
    {
        SetBasicInfo();
        AllyUnitData ally = unitData as AllyUnitData;
        cost.text = ally.Cost.ToString();
    }

    public void SetEnemyInfo()
    {
        SetBasicInfo();
        EnemyUnitData enemy = unitData as EnemyUnitData;
        tendency.text = enemy.aiStance.ToString();

    }
}
