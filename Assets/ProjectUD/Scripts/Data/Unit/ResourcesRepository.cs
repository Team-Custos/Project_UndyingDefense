using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesRepository : MonoBehaviour
{
    private Dictionary<string, UnitData[]> factionDic = new Dictionary<string, UnitData[]>();

    private CommandSkillData[] commandSkillDatas = new CommandSkillData[0];

    private void Start()
    {
        SetFactionDic();
        SetCommanderSkill();
    }

    public void SetFactionDic()
    {
        factionDic.Add("ally", Resources.LoadAll<UnitData>("UnitData/Ally/AllyArchive"));
        factionDic.Add("moor", Resources.LoadAll<UnitData>("UnitData/Enemy/moor"));
        factionDic.Add("empire", Resources.LoadAll<UnitData>("UnitData/Enemy/empire"));
        factionDic.Add("pioneer", Resources.LoadAll<UnitData>("UnitData/Enemy/pioneer"));
        //factionDic.Add("summon", Resources.LoadAll<UnitData>("UnitData/Enemy/summon"));

    }

    private void SetCommanderSkill()
    {
        commandSkillDatas = Resources.LoadAll<CommandSkillData>("Data/Skill/Command");
    }

    public CommandSkillData[] GetCommandSkills()
    {
        return commandSkillDatas;
    }

    public UnitData[] GetFactionArray(string fName)
    {
        return factionDic[fName];
    }

    public UnitData GetUnitData(string fName, int i)
    {
        return factionDic[fName][i];
    }
}
