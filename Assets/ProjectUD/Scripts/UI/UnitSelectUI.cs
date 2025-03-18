using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject unitHPPrefab;
    [SerializeField] private Image unitHP;

    private Unit selectedUnit;
    [SerializeField] private float yPos = 0.9f;


    // Update is called once per frame
    void Update()
    {
        if (selectedUnit != null)
        {
            UpdateHP();
        }
    }

    public void ShowHP(Unit unit)
    {
        selectedUnit = unit;
        gameObject.SetActive(true);
        transform.position = selectedUnit.transform.position + new Vector3(0, yPos, 0);
    }

    public void HideHP()
    {
        gameObject.SetActive(false);
        selectedUnit = null;
    }

    private void UpdateHP()
    {
        if(unitHPPrefab != null)
        {
            unitHP.fillAmount = selectedUnit.HpPercent;
            transform.position = selectedUnit.transform.position + new Vector3(0, yPos, 0);
        }
    }
}
