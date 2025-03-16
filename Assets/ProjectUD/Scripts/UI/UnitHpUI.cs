using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHpUI : MonoBehaviour
{
    [SerializeField] private GameObject unitHPPrefab;
    [SerializeField] private Image unitHP;

    private Unit selectedUnit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        }
    }
}
