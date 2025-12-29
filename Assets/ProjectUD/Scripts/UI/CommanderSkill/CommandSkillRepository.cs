using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSkillRepository : MonoBehaviour
{
    private CommandSkillData[] commandSkillDatas = new CommandSkillData[0];

    // Start is called before the first frame update
    void Start()
    {
        SetCommanderSkill();
    }

    private void SetCommanderSkill()
    {
        commandSkillDatas = Resources.LoadAll<CommandSkillData>("Data/Skill/Command");
    }

    public CommandSkillData[] GetCommandSkills()
    {
        return commandSkillDatas;
    }
}
