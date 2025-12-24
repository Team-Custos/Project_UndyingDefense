using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SelectedCommanderSkillUI : MonoBehaviour
{
    private CommandSkillData[] selectedCSkills = new CommandSkillData[3];
    [SerializeField] private SelectedCSkillBtnUI[] selectedCSkillBtns;
    private int index;
    private bool canAdd = false;

    // [SerializeField]

    public void SetCSkillList(CommandSkillData[] datas)
    {
        selectedCSkills = datas;
        for (int i = 0; i < selectedCSkillBtns.Length; i++)
        {
            SetCSkill(i);
        }
    }

    public void SetCSkill(int index)
    {
        string skillNameId = selectedCSkills[index].Id + "_name";
        string skillDescId = selectedCSkills[index].Id + "_desc";
        string skillEffectId = selectedCSkills[index].Id + "_effect";

        selectedCSkillBtns[index].SetSelectedCSkillUI(index, selectedCSkills[index],
            LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{skillNameId}", LocalizationSettings.SelectedLocale),
            LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{skillDescId}", LocalizationSettings.SelectedLocale),
            LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{skillEffectId}", LocalizationSettings.SelectedLocale));
    }

    public void AddSkill(CommandSkillData data)
    {
        for (int i = 0; i < selectedCSkills.Length; i++)
        {
            if(selectedCSkills[i] == null)
            {
                //canAdd = true;
                selectedCSkills[i] = data;
                SetCSkill(i);
                return;
            }
        }
    }

    // 선택스킬 버튼용
    public void RemoveSkill(int index)
    {
        selectedCSkills[index] = null;
        selectedCSkillBtns[index].ClearSelectedCSkillUI();
    }

    public void RemoveSkill(CommandSkillData data)
    {
        //if(data != null && selectedCSkills.Contains(data))
        
        if(data == null)
            return;
        for (int i = 0; i < selectedCSkills.Length; i++)
        {
            if (selectedCSkills[i] == data)
            {
                RemoveSkill(i);
            }
        }
    }
}
