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

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
    // 추가
    public float globalTime;
=======
    // �߰�
>>>>>>> parent of 98fb097 (Merge branch 'Release' of https://github.com/Team-Custos/Project_UndyingDefense into Release)
=======
    // �߰�
>>>>>>> parent of 98fb097 (Merge branch 'Release' of https://github.com/Team-Custos/Project_UndyingDefense into Release)
=======
    // 추가
    public float globalTime;
>>>>>>> parent of 48d20c1 (Merge branch 'LoPol' into Release)
    public string g_SkillName;
    public string s_SkillName;
    public string unitCode;
    public int level;
    public int cost;
    public string name;

}
