using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData_LoPol", menuName = "Scriptable Object/UnitData_LoPol", order = 2)]

public class UnitData_LoPol : ScriptableObject
{
    [Header("====Unit Data====")]
    public string unitName;
    public int tier;
    public int maxHp;
    public UnitType unitType;
    public DefenseType defenseType;
    public TargetSelectType targetSelectType;
    public int damage;
    public float critChance;
    public float moveSpeed;
    public float attackCooldown; // attackSpeed;
    public int mental;
    public float sightRange;
    public float attackRange;


}
