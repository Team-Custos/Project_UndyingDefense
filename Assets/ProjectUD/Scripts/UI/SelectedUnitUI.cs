using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitUI : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SelectedUnitManager selecteUnitManger;
    [SerializeField] private UpgradeMenuUI upgradeMenuUI;

    [SerializeField] private GameObject unitHPPrefab;
    [SerializeField] private Image unitHP;
    [SerializeField] private RectTransform hpRectTransform;

    [SerializeField] private GameObject unitMenuPrefab;
    [SerializeField] private GameObject unitUpgradeMenuPrefab;

    [SerializeField] private Image modeChangeBtnImage;
    [SerializeField] private Sprite freeIcon;
    [SerializeField] private Sprite siegeIcon;
    [SerializeField] private float yPos;
    [SerializeField] private float xPos;

    [Header("■ UntiInfo")]
    [SerializeField] private Image unitInfoImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitHPText;
    [SerializeField] private Image unitHPImage;
    [SerializeField] private TextMeshProUGUI unitMentalText;
    //[SerializeField] private Image unitMentalImage;  나중에 작업
    [SerializeField] private Image atTypeIcon;
    [SerializeField] private Image dfTypeIcon;
    [SerializeField] private Image unitSSkillImage;
    [SerializeField] private Image unitGSkillImage;
    [SerializeField] private Text critText;
    [SerializeField] private Text moveSpeedText;
    [SerializeField] private Text atSpeedText;
    [SerializeField] private Image[] tierImage;
    [SerializeField] private Text gSkillInfoText;
    [SerializeField] private Text sSkillInfoText;

    [SerializeField] private TextMeshProUGUI unitGSkillText;
    [SerializeField] private TextMeshProUGUI unitSSkillText;
    [SerializeField] private TextMeshProUGUI unitDefenseTypeText;


    // Update is called once per frame
    void Update()
    {
        if (selecteUnitManger.SelectedUnit != null)
        {
            UpdateUI();
        }
    }

    public void ShowHp(Unit unit)
    {
        unit = selecteUnitManger.SelectedUnit; 
        unitHPPrefab.SetActive(true);

    }

    public void HideHp()
    {
        if(unitHPPrefab != null)
        {
            unitHPPrefab.SetActive(false);
            unitInfoImage.gameObject.SetActive(false);
        }
        
    }

    public void HideUpgrdeUI()
    {
        unitUpgradeMenuPrefab.SetActive(false);
    }

    public void ShowUpgradeMenu(Unit unit)
    {
        if (unit.Data.Tier >= 4)
            return;

        unitMenuPrefab.SetActive(false);
        unitUpgradeMenuPrefab.SetActive(true);
        upgradeMenuUI.SetUnitUpgradeMenu(unit);
    }

    public void ShowAllyUI(AllyUnit allyUnit, AllyUnitData allyUnitData)
    {
        allyUnit = (AllyUnit)selecteUnitManger.SelectedUnit;
        unitMenuPrefab.SetActive(true);

        if(allyUnit.ModeType == AllyUnit.Mode.FREE)
        {
            modeChangeBtnImage.sprite = siegeIcon;
        }
        else if(allyUnit.ModeType == AllyUnit.Mode.SEIGE)
        {
            modeChangeBtnImage.sprite = freeIcon;
        }

        //unitMenuUI.PerformModeChange((AllyUnit)selectedUnit);


        //unitMenuUI.PerformUpgrade((AllyUnit)selectedUnit, allyUnitData, upgradeOption);
    }

    public void HideAllyUI()
    {
        unitMenuPrefab.SetActive(false);
        //selectedUnit = null;
    }

    private void UpdateUI()
    {
        if(selecteUnitManger.SelectedUnit != null)
        {
            if (unitHPPrefab != null)
            {
                unitHP.fillAmount = selecteUnitManger.SelectedUnit.HpPercent;

                Vector3 worldPosition = selecteUnitManger.SelectedUnit.transform.position + Vector3.up * yPos;
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

                unitHPPrefab.transform.position = screenPosition;
            }

            if(unitMenuPrefab != null)
            {
                Vector3 worldPosition = selecteUnitManger.SelectedUnit.transform.position + Vector3.right * xPos;
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

                unitMenuPrefab.transform.position = screenPosition;
            }

            if(unitUpgradeMenuPrefab != null)
            {
                Vector3 worldPosition = selecteUnitManger.SelectedUnit.transform.position + Vector3.right * xPos;
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

                unitUpgradeMenuPrefab.transform.position = screenPosition;
            }
        }
    }


    public void UpdateUnitInfo(Unit unit)
    {
       unit.SetUnitUI(this);

       unitInfoImage.gameObject.SetActive(true);

       UpdateHPUI(unit);
       unitImage.sprite = unit.Data.Icon;
       unitNameText.text = unit.Data.Name;
       unitMentalText.text = unit.Data.Mental.ToString();

        SetUnitTierIcon(unit.Data.Tier);

        atTypeIcon.sprite = unit.Data.AtTypeIcon;
       dfTypeIcon.sprite = unit.Data.DfTypeIcon;

       unitDefenseTypeText.text = ConvertDefenseName(unit.Data.ArmorType.ToString());
       unitGSkillText.text = unit.GeneralSkill.Data.name;

        

        if (unit.SpecialSkill != null)
        {
            unitSSkillText.text = unit.SpecialSkill.Data.name;

            unitGSkillImage.gameObject.SetActive(true);
            unitGSkillImage.sprite = unit.SpecialSkill.Data.Icon;

            gSkillInfoText.text = unit.SpecialSkill.Data.Description;
            sSkillInfoText.text = unit.GeneralSkill.Data.Description;

        }
        else
        {
            unitSSkillText.text = " ";
            unitGSkillImage.gameObject.SetActive(false);
            
        }

       unitSSkillImage.sprite = unit.GeneralSkill.Data.Icon;

        critText.text = "치명타 율 : " + unit.Data.CritChance.ToString() + "%";
        moveSpeedText.text = "이동속도 : " + unit.Data.MoveSpeed.ToString();
        atSpeedText.text = "공격속도 : " + unit.Data.AttackSpeed.ToString();
    }
    
    public void HideUntInfo()
    {
        unitInfoImage.gameObject.SetActive(false);
    }

    public void UpdateHPUI(Unit unit)
    {
        if(unit != null)
        {
            unitHPText.text = $"{unit.Hp} / {unit.Data.MaxHp}";
            unitHPImage.fillAmount = unit.HpPercent;
        }
    }

    public string ConvertDefenseName(string armorType)
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

    public void SetUnitTierIcon(int tier)
    {
        for (int i = 0; i < tierImage.Length; i++)
        {
            tierImage[i].gameObject.SetActive(i < tier);
        }
    }
}