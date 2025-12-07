using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UpgradeMenuUI : MonoBehaviour
{
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private SelectedUnitManager selectedUnitManager;
    [SerializeField] private SelectedUnitUI selectedUnitUI;
    [SerializeField] private UnitDataLoader unitDataLoader;

    [SerializeField] private Image[] upgardeImage;

    [Header("NameTextTable")]
    [SerializeField] private FactionNameTextTable fNameTextTable;

    [Header(" ■ 선택된 유닛")]
    [SerializeField] private Image selectedUnitBackImage;
    [SerializeField] private Image selectedUnitImage;
    [SerializeField] private TextMeshProUGUI selectedUnitNameText;
    [SerializeField] private Image selectedUnitAtTypeImage;
    [SerializeField] private Image selectedUnitDfTypeImage;
    [SerializeField] private Image[] selectedUnitTierImage;

    [Header(" ■ 첫번째 업그레이드 유닛")]
    [SerializeField] private Image firstUpgradeUnitBackImage;
    [SerializeField] private Image firstUpgradeUnitImage;
    [SerializeField] private Button firstUpgradeBtn;
    [SerializeField] private TextMeshProUGUI firstUpgradeUnitNameText;
    [SerializeField] private Image firstUpgradeUnitAtTypeImage;
    [SerializeField] private Image firstUpgradeUnitDfTypeImage;
    [SerializeField] private Image[] firstUpgradeUnitTierImage;
    [SerializeField] private Image lockImage;

    [Header(" ■ 두번째 업그레이드 유닛")]
    [SerializeField] private Image secondUpgradeUnitBackImage;
    [SerializeField] private Button secondUpgradeBtn;
    [SerializeField] private Image secondUpgradeUnitImage;
    [SerializeField] private TextMeshProUGUI secondUpgradeUnitNameText;
    [SerializeField] private Image secondUpgradeUnitAtTypeImage;
    [SerializeField] private Image secondUpgradeUnitDfTypeImage;
    [SerializeField] private Image[] secondUpgradeUnitTierImage;

    [Header(" ■ 업그레이드할 유니의 정보")]
    [SerializeField]private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Image beforeHp;
    [SerializeField] private Image afterHp;
    [SerializeField] private TextMeshProUGUI infoHpText;
    [SerializeField] private TextMeshProUGUI infoCrtiText;
    [SerializeField] private TextMeshProUGUI infoMoveSpeedText;
    [SerializeField] private TextMeshProUGUI infoAttackSpeedText;
    [SerializeField] private Image infoGSkillImage;
    [SerializeField] private TextMeshProUGUI infoGSkillText;
    [SerializeField] private Image infoSSkillImage;
    [SerializeField] private TextMeshProUGUI infoSSkillText;
    [SerializeField] private TextMeshProUGUI infoGSkillDescript;
    [SerializeField] private TextMeshProUGUI infoSSkillDescript;
    [SerializeField] private TextMeshProUGUI infoGSkillEffect;
    [SerializeField] private TextMeshProUGUI infoSSkillEffect;
    [SerializeField] private TextMeshProUGUI infoMentalText;
    [SerializeField] private TextMeshProUGUI infoAttackRangeText;
    [SerializeField] private TextMeshProUGUI infoRecommendedRoleText;

    [SerializeField] private RectTransform leftPos;
    [SerializeField] private RectTransform middlePos;

    [SerializeField] private Transform selectedUI;
    [SerializeField] private Sprite selectIcon;
    [SerializeField] private Sprite frameIcon;
    [SerializeField] private Image[] frameImage;
    private int selectedIndex = -1;

    private UnitData currentUnitData;
    private UnitData firstUnitData;
    private UnitData secondUnitData;

    private Unit currentUnit;
    private Unit firstUpgradeUnit;
    private Unit secondUpgradeUnit;

    [SerializeField] private  GameObject twoLine;
    [SerializeField] private    GameObject oneLine;

    [SerializeField] private TextMeshProUGUI currentGoldText;
    [SerializeField] private TextMeshProUGUI upgradeCostTxt;
    [SerializeField] private Button upgradePerformBtn;

    [SerializeField] private Sprite[] unitBackImage;

    private int upgradeIndex = -1;
    private float cost;

    private void FieldStatLocalization(UnitStats unitStats)
    {
        string critT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_critChance", LocalizationSettings.SelectedLocale);
        infoCrtiText.text = $"{critT} : " + unitStats.critChance.ToString() + "%";

        string moveSpeedT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_moveSpeed", LocalizationSettings.SelectedLocale);
        infoMoveSpeedText.text = $"{moveSpeedT} : " + unitStats.moveSpeed.ToString();
        //--
        string attackSText = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_attackSpeed", LocalizationSettings.SelectedLocale);
        string atSpeedId = fNameTextTable.GetName("Interval_" + $"{unitStats.interval.ToString()}");

        infoAttackSpeedText.text = $"{attackSText} : " + LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", $"{atSpeedId}", LocalizationSettings.SelectedLocale);
        //--
        string attackRangeT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_attackRange", LocalizationSettings.SelectedLocale);
        infoAttackRangeText.text = $"{attackRangeT} : " + (unitStats.attackRange / 2).ToString() + "칸";

        string mentalT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_mental", LocalizationSettings.SelectedLocale);
        infoMentalText.text = $"{mentalT} : " + unitStats.mental.ToString();

        string roleT = LocalizationSettings.StringDatabase.
                    GetLocalizedString("CommonUI", "CON_role", LocalizationSettings.SelectedLocale);
        string roleId = fNameTextTable.GetName(unitStats.role.ToString());

        infoRecommendedRoleText.text = $"{roleT} :" + LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", $"{roleId}", LocalizationSettings.SelectedLocale);
    }

    private void FieldNameLocalization(UnitData data, TextMeshProUGUI text)
    {
        text.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{data.Id}_name", LocalizationSettings.SelectedLocale);
    }

    private void FieldSkillLocalization(Unit unit)
    {
        //--Local 스킬이름
        infoGSkillText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.GeneralSkill.Data.Name}_name", LocalizationSettings.SelectedLocale);
        infoSSkillText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_name", LocalizationSettings.SelectedLocale);
        //--Local 스킬 설명 (desc + effect)
        infoGSkillDescript.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.GeneralSkill.Data.Name}_desc", LocalizationSettings.SelectedLocale);
        infoGSkillEffect.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.GeneralSkill.Data.Name}_effect", LocalizationSettings.SelectedLocale);

        infoSSkillDescript.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_desc", LocalizationSettings.SelectedLocale);
        infoSSkillEffect.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_effect", LocalizationSettings.SelectedLocale);
    }

    public void ToggleUpgradeUnit(int index)
    {

        if (upgradeIndex == index)
        {
            upgradeIndex = -1;
            infoPanel.SetActive(false);
            selectedUI.gameObject.SetActive(false);
            upgradePerformBtn.interactable = false;

            SoundManager.Instance.PlayUIClickSFX();

        }
        else
        {
            upgradeIndex = index;

            if (cost <= inGameManager.inGameGold)
            {
                upgradePerformBtn.interactable = true;
            }
            else // 돈 부족
            {
                upgradePerformBtn.interactable = false;
            }


            if (index == 0)
            {
                UnitStats unitStats = unitDataLoader.GetUnitDataById(firstUpgradeUnit.UnitId);

                firstUpgradeUnit.SetUnitStatsByUpgradeUI(unitStats);

                //infoText.text = unitStats.unitName;
                //infoCrtiText.text = "치명타율 : " + unitStats.critChance.ToString();
                //infoMoveSpeedText.text = "이동속도 : " + unitStats.moveSpeed.ToString();
                //infoAttackSpeedText.text = "공격속도 : " + unitStats.attackSpeed;
                //infoMentalText.text = "멘탈 : " + unitStats.mental.ToString();
                //infoAttackRangeText.text = "공격범위 : " + unitStats.attackRange.ToString() + "칸";

                //FieldNameLocalization(firstUpgradeUnit.Data, infoText);
                //infoRecommendedRoleText.text = "추천역할 : " + unitStats.role;

                infoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{unitStats.id}_name", LocalizationSettings.SelectedLocale);
                FieldStatLocalization(unitStats);


                infoHpText.text = currentUnit.Maxhp.ToString() + " + " + (unitStats.maxHp - currentUnit.Maxhp).ToString();
                beforeHp.fillAmount = currentUnit.Maxhp / 500; //firstUnitData.MaxHp;
                afterHp.fillAmount = unitStats.maxHp / 500; // firstUnitData.MaxHp;
                //infoMentalText.text = currentUnit.Mental.ToString() + " + " + unitStats.mental.ToString();

                

                infoGSkillImage.sprite = firstUpgradeUnit.GeneralSkill.Data.Icon;
                infoSSkillImage.sprite = firstUpgradeUnit.SpecialSkill.Data.Icon;
                //infoGSkillText.text = firstUpgradeUnit.GeneralSkill.Data.Name;
                //infoSSkillText.text = firstUpgradeUnit.SpecialSkill.Data.Name;
                //infoGSkillDescript.text = firstUpgradeUnit.GeneralSkill.Data.Description;
                //infoSSkillDescript.text = firstUpgradeUnit.SpecialSkill.Data.Description;
                FieldSkillLocalization(firstUpgradeUnit);

            }
            else if (index == 1)
            {
                if (secondUnitData == null)
                    return;

                UnitStats unitStats = unitDataLoader.GetUnitDataById(secondUpgradeUnit.UnitId);

                secondUpgradeUnit.SetUnitStatsByUpgradeUI(unitStats);

                //infoText.text = unitStats.unitName;
                //infoCrtiText.text = "치명타율 : " + unitStats.critChance.ToString();
                //infoMoveSpeedText.text = "이동속도 : " + unitStats.moveSpeed.ToString();
                //infoAttackSpeedText.text = "공격속도 : " + unitStats.attackSpeed;
                //infoMentalText.text = "멘탈 : " + unitStats.mental.ToString();
                //infoAttackRangeText.text = "공격범위 : " + unitStats.attackRange.ToString() + "칸";

                //FieldNameLocalization (secondUpgradeUnit.Data, infoText);
                //infoRecommendedRoleText.text = "추천역할 : " + unitStats.role;

                infoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{unitStats.id}_name", LocalizationSettings.SelectedLocale);
                FieldStatLocalization(unitStats);
                infoHpText.text = currentUnit.Maxhp.ToString() + " + " + (unitStats.maxHp - currentUnit.Maxhp).ToString();
                beforeHp.fillAmount = currentUnit.Maxhp / 500;// secondUnitData.MaxHp;
                afterHp.fillAmount = unitStats.maxHp / 500; // secondUnitData.MaxHp;
                //infoMentalText.text = currentUnit.Mental.ToString() + " + " + unitStats.mental.ToString();

                infoGSkillImage.sprite = secondUpgradeUnit.GeneralSkill.Data.Icon;
                infoSSkillImage.sprite = secondUpgradeUnit.SpecialSkill.Data.Icon;

                FieldSkillLocalization(secondUpgradeUnit);
                //infoGSkillText.text = secondUpgradeUnit.GeneralSkill.Data.Name;
                //infoGSkillDescript.text = secondUpgradeUnit.GeneralSkill.Data.Description;

                //infoSSkillText.text = secondUpgradeUnit.SpecialSkill.Data.Name;
                //infoSSkillDescript.text = secondUpgradeUnit.SpecialSkill.Data.Description;
            }

            Select(index);

            SoundManager.Instance.PlayUIClickSFX();

            infoPanel.SetActive(true);

        }

    }

    public void PerformUpgrade()
    {
        if (upgradeIndex == -1)
            return;
        selectedUI.gameObject.SetActive(false);
        selectedUnitManager.UpgradeSelectedUnit(upgradeIndex);
        SoundManager.Instance.PlayUIClickSFX();
    }

    public void SetUnitUpgradeMenu(Unit selectedUnit)
    {
        if (selectedUnit is EnemyUnit)
            return;


        infoPanel.SetActive(false);
        upgradePerformBtn.interactable = false;

        upgradeIndex = -1;

        currentUnitData = selectedUnit.Data;
        currentUnit = selectedUnit;
        if (currentUnit.UnitStats == null)
            Debug.Log("fef");


        firstUpgradeBtn.interactable = true;
        lockImage.gameObject.SetActive(false);

        // 현재 선택된 유닛
        selectedUnitImage.sprite = selectedUnit.Data.Icon;
        //selectedUnitNameText.text = selectedUnit.Data.Name;
        FieldNameLocalization(selectedUnit.Data, selectedUnitNameText);

        selectedUnitAtTypeImage.sprite = selectedUnit.Data.AtTypeIcon;
        selectedUnitDfTypeImage.sprite = selectedUnit.Data.DfTypeIcon;

        for (int i = 0; i < selectedUnitTierImage.Length; i++)
        {
            selectedUnitTierImage[i].gameObject.SetActive(i < selectedUnit.Data.Tier);
        }


        AllyUnitData allyUnitData = (AllyUnitData)currentUnitData;

        if (allyUnitData.UpgradeUnits.Length <= 0)
            return;

        // 업그레이드 가능이 한가지인 경우
        if(allyUnitData.UpgradeUnits.Length <= 1 || currentUnitData.Tier >= 3)
        {
            if(currentUnitData.Name == "언월도병")
            {
                firstUpgradeBtn.interactable = false;
                lockImage.gameObject.SetActive(true);
            }

            secondUpgradeUnitBackImage.gameObject.SetActive(false);
            twoLine.SetActive(false);
            oneLine.SetActive(true);
            firstUpgradeUnitBackImage.rectTransform.position = middlePos.position;

            UnitData firstUpgradeUnitData = allyUnitData.UpgradeUnits[0];
            firstUnitData = firstUpgradeUnitData;
            firstUpgradeUnit = firstUpgradeUnitData.Prefab.GetComponent<Unit>();

            if (firstUpgradeUnitData != null)
            {
                // 첫번째 업그레이드 유닛
                firstUpgradeUnitImage.sprite = firstUpgradeUnitData.Icon;
                //firstUpgradeUnitNameText.text = firstUpgradeUnitData.Name;
                FieldNameLocalization(firstUpgradeUnitData, firstUpgradeUnitNameText);
                firstUpgradeUnitAtTypeImage.sprite = firstUpgradeUnitData.AtTypeIcon;
                firstUpgradeUnitDfTypeImage.sprite = firstUpgradeUnitData.DfTypeIcon;

                for (int i = 0; i < firstUpgradeUnitTierImage.Length; i++)
                {
                    if (i < firstUpgradeUnitData.Tier)
                        firstUpgradeUnitTierImage[i].gameObject.SetActive(true); // 켜기
                    else
                        firstUpgradeUnitTierImage[i].gameObject.SetActive(false); // 끄기
                }

                AllyUnitData upgradeUnitData = (AllyUnitData)firstUpgradeUnitData;

                cost = upgradeUnitData.Cost;

                if (inGameManager.inGameGold < upgradeUnitData.Cost) // 돈 부족
                {
                    upgradePerformBtn.interactable = false;
                    currentGoldText.color = Color.red;
                    currentGoldText.text = inGameManager.inGameGold.ToString();
                    upgradeCostTxt.text =  upgradeUnitData.Cost.ToString();
                }
                else
                {
                    //upgradePerformBtn.interactable = true;
                    currentGoldText.color = Color.white;
                    currentGoldText.text = inGameManager.inGameGold.ToString();
                    upgradeCostTxt.text = upgradeUnitData.Cost.ToString();
                }

                
            }

            return;

        }

        // 업그레이드 가능이 두가지인 경우
        if (currentUnitData.Tier < 3 || allyUnitData.UpgradeUnits.Length >= 2)
        {
            firstUpgradeUnitBackImage.rectTransform.position = leftPos.position;

            secondUpgradeUnitBackImage.gameObject.SetActive(true);
            twoLine.SetActive(true);
            oneLine.SetActive(false);
        

            secondUpgradeBtn.interactable = true;

            UnitData firstUpgradeUnitData = allyUnitData.UpgradeUnits[0];
            UnitData secondUpgradeUnitData = allyUnitData.UpgradeUnits[1];

            firstUnitData = firstUpgradeUnitData;
            firstUpgradeUnit = firstUpgradeUnitData.Prefab.GetComponent<Unit>();

            secondUnitData = secondUpgradeUnitData;
            secondUpgradeUnit = secondUpgradeUnitData.Prefab.GetComponent<Unit>();

            if (firstUpgradeUnitData != null)
            {
                // 첫번째 업그레이드 유닛
                firstUpgradeUnitImage.sprite = firstUpgradeUnitData.Icon;
                //firstUpgradeUnitNameText.text = firstUpgradeUnitData.Name;
                FieldNameLocalization(firstUpgradeUnitData, firstUpgradeUnitNameText);
                firstUpgradeUnitAtTypeImage.sprite = firstUpgradeUnitData.AtTypeIcon;
                firstUpgradeUnitDfTypeImage.sprite = firstUpgradeUnitData.DfTypeIcon;

                for (int i = 0; i < firstUpgradeUnitTierImage.Length; i++)
                {
                    if (i < firstUpgradeUnitData.Tier)
                        firstUpgradeUnitTierImage[i].gameObject.SetActive(true); // 켜기
                    else
                        firstUpgradeUnitTierImage[i].gameObject.SetActive(false); // 끄기
                }
            }

            if (secondUpgradeUnitData != null)
            {
                // 두번째 업그레이드 유닛
                secondUpgradeUnitImage.sprite = secondUpgradeUnitData.Icon;
                //secondUpgradeUnitNameText.text = secondUpgradeUnitData.Name;
                FieldNameLocalization(secondUpgradeUnitData, secondUpgradeUnitNameText);
                secondUpgradeUnitAtTypeImage.sprite = secondUpgradeUnitData.AtTypeIcon;
                secondUpgradeUnitDfTypeImage.sprite = secondUpgradeUnitData.DfTypeIcon;

                for (int i = 0; i < secondUpgradeUnitTierImage.Length; i++)
                {
                    if (i < secondUpgradeUnitData.Tier)
                        secondUpgradeUnitTierImage[i].gameObject.SetActive(true); // 켜기
                    else
                        secondUpgradeUnitTierImage[i].gameObject.SetActive(false); // 끄기
                }
            }
            AllyUnitData upgradeUnitData = (AllyUnitData)firstUpgradeUnitData;

            cost = upgradeUnitData.Cost;

            if (inGameManager.inGameGold < upgradeUnitData.Cost) // 돈 부족
            {
                upgradePerformBtn.interactable = false;
                currentGoldText.color = Color.red;
                currentGoldText.text = inGameManager.inGameGold.ToString();
                upgradeCostTxt.text = upgradeUnitData.Cost.ToString();
            }
            else
            {
                //upgradePerformBtn.interactable = true;
                currentGoldText.color = Color.white;
                currentGoldText.text = inGameManager.inGameGold.ToString();
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

    public void UpdateUpgradeCostTxt()
    {
        currentGoldText.text = inGameManager.inGameGold.ToString();

        if(inGameManager.inGameGold >= cost)
        {
            currentGoldText.color = Color.white;
            upgradePerformBtn.interactable = true;
        }
    }

    public void HideUpgradeUI()
    {
        selectedUI.gameObject.SetActive(false);

        gameObject.SetActive(false);
        if(selectedUnitManager.SelectedUnit is AllyUnit)
            selectedUnitUI.ShowAllyUI((AllyUnit)selectedUnitManager.SelectedUnit);


    }


    public void Select(int index)
    {
        if (selectedIndex != -1 && selectedIndex != index)
        {
            Debug.Log(index);
            frameImage[index].sprite = frameIcon;
        }

        //selectedUI.gameObject.SetActive(true);
        //Vector3 targetPos = upgardeImage[index].transform.position;
        //selectedUI.position = new Vector3(targetPos.x, targetPos.y, selectedUI.position.z);
    }
}
