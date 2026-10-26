using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class SelectedUnitInfoPanel : MonoBehaviour
{
    [Header("NameTextTable")]
    [SerializeField] private FactionNameTextTable fNameTextTable;

    [Header("■ UnitInfo")]
    [SerializeField] private Image unitInfoImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitHPText;
    [SerializeField] private Image unitHPImage;
    [SerializeField] private TextMeshProUGUI unitMentalText;    // 삭제 예정
    //[SerializeField] private Image unitMentalImage;  나중에 작업
    [SerializeField] private Image atTypeIcon;
    [SerializeField] private Image dfTypeIcon;
    [SerializeField] private Image unitSSkillImage;
    [SerializeField] private Image unitGSkillImage;
    [SerializeField] private GameObject specialAbilityUI;
    [SerializeField] private Image specialAbilityImage;
    [SerializeField] private TextMeshProUGUI dfTypeText;
    [SerializeField] private TextMeshProUGUI critText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI atSpeedText;
    [SerializeField] private TextMeshProUGUI atRangeText;
    [SerializeField] private TextMeshProUGUI mentalText;
    [SerializeField] private Image[] tierImage;

    [SerializeField] private Image[] unitStateImage;
    [SerializeField] private StatusUI[] unitStateUIs;
    //[SerializeField] private GameObject unitStatePanel;
    [SerializeField] private GameObject typeInfo;


    [Header("======= SkillInfo =======")]

    // 기본 스킬
    [SerializeField] private TextMeshProUGUI gSkillNameText;
    [SerializeField] private TextMeshProUGUI gSkillInfoText;
    [SerializeField] private TextMeshProUGUI gSkillEffectText;
    [SerializeField] private TextMeshProUGUI gSkillEtcText;

    // 특수 스킬
    [SerializeField] private TextMeshProUGUI sSkilNameText;
    [SerializeField] private TextMeshProUGUI sSkillInfoText;
    [SerializeField] private TextMeshProUGUI sSkillEffectText;
    [SerializeField] private TextMeshProUGUI sSkillEtcText;

    // 특수 능력
    [SerializeField] private TextMeshProUGUI sAbilityNameText;
    [SerializeField] private TextMeshProUGUI sAbilityInfoText;
    [SerializeField] private TextMeshProUGUI sAbilityEffectText;

    [SerializeField] private GameObject[] skillToolTip;

    private Unit selectedUnit;

    // 선택 대상 정보 창
    public void UpdateUnitInfoByBtn(UnitData unitData, UnitStats stats) //, UnitDataLoader unitDataLoader)   // 데이터로더 말고 유닛스탯을 매개변수로 받아서 처리하도록 수정 필요_261014
    {
        //Unit unit = unitData.Prefab.GetComponent<Unit>();

        //UnitStats unitStats = unitDataLoader.GetUnitDataById(unit.UnitId);

        UnitStats unitStats = stats;

        unitInfoImage.gameObject.SetActive(true);


        unitImage.sprite = unitData.Icon;

        //unitNameText.text = unitStats.unitName;
        //-- Localization --
        unitNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{unitStats.id}_name", LocalizationSettings.SelectedLocale);
        unitMentalText.text = "멘탈 : " + unitStats.mental.ToString();
        unitHPImage.fillAmount = unitStats.maxHp / unitStats.maxHp;
        unitHPText.text = $"{unitStats.maxHp} / {unitStats.maxHp}";

        SetUnitTierIcon(unitData.Tier);

        //AttackSkillData attackSkillData = unit.GeneralSkill.Data as AttackSkillData;    // 필요없음

        //atTypeIcon.sprite = attackSkillData.Info.TypeIcon;
        dfTypeIcon.sprite = unitData.DfTypeIcon;

        dfTypeText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", $"CON_defense{unitData.ArmorType}", LocalizationSettings.SelectedLocale);

        //attackTypeText.text = unitData.AttackType;

        //unitGSkillText.text = unit.GeneralSkill.Data.name;

        UpdateSkillInfo(unitData);      // 선택 유닛 스킬 정보 처리

        FieldLocalization(unitStats);   // 선택 유닛 스탯 로컬라이징 처리


        // 상태 이미지 끄기
        for (int i = 0; i < unitStateImage.Length; i++)
        {
            unitStateImage[i].gameObject.SetActive(false);
        }
    }

    //string pointColor = "#000000";

    // 선택 대상 정보 창
    private void FieldLocalization(UnitStats unitStats)     //260408 로폴 수정  
    {
        //string mentalT = LocalizationSettings.StringDatabase.
        //    GetLocalizedString("CommonUI", "CON_mental", LocalizationSettings.SelectedLocale);
        mentalText.text = $"{unitStats.mental}"; // $"{mentalT} <color={pointColor}>{unitStats.mental}</color>";

        //string critT = LocalizationSettings.StringDatabase.
        //    GetLocalizedString("CommonUI", "CON_critChance", LocalizationSettings.SelectedLocale);
        critText.text = $"{unitStats.critChance}%"; //$"{critT} <color={pointColor}>{unitStats.critChance}%</color>";

        //string moveSpeedT = LocalizationSettings.StringDatabase.
        //    GetLocalizedString("CommonUI", "CON_moveSpeed", LocalizationSettings.SelectedLocale);
        moveSpeedText.text = $"{unitStats.moveSpeed}"; //  <color={pointColor}>{unitStats.moveSpeed}</color>";

        //string attackSText = LocalizationSettings.StringDatabase.
        //    GetLocalizedString("CommonUI", "CON_attackSpeed", LocalizationSettings.SelectedLocale);
        string atSpeedId = fNameTextTable.GetName("Interval_" + unitStats.interval.ToString());
        string atSpeedValue = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", atSpeedId, LocalizationSettings.SelectedLocale);
        atSpeedText.text = $"{atSpeedValue}";

        string attackRangeT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_attackRange", LocalizationSettings.SelectedLocale);
        var atRange = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillRangeValue",
            new object[] { new { num = unitStats.attackRange / 2 } });
        atRangeText.text = $"{atRange}";  //$"{attackRangeT} <color={pointColor}>{atRange}</color>";
    }

    // 선택 대상 정보 창   // SelectedUnitManager에서 선택된 유닛을 매개변수로 받아오기
    public void UpdateUnitInfo(Unit unit)
    {
        //unit.SetSelectedUnitUI(this);
        selectedUnit = unit;

        unitInfoImage.gameObject.SetActive(true);

        if (unit.UnitStats == null)
        {
            Debug.Log("데이터 없음");
            return;
        }

        UpdateHPUI(unit.Hp, unit.Maxhp);
        unitImage.sprite = unit.Data.Icon;
        //unitNameText.text = unit.UnitStats.unitName;
        //--Localization
        unitNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{unit.UnitStats.id}_name", LocalizationSettings.SelectedLocale);
        unitMentalText.text = "멘탈 : " + unit.UnitStats.mental.ToString();

        SetUnitTierIcon(unit.Data.Tier);

        AttackSkillData attackSkillData = unit.GeneralSkill.Data as AttackSkillData;

        //atTypeIcon.sprite = attackSkillData.Info.TypeIcon;
        dfTypeIcon.sprite = unit.Data.DfTypeIcon;

        dfTypeText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", $"CON_defense{unit.Data.ArmorType}", LocalizationSettings.SelectedLocale);

        //attackTypeText.text = unit.Data.AttackType;

        //unitGSkillText.text = unit.GeneralSkill.Data.name;

        UpdateSkillInfo(unit.Data);


        FieldLocalization(unit.UnitStats);

        //critText.text = "치명타율 : " + unit.UnitStats.critChance.ToString() + "%";
        //moveSpeedText.text = "이동속도 : " + unit.UnitStats.moveSpeed.ToString();
        //atSpeedText.text = "공격속도 : " + unit.UnitStats.attackSpeed;
        //atRangeText.text = "공격거리 : " + (unit.UnitStats.attackRange / 2).ToString() + "칸";
        //mentalText.text = "멘탈 : " + unit.UnitStats.mental.ToString();

        UpdateUnitStateUI();
    }

    // 선택 대상 정보 창
    public void HideUnitInfo()
    {
        unitInfoImage.gameObject.SetActive(false);
        //unitStatePanel.SetActive(false);
        typeInfo.SetActive(false);

        for (int i = 0; i < skillToolTip.Length; i++)
        {
            skillToolTip[i].SetActive(false);
        }
    }

    // 선택 대상 정보 창           // _ 유닛이 아니라 float로 받아오기
    public void UpdateHPUI(float hp, float maxHp)//(Unit unit)
    {
        //if (unit != null)
        //{
        //    unitHPText.text = $"{unit.Hp} / {unit.Maxhp}";  
        //    unitHPImage.fillAmount = unit.HpPercent;    // unit의 HpPercent 존재.
        //}
        unitHPText.text = $"{hp} / {maxHp}";
        unitHPImage.fillAmount = hp / maxHp;
    }


    // 선택 대상 정보 창
    public void SetUnitTierIcon(int tier)
    {
        for (int i = 0; i < tierImage.Length; i++)
        {
            tierImage[i].gameObject.SetActive(i < tier);
        }
    }

    // 선택 대상 정보 창   // SelectedUnitManager에서 선택된 유닛을 매개변수로 받아오기
    public void UpdateUnitStateUI()
    {
        if (selectedUnit != null)
            //f (selecteUnitManger.SelectedUnit != null)
        {
            IReadOnlyList<DurationEffect> effects = selectedUnit.EffectList;    //selecteUnitManger.SelectedUnit.EffectList;
            //HashSet<Sprite> usedSprites = new HashSet<Sprite>();
            int imageIndex = 0;

            for (int i = 0; i < effects.Count && imageIndex < unitStateImage.Length; i++)
            {
                unitStateImage[imageIndex].sprite = effects[imageIndex].IconSprite;
                unitStateImage[imageIndex].gameObject.SetActive(true);
                unitStateUIs[imageIndex].SetEffect(effects[imageIndex]);
                imageIndex++;

                //if (sprite != null && !usedSprites.Contains(sprite))
                //{
                //    usedSprites.Add(sprite);

                //    // Sprite가 다르면 Sprite 업데이트
                //    if (unitStateImage[imageIndex].sprite != sprite)
                //    {
                //        unitStateImage[imageIndex].sprite = sprite;
                //    }

                //    // 꺼져있으면 켜기
                //    if (!unitStateImage[imageIndex].gameObject.activeSelf)
                //    {
                //        unitStateImage[imageIndex].gameObject.SetActive(true);
                //    }

                //    // Effect도 갱신
                //    unitStateUIs[imageIndex].SetEffect(effect);

                //    imageIndex++;
                //}
            }

            // 이펙트가 더 적어졌을 때만 뒤에 남은 이미지를 꺼주기
            for (int i = imageIndex; i < unitStateImage.Length; i++)
            {
                if (unitStateImage[i].gameObject.activeSelf)
                {
                    unitStateImage[i].gameObject.SetActive(false);
                }
            }
        }
    }

    // 선택 대상 정보 창
    private void UpdateSkillInfo(UnitData unitData)
    {
        unitGSkillImage.sprite = unitData.GeneralSkill.Icon;
        //--Local 스킬이름
        gSkillNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.GeneralSkill.Name}_name", LocalizationSettings.SelectedLocale);
        //--Local 스킬 설명 (desc + effect)
        gSkillInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.GeneralSkill.Name}_desc", LocalizationSettings.SelectedLocale);
        gSkillEffectText.text = LocalizationSettings.StringDatabase.
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

        gSkillEtcText.text = $"{gCooltime} / {gRange}  / {gMental}";
        //infoGSkillEtc.text = $"쿨타임 {unitData.GeneralSkill.CoolTime}초 / 사거리 {unitData.GeneralSkill.Range}보 / 멘탈 요구 {unitData.GeneralSkill.ActiveMental}";

        // special skill
        unitSSkillImage.sprite = unitData.SpecialSkill.Icon;
        sSkilNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.SpecialSkill.Name}_name", LocalizationSettings.SelectedLocale);
        sSkillInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.SpecialSkill.Name}_desc", LocalizationSettings.SelectedLocale);
        sSkillEffectText.text = LocalizationSettings.StringDatabase.
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

        sSkillEtcText.text = $"{sCooltime} / {sRange} / {sMental}";
        //infoSSkillEtc.text = $"쿨타임 {unitData.SpecialSkill.CoolTime}초 / 사거리 {unitData.SpecialSkill.Range}보 / 멘탈 요구 {unitData.SpecialSkill.ActiveMental}";

        if (unitData.SpecialAbility != null)
        {

            // 아이콘이 있는 경우 알파값1로 변경
            specialAbilityImage.sprite = unitData.SpecialAbility.Icon;
            specialAbilityUI.SetActive(true);
            //specialAbilityImage.gameObject.SetActive(true);
            //specialAbilityImage.color = new Color(1, 1, 1, 1f);
            //specialAbilityImage.sprite = unitData.SpecialAbility.Icon;

            sAbilityNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("SpecialAbility", $"{unitData.SpecialAbility.Id}_name", LocalizationSettings.SelectedLocale);
            sAbilityInfoText.text = LocalizationSettings.StringDatabase.
                GetLocalizedString("SpecialAbility", $"{unitData.SpecialAbility.Id}_desc", LocalizationSettings.SelectedLocale);
            sAbilityEffectText.text = LocalizationSettings.StringDatabase.
                GetLocalizedString("SpecialAbility", $"{unitData.SpecialAbility.Id}_effect", LocalizationSettings.SelectedLocale);


        }
        else
        {
            specialAbilityUI.SetActive(false);
            //specialAbilityImage.gameObject.SetActive(false);

            // 아이콘이 없는 경우 알파값0으로 변경
            //passiveIcon.color = new Color(1, 1, 1, 0f);
            //infoAbilityName.text = "";
            //infoAbilityDescript.text = "";
            //infoAbilityEffect.text = "";
            //passiveIcon.gameObject.SetActive(false);
        }
    }
}
