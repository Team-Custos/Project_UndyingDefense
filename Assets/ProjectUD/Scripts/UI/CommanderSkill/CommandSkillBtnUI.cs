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

    [Header("스킬배경이미지")]
    [SerializeField] private Sprite SelectedSprite;
    [SerializeField] private Sprite NormalSprite;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private RectTransform hoverPosition;

    [SerializeField] private DescriptionPanel descriptionPanel;

    [SerializeField] private Image lockIcon;
    [SerializeField] private Image lockPanel;
    [SerializeField] private TextMeshProUGUI helpDesc;
    [SerializeField] private Image panelImage;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button unlockButton;

    [SerializeField] private MessageUI warningMessage;

    private int index;
    private string sName;
    private string sDescription;
    private string sEffect;
    private string sCoolTime;
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
        panelImage.sprite = b ? SelectedSprite : NormalSprite;
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

        if(!b)
        {
            unlockButton.onClick.AddListener(() =>
            {
                warningMessage.AddMessage("해당 계급이 되어야 스킬을 해금할 수 있습니다.");
            });
        }
    }

    public void SetBtn(int i, bool canUse, Sprite sprite, string name, string desc, string effect, string coolTime)
    {
        index = i;
        SetActiveSkillBtn(canUse);
        icon.sprite = sprite;
        skillName.text = name;  
        sName = name;
        sDescription = desc;
        sEffect = effect;
        sCoolTime = coolTime;
    }

    public void ResetButton()
    {
        skillName.text = "";
        icon.sprite = null;
        selectedIndicator.SetActive(false);
        panelImage.sprite = NormalSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionPanel.SetPanel(sName, sDescription, sEffect, sCoolTime);
        descriptionPanel.transform.position = hoverPosition.position;
        descriptionPanel.SetPanelHeight();
        descriptionPanel.ShowPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.HidePanel();
    }
}
