using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandSkillBtnUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private RectTransform hoverPosition;

    public void SetBtn(Sprite sprite, string name)
    {
        icon.sprite = sprite;
        skillName.text = name;   
    }

    public void ResetButton()
    {
        skillName.text = "";
        icon.sprite = null;
    }
}
