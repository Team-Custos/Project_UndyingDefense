using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeMenuUI : MonoBehaviour
{
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private SelectedUnitManager selectedUnitManager;
    [SerializeField] private SelectedUnitUI selectedUnitUI;
    [SerializeField] private UnitDataLoader unitDataLoader;

    [SerializeField] private Image[] upgardeImage;

    [Header(" ■ 선택된 유닛")]
    [SerializeField] private Image selectedUnitBackImage;
    [SerializeField] private Image selectedUnitImage;
    [SerializeField] private Text selectedUnitNameText;
    [SerializeField] private Image selectedUnitAtTypeImage;
    [SerializeField] private Image selectedUnitDfTypeImage;
    [SerializeField] private Image[] selectedUnitTierImage;

    [Header(" ■ 첫번째 업그레이드 유닛")]
    [SerializeField] private Image firstUpgradeUnitBackImage;
    [SerializeField] private Image firstUpgradeUnitImage;
    [SerializeField] private Button firstUpgradeBtn;
    [SerializeField] private Text firstUpgradeUnitNameText;
    [SerializeField] private Image firstUpgradeUnitAtTypeImage;
    [SerializeField] private Image firstUpgradeUnitDfTypeImage;
    [SerializeField] private Image[] firstUpgradeUnitTierImage;
    [SerializeField] private Image lockImage;

    [Header(" ■ 두번째 업그레이드 유닛")]
    [SerializeField] private Image secondUpgradeUnitBackImage;
    [SerializeField] private Button secondUpgradeBtn;
    [SerializeField] private Image secondUpgradeUnitImage;
    [SerializeField] private Text secondUpgradeUnitNameText;
    [SerializeField] private Image secondUpgradeUnitAtTypeImage;
    [SerializeField] private Image secondUpgradeUnitDfTypeImage;
    [SerializeField] private Image[] secondUpgradeUnitTierImage;

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
    [SerializeField] private Text infoGSkillDescript;
    [SerializeField] private Text infoSSkillDescript;
    [SerializeField] private Text infoMentalText;
    [SerializeField] private Text infoAttackRangeText;
    [SerializeField] private Text infoRecommendedRoleText;

    [SerializeField] private RectTransform leftPos;
    [SerializeField] private RectTransform middlePos;
    [SerializeField] private Transform selectedUI;

    private UnitData currentUnitData;
    private UnitData firstUnitData;
    private UnitData secondUnitData;

    private Unit currentUnit;
    private Unit firstUpgradeUnit;
    private Unit secondUpgradeUnit;

    [SerializeField] private  GameObject twoLine;
    [SerializeField] private    GameObject oneLine;

    [SerializeField] private Text currentGoldText;
    [SerializeField] private Text upgradeCostTxt;
    [SerializeField] private Button upgradePerformBtn;

    [SerializeField] private Sprite[] unitBackImage;

    private int upgradeIndex = -1;
    private float cost;


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
                UnitStats unitStats = unitDataLoader.GetUnitDataById(firstUpgradeUnit.UnitId, firstUpgradeUnit);

                firstUpgradeUnit.SetUnitStatsByUpgradeUI(unitStats);

                infoText.text = unitStats.unitName;
                infoCrtiText.text = "치명타율 : " + unitStats.critChance.ToString();
                infoMoveSpeedText.text = "이동속도 : " + unitStats.moveSpeed.ToString();
                infoAttackSpeedText.text = "공격속도 : " + unitStats.attackSpeed.ToString();
                infoMentalText.text = "멘탈 : " + unitStats.mental.ToString();
                infoAttackRangeText.text = "공격범위 : " + unitStats.attackRange.ToString() + "칸";
                infoRecommendedRoleText.text = "추천역할 : " + unitStats.role;
                infoHpText.text = currentUnit.Maxhp.ToString() + " + " + (unitStats.maxHp - currentUnit.Maxhp).ToString();
                beforeHp.fillAmount = currentUnit.Maxhp / 500; //firstUnitData.MaxHp;
                afterHp.fillAmount = unitStats.maxHp / 500; // firstUnitData.MaxHp;
                //infoMentalText.text = currentUnit.Mental.ToString() + " + " + unitStats.mental.ToString();

                

                infoGSkillImage.sprite = firstUpgradeUnit.GeneralSkill.Data.Icon;
                infoGSkillText.text = firstUpgradeUnit.GeneralSkill.Data.Name;
                infoSSkillImage.sprite = firstUpgradeUnit.SpecialSkill.Data.Icon;
                infoSSkillText.text = firstUpgradeUnit.SpecialSkill.Data.Name;
                infoGSkillDescript.text = firstUpgradeUnit.GeneralSkill.Data.Description;
                infoSSkillDescript.text = firstUpgradeUnit.SpecialSkill.Data.Description;

            }
            else if (index == 1)
            {
                if (secondUnitData == null)
                    return;

                UnitStats unitStats = unitDataLoader.GetUnitDataById(secondUpgradeUnit.UnitId, secondUpgradeUnit);

                secondUpgradeUnit.SetUnitStatsByUpgradeUI(unitStats);

                infoText.text = unitStats.unitName;
                infoCrtiText.text = "치명타율 : " + unitStats.critChance.ToString();
                infoMoveSpeedText.text = "이동속도 : " + unitStats.moveSpeed.ToString();
                infoAttackSpeedText.text = "공격속도 : " + unitStats.attackSpeed.ToString();
                infoMentalText.text = "멘탈 : " + unitStats.mental.ToString();
                infoAttackRangeText.text = "공격범위 : " + unitStats.attackRange.ToString() + "칸";
                infoRecommendedRoleText.text = "추천역할 : " + unitStats.role;
                infoHpText.text = currentUnit.Maxhp.ToString() + " + " + (unitStats.maxHp - currentUnit.Maxhp).ToString();
                beforeHp.fillAmount = currentUnit.Maxhp / 500;// secondUnitData.MaxHp;
                afterHp.fillAmount = unitStats.maxHp / 500; // secondUnitData.MaxHp;
                //infoMentalText.text = currentUnit.Mental.ToString() + " + " + unitStats.mental.ToString();

                infoGSkillImage.sprite = secondUpgradeUnit.GeneralSkill.Data.Icon;
                infoGSkillText.text = secondUpgradeUnit.GeneralSkill.Data.Name;
                infoGSkillDescript.text = secondUpgradeUnit.GeneralSkill.Data.Description;

                infoSSkillImage.sprite = secondUpgradeUnit.SpecialSkill.Data.Icon;
                infoSSkillText.text = secondUpgradeUnit.SpecialSkill.Data.Name;
                infoSSkillDescript.text = secondUpgradeUnit.SpecialSkill.Data.Description;
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
        selectedUnitNameText.text = selectedUnit.Data.Name;
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
                firstUpgradeUnitNameText.text = firstUpgradeUnitData.Name;
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
                firstUpgradeUnitNameText.text = firstUpgradeUnitData.Name;
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
                secondUpgradeUnitNameText.text = secondUpgradeUnitData.Name;
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
        selectedUI.gameObject.SetActive(true);
        Vector3 targetPos = upgardeImage[index].transform.position;
        selectedUI.position = new Vector3(targetPos.x, targetPos.y, selectedUI.position.z);
    }
}
