using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectUI : MonoBehaviour
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

        unitMenuUI.PerformModeChange((AllyUnit)selectedUnit);
        unitMenuUI.PerformUpgrade((AllyUnit)selectedUnit);
    }

    public void HideHp()
    {
        unitHPPrefab.SetActive(false);
        selectedUnit = null;
    }

    public void ShowAllyUI(AllyUnit allyUnit)
    {
        selectedUnit = allyUnit;
        unitMenuPrefab.SetActive(true);


    }

    public void HideAllyUI()
    {
        unitMenuPrefab.SetActive(false);
        selectedUnit = null;
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
}
