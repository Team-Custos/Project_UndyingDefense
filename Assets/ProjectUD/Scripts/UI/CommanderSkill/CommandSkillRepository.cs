using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSkillRepository : MonoBehaviour
{
    private CommandSkillData[] commandSkillDatas;

    // Start is called before the first frame update
    void Start()
    {
        SetCommanderSkill();
    }

    public void SetCommanderSkill()
    {
        if (commandSkillDatas != null)
            return;
        commandSkillDatas = Resources.LoadAll<CommandSkillData>("Data/Skill/Command");
        Debug.Log($"[commandSkillDatas]가 null이어서 로드함. Loaded {commandSkillDatas.Length} command skills.");
    }

    public CommandSkillData[] GetCommandSkills()
    {
        return commandSkillDatas;
    }
}
