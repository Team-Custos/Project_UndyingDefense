using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonCooldownUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CommandSkill commandSkill;
    private CommandSkillData commandSkillData;
    [SerializeField] private IngameCommandSkillManager commandSkillManager;
    [SerializeField] private Image cooldownImage;

    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI commandSkillNameText;
    [SerializeField] private TextMeshProUGUI commandSkillCoolText;
    [SerializeField] private TextMeshProUGUI commandSkillDescriptionText;
    [SerializeField] private Image skillIcon;

    //ayo_0117
    [SerializeField] private Button skillButton;

    private float coolTime;
    private float cooldownCheck;
    private int index;

    private void Start()
    {
        coolTime = commandSkillData.CoolTime;
        cooldownCheck = coolTime;

        //commandSkillNameText.text = commandSkillData.Name;
        //commandSkillCoolText.text = "쿨타임 " + commandSkillData.CoolTime.ToString() + "초";
        //commandSkillDescriptionText.text = commandSkillData.Description;
    }

    private void Update()
    {
        if(commandSkill.IsCoolDown) // 쿨타임 종료 -> 스킬 사용 가능
        {
            cooldownCheck = 0f;
            cooldownImage.fillAmount = 1f;
            cooldownImage.gameObject.SetActive(false);
            this.tag = "InteractiveUi";
        }
        else // 쿨타임 중 -> 스킬 사용 불가
        {
            commandSkillManager.ResetButton();
            cooldownImage.gameObject.SetActive(true);
            cooldownCheck += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (cooldownCheck / coolTime);
            this.tag = "UnInteractiveUi";
        }
    }
    
    public void SetSelectedCSkillDataUI(int i, CommandSkillData data, string name, string desc, string effect)
    {
        index = i;
        commandSkillData = data;
        skillIcon.sprite = commandSkillData.Icon;
        commandSkillNameText.text = name;
        commandSkillDescriptionText.text = desc;

        coolTime = commandSkillData.CoolTime;
        cooldownCheck = coolTime;

        commandSkillCoolText.text = "쿨타임 " + commandSkillData.CoolTime.ToString() + "초";
    }

    public void SetCommandSkill(CommandSkill skill)
    {
        commandSkill = skill;
    }

    public void OnButtonClick()
    {
        //commandSkillManager.GetClickControl(index, commandSkill);
        // ayo_0117
        commandSkillManager.SetBeingUsedCommandSkill(index, commandSkill);
        if (!commandSkill.IsCoolDown)                               
        {
            commandSkillManager.ResetButton();
            Debug.Log(commandSkill.name + "이 쿨타임 중...");         
            SoundManager.Instance.PlayUnableUIClickSFX();
            return;
        }
        commandSkill.Activate();
        SoundManager.Instance.PlayUIClickSFX();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.SetActive(false);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoPanel.SetActive(true);
    }
}
