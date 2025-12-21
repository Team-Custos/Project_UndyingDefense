using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedCSkillBtnUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image skillIconImage;
    [SerializeField] private DescriptionPanel descriptionPanel;
    [SerializeField] private RectTransform hoverPosition;

    private string sName;
    private string sDescription;
    private string sEffect;

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
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

    public void SetSelectedCSkillUI(CommandSkillData data, string name, string desc, string effect)
    {
        if(data != null)
        {
            skillIconImage.sprite = data.Icon;
            //skillIconImage.color = Color.white;
            sName = name;
            sDescription = desc;
            sEffect = effect;
        }
        else
        {
            skillIconImage.sprite = null;
            //skillIconImage.color = new Color(1, 1, 1, 0);
        }
    }
    public void SetSelectedCSkillUI()
    {

    }
}
