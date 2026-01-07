using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SelectedCommanderSkillUI : MonoBehaviour
{
    [SerializeField] private SelectedCSkillBtnUI[] selectedCSkillBtns;
    [SerializeField] private CommanderSkillUI commanderSkillUI;

    private CommandSkillData[] selectedCSkills = new CommandSkillData[3];
    private List<string> selectCSkillID = new List<string>();

    private int index;
    private bool canAdd = false;


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

    public bool AddSkill(CommandSkillData data)
    {
        for (int i = 0; i < selectedCSkills.Length; i++)
        {
            if(selectedCSkills[i] == null)
            {
                //canAdd = true;
                selectedCSkills[i] = data;
                SetCSkill(i);
                return true;
            }
        }
        return false;
    }

    // 선택스킬 버튼용
    public void RemoveSkill(int index)
    {
        commanderSkillUI.DeSelectCommanderSkill(selectedCSkills[index]);
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

    // 저장 버튼용
    public void SaveChoiceCommanderSkill()
    {
        selectCSkillID.Clear();
        //Debug.Log(selectedCSkills);

        for (int i = 0; i < selectedCSkills.Length; i++)
        {
            if (selectedCSkills[i] == null)
                selectCSkillID.Add(string.Empty);
            selectCSkillID.Add(selectedCSkills[i].Id);
        }

        PlayerPrefsData.instance.SetSelectCSkill(selectCSkillID);

    }
}
