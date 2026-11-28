using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CommandSkillBtnUI : MonoBehaviour
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
                // 해당 등급 달성 후 스킬 해금 가능 메세지
                string warningMessageText = LocalizationSettings.StringDatabase.GetLocalizedString("LobbyUI", "MSG_noRank", LocalizationSettings.SelectedLocale);
                warningMessage.AddMessage(warningMessageText);  // LobbyUI MSG_noRank
            });
        }
    }

    public void SetBtn(int i, bool canUse, Sprite sprite, string name, string desc, string effect, string coolTime)
    {
        index = i;
        SetActiveSkillBtn(canUse);
        if(sprite == null)
        {
            icon.sprite = null;
            icon.color =  new Color(0.65f, 0.6f, 0.5f, 1);
        }
        else
        {
            icon.sprite = sprite;
            icon.color = Color.white;
        }
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
