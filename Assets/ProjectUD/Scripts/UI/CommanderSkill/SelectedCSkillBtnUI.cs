using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedCSkillBtnUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SelectedCommanderSkillUI selectedCommanderSkillUI;

    [SerializeField] private Image skillIconImage;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private DescriptionPanel descriptionPanel;
    [SerializeField] private RectTransform hoverPosition;
    [SerializeField] private GameObject removeBtn;

    private CommandSkillData skillData;
    private int index;
    private string sName;
    private string sDescription;
    private string sEffect;

    // -- 더블 클릭용 메서드
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

            selectedCommanderSkillUI.RemoveSkill(index);
        }
        else
        {
            isDoubleClicked = false;
            doubleClickedTime = Time.time;
        }
    }*/

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
            skillIconImage.color = new Color(1, 1, 1, 1);
            skillIconImage.sprite = skillData.Icon;
            skillNameText.text = name;
            sName = name;
            sDescription = desc;
            sEffect = effect;
            removeBtn.SetActive(true);
        }
        else
        {
            //index = i;
            skillData = null;
            skillIconImage.sprite = null;
            removeBtn.SetActive(false);
            skillIconImage.color = new Color(1, 1, 1, 0);
            //skillIconImage.color = new Color(0.658f, 0.572f, 0.494f, 1f);
            skillNameText.text = string.Empty;
            sName = string.Empty;
            sDescription = string.Empty;
            sEffect = string.Empty;
        }
    }

    public void ClearSelectedCSkillUI()
    {
        skillIconImage.sprite = null;
        skillNameText.text = "";
        sName = "";
        sDescription = "";
        sEffect = "";
        skillData = null;
    }
    public void SetSelectedCSkillUI()
    {

    }
}
