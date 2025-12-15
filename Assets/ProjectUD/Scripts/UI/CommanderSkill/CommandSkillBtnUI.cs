using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandSkillBtnUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private RectTransform hoverPosition;

    [SerializeField] private DescriptionPanel descriptionPanel;

    private string sName;
    private string sDescription;
    private string sEffect;


    public void SetBtn(Sprite sprite, string name, string desc, string effect)
    {
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
        eventData.clickCount++;
        if(eventData.clickCount == 2)
        {
            eventData.clickCount = 0;
        }
    }
}
