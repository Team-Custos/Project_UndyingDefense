using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Object/UnitData", order = int.MaxValue)]
public class Ingame_UnitData : ScriptableObject
{
    [Header("====General====")]
    public UnitType unitType;
    public DefenseType defenseType;
    public TargetSelectType targetSelectType;

    public int modelType;
    public int maxHP;
    public int mental = 1;
    public float moveSpeed;

    public AnimatorOverrideController overrideController;

    [Header("====Attack====")]
    public int attackPoint = 1;
    public int critChanceRate;
    public int generalSkillCode = 101;
    public int specialSkillCode = 101;
    public float weaponCooldown = 0;
    public float skillCooldown = 0;
    public float sightRange = 0;
    public float attackRange = 0;
    public AudioClip[] attackSound;

    // 추가
    public string unitCode;
    public string g_SkillName;
    public string s_SkillName;
    public int level;
    public int cost;
    public string name;
    public string g_SkillInfo;
    public string s_SkillInfo;

}
