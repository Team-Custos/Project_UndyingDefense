using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingame_UnitDataStatus : MonoBehaviour
{
    [Header("====General====")]
    public UnitType unitType;
    public DefenseType defenseType;
    public TargetSelectType targetSelectType;

    public int modelType;
    public int cur_modelType;
    public int curLevel = 1;
    public int HP;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
