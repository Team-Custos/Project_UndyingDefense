using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonCooldownUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CommandSkill commandSkill;
    public CommandSkillData commandSkillData;
    [SerializeField] private Image cooldownImage;

    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI commandSkillNameText;
    [SerializeField] private TextMeshProUGUI commandSkillCoolText;
    [SerializeField] private TextMeshProUGUI commandSkillDescriptionText;

    private float coolTime;
    private float cooldownCheck;

    private void Start()
    {
        coolTime = commandSkillData.CoolTime;
        cooldownCheck = coolTime;

        commandSkillNameText.text = commandSkillData.Name;
        commandSkillCoolText.text = "쿨타임 " + commandSkillData.CoolTime.ToString() + "초";
        commandSkillDescriptionText.text = commandSkillData.Description;
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
            cooldownImage.gameObject.SetActive(true);
            cooldownCheck += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (cooldownCheck / coolTime);
            this.tag = "UnInteractiveUi";
        }
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
