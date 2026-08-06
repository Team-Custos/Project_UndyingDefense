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
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private ToolTipUI toolTipUI;

    [SerializeField] private Image cooldownImage;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI commandSkillNameText;
    [SerializeField] private TextMeshProUGUI commandSkillDescriptionText;
    [SerializeField] private TextMeshProUGUI commandSkilEffectText;
    [SerializeField] private TextMeshProUGUI commandSkillCoolText;
    [SerializeField] private Image skillIcon;
    [SerializeField] private GameObject alarmIcon;

    //ayo_0117
    [SerializeField] private Button skillButton;

    private float coolTime;
    private float cooldownCheck;
    private int index;

    private void Start()
    {
        if(commandSkillData == null)
            return;
        coolTime = commandSkillData.CoolTime;
        cooldownCheck = coolTime;

        //commandSkillNameText.text = commandSkillData.Name;
        //commandSkillCoolText.text = "쿨타임 " + commandSkillData.CoolTime.ToString() + "초";
        //commandSkillDescriptionText.text = commandSkillData.Description;
    }

    private void Update()
    {
        if(commandSkill != null)
            UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        if (commandSkill.IsCoolDown) // 쿨타임 종료 -> 스킬 사용 가능
        {
            cooldownCheck = 0f;
            cooldownImage.fillAmount = 1f;
            cooldownImage.gameObject.SetActive(false);
            this.tag = "InteractiveUi";
        }
        else // 쿨타임 중 -> 스킬 사용 불가
        {
            //commandSkillManager.ResetButton();
            cooldownImage.gameObject.SetActive(true);
            cooldownCheck += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (cooldownCheck / coolTime);
            this.tag = "UnInteractiveUi";
        }
    }


    public void SetSelectedCSkillDataUI(int i, CommandSkillData data, string name, string desc, string effect)
    {
        if(data == null)
        {
            // 빈칸이 넘어오면 초기화하고 슬롯 이미지 검은색으로 변경
            this.tag = "UnInteractiveUi";

            commandSkillData = null;
            skillIcon.sprite = null;
            skillIcon.color = new Color(1, 1, 1, 0);
            commandSkillNameText.text = string.Empty;
            commandSkillDescriptionText.text = string.Empty;
            commandSkilEffectText.text = string.Empty;
            coolTime = 0f;
            cooldownCheck = 0f;
            commandSkillCoolText.text = string.Empty;
            return;
        }

        this.tag = "InteractiveUi";

        index = i;
        commandSkillData = data;
        skillIcon.sprite = commandSkillData.Icon;
        commandSkillNameText.text = name;
        commandSkillDescriptionText.text = desc;
        commandSkilEffectText.text = effect;

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
        if(alarmIcon != null && alarmIcon.activeSelf)
            alarmIcon.SetActive(false);

        //commandSkillManager.GetClickControl(index, commandSkill);
        // ayo_0117
        if (commandSkill == null)
        {
            Debug.Log("스킬이 설정되지 않았습니다.");
            //SoundManager.Instance.PlayUnableUIClickSFX();
            return;
        }
        if (!commandSkill.IsCoolDown)                               
        {
            //commandSkillManager.ResetButton();
            Debug.Log(commandSkill.name + "이 쿨타임 중...");         
            SoundManager.Instance.PlayUnableUIClickSFX();
            return;
        }

        inGameManager.CancleInputState(InputState.COMMAND_SKILL);
        commandSkillManager.SetBeingUsedCommandSkill(index);    //, commandSkill);



        commandSkill.Activate();
        SoundManager.Instance.PlayUIClickSFX();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (commandSkillData != null)
        {
            infoPanel.SetActive(true);
            toolTipUI.SetPanelHeight();
        }
    }
}
