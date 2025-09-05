using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonUI : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI buttonText;
    private UnitData unitData;

    public void SetButton(Sprite image, string text)
    {
        buttonImage.sprite = image;
        buttonText.text = text;
    }

    public void SetUnitData(UnitData unitData)
    {
        this.unitData = unitData;
    }

    public void ResetButton()
    {
        buttonText.text = "";
        buttonImage.sprite = null;
        unitData = null;
    }

    public UnitData GetUnitData()
    {
        return unitData;
    }
}
