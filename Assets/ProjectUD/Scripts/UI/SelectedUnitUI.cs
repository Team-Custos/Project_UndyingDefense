using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitUI : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject unitHPPrefab;
    [SerializeField] private Image unitHP;
    [SerializeField] private RectTransform hpRectTransform;

    [SerializeField] private GameObject unitMenuPrefab;
    [SerializeField] private GameObject unitUpgradeMenuPrefab;

    [SerializeField] private Image modeChangeBtnImage;
    [SerializeField] private Sprite freeIcon;
    [SerializeField] private Sprite siegeIcon;

    private Unit selectedUnit;
    [SerializeField] private float yPos;
    [SerializeField] private float xPos;

    [Header("■ UntiInfo")]
    [SerializeField] private Image unitInfoImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitHPText;
    [SerializeField] private TextMeshProUGUI unitGSkillText;
    [SerializeField] private TextMeshProUGUI unitSSkillText;
    [SerializeField] private TextMeshProUGUI unitDefenseTypeText;
    [SerializeField] private Image unitSSkillImage;
    [SerializeField] private Image unitGSkillImage;


    // Update is called once per frame
    void Update()
    {
        if (selectedUnit != null)
        {
            UpdateUI();
        }
    }

    public void ShowHp(Unit unit)
    {
        selectedUnit = unit;
        unitHPPrefab.SetActive(true);

    }

    public void HideHp()
    {
        if(unitHPPrefab != null)
        {
            unitHPPrefab.SetActive(false);
            unitInfoImage.gameObject.SetActive(false);

            selectedUnit = null;
        }
        
    }

    public void HideUpgrdeUI()
    {
        unitUpgradeMenuPrefab.SetActive(false);
    }

    public void ShowUpgradeMenu()
    {
        unitMenuPrefab.SetActive(false);
        unitUpgradeMenuPrefab.SetActive(true);
    }

    public void ShowAllyUI(AllyUnit allyUnit, AllyUnitData allyUnitData)
    {
        selectedUnit = allyUnit;
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
        if(selectedUnit != null)
        {
            if (unitHPPrefab != null)
            {
                unitHP.fillAmount = selectedUnit.HpPercent;

                Vector3 worldPosition = selectedUnit.transform.position + Vector3.up * yPos;
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

                unitHPPrefab.transform.position = screenPosition;
            }

            if(unitMenuPrefab != null)
            {
                Vector3 worldPosition = selectedUnit.transform.position + Vector3.right * xPos;
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

                unitMenuPrefab.transform.position = screenPosition;
            }

            if(unitUpgradeMenuPrefab != null)
            {
                Vector3 worldPosition = selectedUnit.transform.position + Vector3.right * xPos;
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
       unitDefenseTypeText.text = ConvertDefenseName(unit.Data.ArmorType.ToString());
       unitGSkillText.text = unit.GeneralSkill.Data.name;

       if(unit.SpecialSkill != null)
        {
            unitSSkillText.text = unit.SpecialSkill.Data.name;
            unitGSkillImage.sprite = unit.SpecialSkill.Data.Icon;
        }
        else
        {
            unitSSkillText.text = " ";
            unitGSkillImage.sprite = null;
        }

       unitSSkillImage.sprite = unit.GeneralSkill.Data.Icon;
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
}