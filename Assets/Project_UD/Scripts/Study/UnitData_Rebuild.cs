using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData_ReBuild : ScriptableObject
{
    [Header("====General====")]
    public string unitName;
    //public string description;
    public int tier = 1;
    public int maxHP;
    public DefenseType defenseType;
    public TargetSelectType targetSelectType;
    public int baseDamage;
    public int baseCritChanceRate;
    public int mental;
    public float baseMoveSpeed;
    public float baseAttackCooldown;

    public float sightRange;
    public float attackRange;
}
