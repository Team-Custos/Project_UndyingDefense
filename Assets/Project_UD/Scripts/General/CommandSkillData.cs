using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandType
{
    ORDER,
    SURPPORT,
    MORALE
}

[CreateAssetMenu(fileName = "CommandSkillData", menuName = "Scriptable Object/CommandSkillData", order = 1)]
public class CommandSkillData : ScriptableObject
{
    public CommandType commandType;
    public string commandSkillName;
    public string commandSkillDescription;
    public Sprite commandSkillImage;
}
