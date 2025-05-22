using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuUI : MonoBehaviour
{
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private SelectedUnitManager selectedUnitManager;

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

    [SerializeField] private RectTransform leftPos;
    [SerializeField] private RectTransform middlePos;

    private UnitData currentUnitData;
    private UnitData firstUnitData;
    private UnitData secondUnitData;

    [SerializeField] private  GameObject twoLine;
    [SerializeField] private    GameObject oneLine;

    [SerializeField] private Text currentGoldText;
    [SerializeField] private Text upgradeCostTxt;
    [SerializeField] private Button upgradePerformBtn;

    [SerializeField] private Sprite[] unitBackImage;

    private int upgradeIndex = -1;
    private float cost;

    //public void UpgradeToFirstUnit()
    //{
    //    selectedUnitManager.UpgradeSelectedUnit(0);
    //}

    //public void UpgradeToSecondUnit()
    //{
    //    selectedUnitManager.UpgradeSelectedUnit(1);
    //}

    public void ToggleUpgradeUnit(int index)
    {

        if (upgradeIndex == index)
        {
            upgradeIndex = -1;
            infoPanel.SetActive(false);
            upgradePerformBtn.interactable = false;

        }
        else
        {
            upgradeIndex = index;

            if (cost < inGameManager.inGameGold)
            {
                upgradePerformBtn.interactable = true;
            }
            else // 돈 부족
            {
                upgradePerformBtn.interactable = false;
            }


            if (index == 0)
            {
                infoText.text = firstUnitData.Name;
                infoCrtiText.text = "치명타율 : " + firstUnitData.CritChance.ToString();
                infoMoveSpeedText.text = "이동속도 : " + firstUnitData.MoveSpeed.ToString();
                infoAttackSpeedText.text = "공격속도 : " + firstUnitData.AttackSpeed.ToString();
                infoHpText.text = currentUnitData.MaxHp.ToString() + " + " + (firstUnitData.MaxHp - currentUnitData.MaxHp).ToString();
                beforeHp.fillAmount = currentUnitData.MaxHp / 500; //firstUnitData.MaxHp;
                afterHp.fillAmount = firstUnitData.MaxHp / 500; // firstUnitData.MaxHp;
                infoMentalText.text = currentUnitData.Mental.ToString() + " + " + firstUnitData.Mental.ToString();

                Unit firstUpgradeUnit = (selectedUnitManager.SelectedUnit.Data as AllyUnitData).UpgradeUnits[0].Prefab.GetComponent<Unit>();

                infoGSkillImage.sprite = firstUpgradeUnit.GeneralSkill.Data.Icon;
                infoGSkillText.text = firstUpgradeUnit.GeneralSkill.Data.Name;
                infoSSkillImage.sprite = firstUpgradeUnit.SpecialSkill.Data.Icon;
                infoSSkillText.text = firstUpgradeUnit.SpecialSkill.Data.Name;
                infoGSkillDescript.text = firstUpgradeUnit.GeneralSkill.Data.Description;
                infoSSkillDescript.text = firstUpgradeUnit.SpecialSkill.Data.Description;



                //infoGSkillImage.sprite = firstUnitData.GeneralSkill.Icon;
                //infoGSkillText.text = secondUnitData.GSkillName;
                //infoSSkillImage.sprite = secondUnitData.SSkillIcon;
                //infoSSkillText.text = secondUnitData.SSkillName;

            }
            else if (index == 1)
            {
                if (secondUnitData == null)
                    return;

                infoText.text = secondUnitData.Name;
                infoCrtiText.text = "치명타율 : " + secondUnitData.CritChance.ToString();
                infoMoveSpeedText.text = "이동속도 : " + secondUnitData.MoveSpeed.ToString();
                infoAttackSpeedText.text = "공격속도 : " + secondUnitData.AttackSpeed.ToString();
                infoHpText.text = currentUnitData.MaxHp.ToString() + " + " + (secondUnitData.MaxHp - currentUnitData.MaxHp).ToString();
                beforeHp.fillAmount = currentUnitData.MaxHp / 500;// secondUnitData.MaxHp;
                afterHp.fillAmount = secondUnitData.MaxHp / 500; // secondUnitData.MaxHp;
                infoMentalText.text = currentUnitData.Mental.ToString() + " + " + secondUnitData.Mental.ToString();

                Unit secondUpgradeUnit = (selectedUnitManager.SelectedUnit.Data as AllyUnitData).UpgradeUnits[1].Prefab.GetComponent<Unit>();
                infoGSkillImage.sprite = secondUpgradeUnit.GeneralSkill.Data.Icon;
                infoGSkillText.text = secondUpgradeUnit.GeneralSkill.Data.Name;
                infoGSkillDescript.text = secondUpgradeUnit.GeneralSkill.Data.Description;

                infoSSkillImage.sprite = secondUpgradeUnit.SpecialSkill.Data.Icon;
                infoSSkillText.text = secondUpgradeUnit.SpecialSkill.Data.Name;
                infoSSkillDescript.text = secondUpgradeUnit.SpecialSkill.Data.Description;
            }

            
            infoPanel.SetActive(true);

        }

    }

    public void PerformUpgrade()
    {
        if (upgradeIndex == -1)
            return;
        selectedUnitManager.UpgradeSelectedUnit(upgradeIndex);
    }

    public void SetUnitUpgradeMenu(Unit selectedUnit)
    {
        if (selectedUnit is EnemyUnit)
            return;

        infoPanel.SetActive(false);

        upgradeIndex = -1;

        currentUnitData = selectedUnit.Data;

        upgradePerformBtn.interactable = false;

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
                    upgradePerformBtn.interactable = true;
                    currentGoldText.text = inGameManager.inGameGold.ToString();
                    upgradeCostTxt.text = upgradeUnitData.Cost.ToString();
                }

                
            }

            return;

        }

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
            secondUnitData = secondUpgradeUnitData;

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
                upgradePerformBtn.interactable = true;
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


}
