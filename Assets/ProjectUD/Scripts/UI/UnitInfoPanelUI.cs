using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using static Unit;

public class UnitInfoPanelUI : MonoBehaviour
{
    [Header("UnitDataLoader")]
    [SerializeField] private UnitDataLoader loader;

    [Header("NameTextTable")]
    [SerializeField] private FactionNameTextTable fNameTextTable;

    private UnitData unitData;
    private UnitStats stats;

    [Header("Unit")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI tier;
    [SerializeField] private TextMeshProUGUI tierTxt;

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
    [SerializeField] private GameObject abilityPanel;

    [Header("Tooltip")]
    [SerializeField] private ToolTipUI[] tooltips;

    [Header("기본스킬")]
    [SerializeField] private TextMeshProUGUI infoGSkillName;
    [SerializeField] private TextMeshProUGUI infoGSkillDescript;
    [SerializeField] private TextMeshProUGUI infoGSkillEffect;
    [SerializeField] private TextMeshProUGUI infoGSkillEtc;

    [Header("특수스킬")]
    [SerializeField] private TextMeshProUGUI infoSSkillName;
    [SerializeField] private TextMeshProUGUI infoSSkillDescript;
    [SerializeField] private TextMeshProUGUI infoSSkillEffect;
    [SerializeField] private TextMeshProUGUI infoSSkillEtc;

    [Header("특수능력")]
    [SerializeField] private TextMeshProUGUI infoAbilityName;
    [SerializeField] private TextMeshProUGUI infoAbilityDescript;
    [SerializeField] private TextMeshProUGUI infoAbilityEffect;

    [Header("Defense")]
    [SerializeField] private TextMeshProUGUI armorType;
    [SerializeField] private TextMeshProUGUI armorTypeTxt;
    [SerializeField] private Image armorIcon;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI hpTxt;
    [SerializeField] private TextMeshProUGUI attackSpeed;
    [SerializeField] private TextMeshProUGUI attackSpeedTxt;
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
    [SerializeField] private TextMeshProUGUI storyText;

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

    //public void ConvertInfo()
    //{
    //    armorType.text = unitData.ArmorType.ToString();
    //    Debug.Log(armorType.text);
    //}

    public void SetBasicInfo()
    {
        // unit
        icon.sprite = unitData.Icon;

        string unitId = unitData.Id + "_name";
        unitName.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{unitId}", LocalizationSettings.SelectedLocale);

        //--
        string tierT = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", "CON_tier", LocalizationSettings.SelectedLocale);
        tierTxt.text = $"{tierT} :";

        string tierId = fNameTextTable.GetName(unitData.Tier.ToString());
        tier.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", $"{tierId}", LocalizationSettings.SelectedLocale);

        // skill
        generalIcon.sprite = unitData.GeneralSkill.Icon;
        specialIcon.sprite = unitData.SpecialSkill.Icon;

        //--Local 스킬이름
        infoGSkillName.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.GeneralSkill.Name}_name", LocalizationSettings.SelectedLocale);
        //--Local 스킬 설명 (desc + effect)
        infoGSkillDescript.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.GeneralSkill.Name}_desc", LocalizationSettings.SelectedLocale);
        infoGSkillEffect.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.GeneralSkill.Name}_effect", LocalizationSettings.SelectedLocale);

        // Smart String Arguments용
        string gCooltime = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillCooltime",
            new object[] { new { num = unitData.GeneralSkill.CoolTime } });

        var gRange = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillRange",
            new object[] { new { num = unitData.GeneralSkill.Range / 2 } });

        var gMental = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillMental",
            new object[] { new { num = unitData.GeneralSkill.ActiveMental } });

        infoGSkillEtc.text = $"{gCooltime} / {gRange} / {gMental}";
        //infoGSkillEtc.text = $"쿨타임 {unitData.GeneralSkill.CoolTime}초 / 사거리 {unitData.GeneralSkill.Range}보 / 멘탈 요구 {unitData.GeneralSkill.ActiveMental}";

        // special skill
        infoSSkillName.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.SpecialSkill.Name}_name", LocalizationSettings.SelectedLocale);
        infoSSkillDescript.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.SpecialSkill.Name}_desc", LocalizationSettings.SelectedLocale);
        infoSSkillEffect.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.SpecialSkill.Name}_effect", LocalizationSettings.SelectedLocale);

        // Smart String Arguments용
        var sCooltime = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillCooltime", 
            new object[] { new { num = unitData.SpecialSkill.CoolTime } });

        var sRange = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillRange",
            new object[] { new { num = unitData.SpecialSkill.Range / 2 } });

        var sMental = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillMental",
            new object[] { new { num = unitData.SpecialSkill.ActiveMental } });

        infoSSkillEtc.text = $"{sCooltime} / {sRange} / {sMental}";
        //infoSSkillEtc.text = $"쿨타임 {unitData.SpecialSkill.CoolTime}초 / 사거리 {unitData.SpecialSkill.Range}보 / 멘탈 요구 {unitData.SpecialSkill.ActiveMental}";

        // passiveSkill
        if(unitData.SpecialAbility != null)
        {
            // 아이콘이 있는 경우 알파값1로 변경
            passiveIcon.gameObject.SetActive(true);
            passiveIcon.color = new Color(1,1,1,1f);
            passiveIcon.sprite = unitData.SpecialAbility.Icon;
            abilityPanel.SetActive(true);

            infoAbilityName.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("SpecialAbility", $"{unitData.SpecialAbility.Id}_name", LocalizationSettings.SelectedLocale);
            infoAbilityDescript.text = LocalizationSettings.StringDatabase.
                GetLocalizedString("SpecialAbility", $"{unitData.SpecialAbility.Id}_desc", LocalizationSettings.SelectedLocale);
            infoAbilityEffect.text = LocalizationSettings.StringDatabase.
                GetLocalizedString("SpecialAbility", $"{unitData.SpecialAbility.Id}_effect", LocalizationSettings.SelectedLocale);
        }
        else
        {
            // 아이콘이 없는 경우 알파값0으로 변경
            passiveIcon.color = new Color(1, 1, 1, 0f);
            infoAbilityName.text = "";
            infoAbilityDescript.text = "";
            infoAbilityEffect.text = "";
            passiveIcon.gameObject.SetActive(false);
            abilityPanel.SetActive(false);
        }



        // stats
        hp.text = stats.maxHp.ToString();
        string hpText = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", "CON_hp", LocalizationSettings.SelectedLocale);
        hpTxt.text = $"{hpText} :";
        //---attackSpeed
        string atSpeedId = fNameTextTable.GetName("Interval_" + $"{stats.interval.ToString()}");
        attackSpeed.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", $"{atSpeedId}", LocalizationSettings.SelectedLocale);

        string attackSText = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_attackSpeed", LocalizationSettings.SelectedLocale);
        attackSpeedTxt.text = $"{attackSText} :";
        //---
        string armorId = fNameTextTable.GetName(unitData.ArmorType.ToString());
        armorType.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", $"{armorId}", LocalizationSettings.SelectedLocale);
        armorIcon.sprite = unitData.DfTypeIcon;

        string armorT = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", "CON_defenseType1", LocalizationSettings.SelectedLocale);
        armorTypeTxt.text = $"{armorT} :";
        //---
        critChance.text = $"{stats.critChance}%";
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
        var atRange = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillRangeValue",
            new object[] { new { num = stats.attackRange / 2 } });
        attackRange.text = $"{atRange}";
        string attackRangeT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_attackRange", LocalizationSettings.SelectedLocale);
        attackRangeTxt.text = $"{attackRangeT} :";
        // story
        string storyId = unitData.Id + "_desc";
        story.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{storyId}", LocalizationSettings.SelectedLocale);
        string storyTId = unitData.Id + "_script";
        storyText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{storyTId}", LocalizationSettings.SelectedLocale);

    }

    public void SetAllyInfo()
    {
        SetBasicInfo();
        AllyUnitData ally = unitData as AllyUnitData;
        /*
        cost.text = ally.Cost.ToString();
        string costT = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", "CON_recruitmentCost", LocalizationSettings.SelectedLocale);
        costTxt.text = $"{costT} :";
        costGameObj.SetActive(true);
        //---
        string roleId = fNameTextTable.GetName(stats.role.ToString());
        role.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", $"{roleId}", LocalizationSettings.SelectedLocale);

        string roleT = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", "CON_role", LocalizationSettings.SelectedLocale);
        roleTxt.text = $"{roleT} :";
        roleGameObj.SetActive(true);


        tendency.text = "";
        tendencyGameObj.SetActive(false);
        dropGold.text = "";
        goldGameObj.SetActive(false);
        */
    }

    public void SetEnemyInfo()
    {
        SetBasicInfo();
        EnemyUnitData enemy = unitData as EnemyUnitData;
        //tendency.text = enemy.aiStance.ToString();
        /*
        string tendencyId = fNameTextTable.GetName(enemy.aiStance.ToString());
        tendency.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", $"{tendencyId}", LocalizationSettings.SelectedLocale);

        string tendencyT = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", "CON_mission", LocalizationSettings.SelectedLocale);
        tendencyTxt.text = $"{tendencyT} :";
        tendencyGameObj.SetActive(true);
        //---
        dropGold.text = enemy.Gold.ToString();
        string dropGoldT = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", "CON_reward", LocalizationSettings.SelectedLocale);
        dropGoldTxt.text = $"{dropGoldT} :";
        goldGameObj.SetActive(true);

        cost.text = "";
        costGameObj.SetActive(false);
        role.text = "";
        roleGameObj.SetActive(false);
        */

    }
}
