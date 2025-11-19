using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
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
    [SerializeField] private TextMeshProUGUI costTxt;
    [SerializeField] private GameObject costGameObj;
    [SerializeField] private TextMeshProUGUI tendency;   // Enemy 만
    [SerializeField] private TextMeshProUGUI tendencyTxt;
    [SerializeField] private GameObject tendencyGameObj;
    [SerializeField] private TextMeshProUGUI role;
    [SerializeField] private TextMeshProUGUI roleTxt;
    [SerializeField] private GameObject roleGameObj;
    [SerializeField] private TextMeshProUGUI dropGold;
    [SerializeField] private TextMeshProUGUI dropGoldTxt;   // Enemy
    [SerializeField] private GameObject goldGameObj;

    [Header("Skill")]
    [SerializeField] private Image generalIcon;
    [SerializeField] private Image specialIcon;
    [SerializeField] private Image passiveIcon;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI hpTxt;
    [SerializeField] private TextMeshProUGUI attackSpeed;
    [SerializeField] private TextMeshProUGUI attackSpeedTxt;
    [SerializeField] private TextMeshProUGUI armorType;
    [SerializeField] private TextMeshProUGUI armorTypeTxt;
    [SerializeField] private TextMeshProUGUI critChance;
    [SerializeField] private TextMeshProUGUI critChanceTxt;
    [SerializeField] private TextMeshProUGUI moveSpeed;
    [SerializeField] private TextMeshProUGUI moveSpeedTxt;
    [SerializeField] private TextMeshProUGUI mental;
    [SerializeField] private TextMeshProUGUI mentalTxt;
    [SerializeField] private TextMeshProUGUI attackRange;
    [SerializeField] private TextMeshProUGUI attackRangeTxt;

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
        tier.text = fNameTextTable.GetName(unitData.Tier.ToString());

        // skill
        generalIcon.sprite = unitData.GeneralSkill.Icon;
        specialIcon.sprite = unitData.SpecialSkill.Icon;
        // passiveSkill

        // stats
        hp.text = stats.maxHp.ToString();
        string hpText = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", "CON_hp", LocalizationSettings.SelectedLocale);
        hpTxt.text = $"{hpText} :"; 
        //---
        attackSpeed.text = stats.attackSpeed;
        string attackSText = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_attackSpeed", LocalizationSettings.SelectedLocale);
        attackSpeedTxt.text = $"{attackSText} :";
        //---
        string armor = unitData.ArmorType.ToString();
        armorType.text = fNameTextTable.GetName(armor);
        string armorT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_defenseType", LocalizationSettings.SelectedLocale);
        armorTypeTxt.text = $"{armorT} :";
        //---
        critChance.text = stats.critChance.ToString();
        string critT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_critChance", LocalizationSettings.SelectedLocale);
        critChanceTxt.text = $"{critT} :";
        //---
        moveSpeed.text = stats.moveSpeed.ToString();
        string moveSpeedT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_moveSpeed", LocalizationSettings.SelectedLocale);
        moveSpeedTxt.text = $"{moveSpeedT} : ";
        //---
        mental.text = stats.mental.ToString();
        string mentalT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_mental", LocalizationSettings.SelectedLocale);
        mentalTxt.text = $"{mentalT} :";
        //---
        attackRange.text = stats.attackRange.ToString();
        string attackRangeT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_attackRange", LocalizationSettings.SelectedLocale);
        attackRangeTxt.text = $"{attackRangeT} :";
        // story
        story.text = unitData.Description;

    }

    public void SetAllyInfo()
    {
        SetBasicInfo();
        AllyUnitData ally = unitData as AllyUnitData;
        cost.text = ally.Cost.ToString();
        string costT = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", "CON_recruitmentCost", LocalizationSettings.SelectedLocale);
        costTxt.text = $"{costT} :";
        costGameObj.SetActive(true);
        //---
        role.text = stats.role.ToString();
        string roleT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_role", LocalizationSettings.SelectedLocale);
        roleTxt.text = $"{roleT} :";
        roleGameObj.SetActive(true);


        tendency.text = "";
        tendencyGameObj.SetActive(false);
        dropGold.text = "";
        goldGameObj.SetActive(false);
    }

    public void SetEnemyInfo()
    {
        SetBasicInfo();
        EnemyUnitData enemy = unitData as EnemyUnitData;
        //tendency.text = enemy.aiStance.ToString();
        string s = enemy.aiStance.ToString();
        tendency.text = fNameTextTable.GetName(s);
        //string tendencyT = LocalizationSettings.StringDatabase.
        //    GetLocalizedString("CommonUI", "CON_attackRange", LocalizationSettings.SelectedLocale);
        //tendencyTxt.text = $"{tendencyT} :";
        tendencyGameObj.SetActive(true);
        //---
        dropGold.text = enemy.Gold.ToString();
        //string dropGoldT = LocalizationSettings.StringDatabase.
        //    GetLocalizedString("CommonUI", "CON_attackRange", LocalizationSettings.SelectedLocale);
        //dropGoldTxt.text = $"{dropGoldT} :";
        goldGameObj.SetActive(true);

        cost.text = "";
        costGameObj.SetActive(false);
        role.text = "";
        roleGameObj.SetActive(false);

    }
}
