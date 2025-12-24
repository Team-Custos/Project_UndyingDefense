using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandSkillBtnUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private CommanderSkillUI commanderSkillUI;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private RectTransform hoverPosition;

    [SerializeField] private DescriptionPanel descriptionPanel;

    private int index;
    private string sName;
    private string sDescription;
    private string sEffect;

    float interval = 0.25f;
    float doubleClickedTime = -1.0f;
    bool isDoubleClicked = false;

    public void SetBtn(int i, Sprite sprite, string name, string desc, string effect)
    {
        index = i;
        icon.sprite = sprite;
        skillName.text = name;  
        sName = name;
        sDescription = desc;
        sEffect = effect;
    }

    public void ResetButton()
    {
        skillName.text = "";
        icon.sprite = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionPanel.SetPanel(sName, sDescription, sEffect);
        descriptionPanel.transform.position = hoverPosition.position;
        descriptionPanel.ShowPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.HidePanel();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((Time.time - doubleClickedTime) < interval)
        {
            isDoubleClicked = true;
            doubleClickedTime = -1.0f;

            Debug.Log("double click!");

            commanderSkillUI.SelectCommandSkill(index);
        }
        else
        {
            isDoubleClicked = false;
            doubleClickedTime = Time.time;
        }
    }
}
