using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitUI : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject unitHPPrefab;
    [SerializeField] private Image unitHP;
    [SerializeField] private RectTransform hpRectTransform;

    [SerializeField] private UnitMenuUI unitMenuUI;
    [SerializeField] private GameObject unitMenuPrefab;

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
        unitHPPrefab.SetActive(false);
        unitInfoImage.gameObject.SetActive(false);

        selectedUnit = null;
    }

    public void ShowAllyUI(AllyUnit allyUnit, AllyUnitData allyUnitData)
    {
        selectedUnit = allyUnit;
        unitMenuPrefab.SetActive(true);

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
        }
    }

    public void UpdateUnitInfo(Unit unit)
    {
       unit.Initialize(this);

       unitInfoImage.gameObject.SetActive(true);

       unitHPText.text = unit.SetUnitHPUI();
       unitImage.sprite = unit.Data.Icon;
       unitNameText.text = unit.Data.Name;
       unitDefenseTypeText.text = unit.Data.ArmorType.ToString();
       unitGSkillText.text = unit.Data.GSkillText;
       unitSSkillText.text = unit.Data.SSkillText;
       unitSSkillImage.sprite = unit.Data.GSkillIcon;
       unitGSkillImage.sprite = unit.Data.SSkillIcon;
    }

    public void UpdateHPUI(Unit unit)
    {
        if(unit != null && selectedUnit == unit)
        {
            unitHPText.text = unit.SetUnitHPUI();
        }
    }
}