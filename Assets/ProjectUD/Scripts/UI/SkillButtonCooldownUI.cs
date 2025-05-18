using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonCooldownUI : MonoBehaviour
{
    [SerializeField] private CommandSkill commandSkill;
    public CommandSkillData commandSkillData;
    [SerializeField] private Image cooldownImage;

    private float coolTime;
    private float cooldownCheck;

    private void Start()
    {
        coolTime = commandSkillData.CoolTime;
        cooldownCheck = coolTime;
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
}
