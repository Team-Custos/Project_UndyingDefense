using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedCSkillBtnUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private SelectedCommanderSkillUI selectedCommanderSkillUI;

    [SerializeField] private Image skillIconImage;
    [SerializeField] private DescriptionPanel descriptionPanel;
    [SerializeField] private RectTransform hoverPosition;

    private CommandSkillData skillData;
    private int index;
    private string sName;
    private string sDescription;
    private string sEffect;

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

            selectedCommanderSkillUI.RemoveSkill(index);
        }
        else
        {
            isDoubleClicked = false;
            doubleClickedTime = Time.time;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(skillData == null)
            return;
        descriptionPanel.SetPanel(sName, sDescription, sEffect);
        descriptionPanel.transform.position = hoverPosition.position;
        descriptionPanel.ShowPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.HidePanel();
    }

    public void SetSelectedCSkillUI(int i, CommandSkillData data, string name, string desc, string effect)
    {
        if(data != null)
        {
            index = i;
            skillData = data;
            skillIconImage.sprite = skillData.Icon;
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

    public void ClearSelectedCSkillUI()
    {
        skillIconImage.sprite = null;
        sName = "";
        sDescription = "";
        sEffect = "";
        skillData = null;
    }
    public void SetSelectedCSkillUI()
    {

    }
}
