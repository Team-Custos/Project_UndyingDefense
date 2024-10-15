using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    order,
    support,
    morale
};

[Serializable]
public class CommanderSkillData
{
    public SkillType type;
    public int SkillCode;
    public string Name;
    public string Description;
    public float CoolDown;
    public bool isTargetEnemy;
}

public class Ingame_CommanderSkillManager : MonoBehaviour
{
    public CommanderSkillData[] Order_SkillData;
    public CommanderSkillData[] Surpport_SkillData;
    public CommanderSkillData[] Morale_SkillData;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Skill(SkillType type)
    {
        
    }
    
}
