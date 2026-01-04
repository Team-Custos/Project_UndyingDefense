using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandSkillBtnUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CommanderSkillUI commanderSkillUI;
    [SerializeField] private GameObject selectedIndicator;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private RectTransform hoverPosition;

    [SerializeField] private DescriptionPanel descriptionPanel;

    [SerializeField] private Image lockIcon;
    [SerializeField] private Image lockPanel;
    [SerializeField] private TextMeshProUGUI helpDesc;
    [SerializeField] private Image panelImage;
    [SerializeField] private Button skillButton;

    private int index;
    private string sName;
    private string sDescription;
    private string sEffect;

    private bool isSelected = false;
    private bool isActive = false;

    // -- 더블 클릭 메서드--
    /*
    float interval = 0.25f;
    float doubleClickedTime = -1.0f;
    bool isDoubleClicked = false;

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
    }*/

    public void ToggleSelected(bool b)
    {
        selectedIndicator.SetActive(b);
        isSelected = b;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    private void SetActiveSkillBtn(bool b)
    {
        panelImage.color = b ? Color.white : new Color(1, 1, 1, 0);
        skillButton.interactable = b;
        lockIcon.gameObject.SetActive(!b);
        lockPanel.gameObject.SetActive(!b);
        helpDesc.gameObject.SetActive(!b);
    }

    public void SetBtn(int i, bool canUse, Sprite sprite, string name, string desc, string effect)
    {
        index = i;
        SetActiveSkillBtn(canUse);
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
}
