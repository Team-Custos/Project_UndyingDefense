using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType //유닛의 종류.
{
    None = -1,
    민병,
    사냥꾼,
    창병,
    한량,
    척후병,
    포수,
}

public enum DefenseType //유닛의 방어속성.
{
    cloth,
    metal,
    leather
}

public enum AllyMode//아군의 모드.
{
    Siege,
    Free,
    Change
}

public enum TargetSelectType//유닛의 타겟 선정 방식.
{
    Nearest,
    LowestHP,
    Fixed
}

[CreateAssetMenu(fileName = "UnitDataReBuild", menuName = "Scriptable Object/UnitDataReBuild", order = int.MaxValue - 1)]
public class UnitData_ReBuild : ScriptableObject
{
    [Header("====General====")]
    public string unitName;
    //public string description;
    public int tier = 1;
    public float maxHP;
    public DefenseType defenseType;
    public TargetSelectType targetSelectType;
    
    //public int baseDamage;
    public float baseCritChanceRate;
    public int mental;
    public float baseMoveSpeed;
    public float baseAttackCooldown;

    public float sightRange;
    public float attackRange;

    public AudioClip[] attackSound;
}
