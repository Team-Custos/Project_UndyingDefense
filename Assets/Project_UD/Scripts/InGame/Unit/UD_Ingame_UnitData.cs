using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_UnitData : MonoBehaviour
{
    [Header("====Data====")]
    public int modelType;
    public int cur_modelType;
    public int curLevel = 1;
    public int HP;
    public int maxHP;
    public float moveSpeed;
    public float attackSpeed;
    public int mental = 1;
    public float sightRange = 0;
    public float attackRange = 0;
    public int attackPoint = 1;
    public int critChanceRate;
    public int generalSkillCode = 101;
    public int specialSkillCode = 101;
    public UnitType unitType;
    public TargetSelectType targetSelectType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
