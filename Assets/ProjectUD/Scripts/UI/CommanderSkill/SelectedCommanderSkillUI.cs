using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCommanderSkillUI : MonoBehaviour
{
    private CommandSkillData[] selectedCSkills = new CommandSkillData[3];
    [SerializeField] private SelectedCSkillBtnUI[] selectedCSkillBtns;
    private int index;
    private bool canAdd = false;

    // [SerializeField]

    public void SetCommandSkill(CommandSkillData[] datas)
    {
        selectedCSkills = datas;
        for (int i = 0; i < selectedCSkillBtns.Length; i++)
        {
            selectedCSkillBtns[i].SetSelectedCSkillUI(selectedCSkills[i]);
        }
    }

    public void AddSkill(CommandSkillData data)
    {
        for (int i = 0; i < selectedCSkills.Length; i++)
        {
            if(selectedCSkills[i] == null)
            {
                //canAdd = true;
                selectedCSkills[i] = data;
                return;
            }
        }
    }

    public void RemoveSkill(int index)
    {
        selectedCSkills[index] = null; 
    }
}
