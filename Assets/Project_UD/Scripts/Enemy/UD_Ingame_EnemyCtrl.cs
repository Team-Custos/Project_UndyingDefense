using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class UD_Ingame_EnemyCtrl : MonoBehaviour
{
    [HideInInspector] public UD_Ingame_EnemyState Enemy_State;
    MeshRenderer MeshRenderer;

    [Header("====Data====")]
    public int modelType;
    public float health;
    public float maxHealth;
    public float speed;
    public UnitType unitType;

    [Header("====Status====")]

    public Vector2 UnitPos = Vector2.zero;
    public Color32 colorEnemy = Color.red;
    public GameObject Selected_Particle;
    public bool isSelected = false;
    public float weaponCooldown = 0;


    [Header("====AI====")]
    public GameObject targetBase;

    public Vector3 moveTargetPos = Vector3.zero;
    public GameObject targetUnit = null;
    public float unitSightDistance = 0;
    public float unitAttackDistance = 0;
    public bool isUnitInSight = false;
    public bool isUnitInRange = false;

    public bool isBaseInRange = false;

    public GameObject findUnitRange = null;
    public GameObject Bow = null;


    public float testSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        Enemy_State = GetComponent<UD_Ingame_EnemyState>();
        moveTargetPos = transform.position;

        targetBase = UD_Ingame_GameManager.inst.Base;
        moveTargetPos = targetBase.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MeshRenderer.material.color = colorEnemy;
        Selected_Particle.SetActive(isSelected);
        findUnitRange.SetActive(isSelected);

        findUnitRange.transform.localScale = new Vector3(unitAttackDistance + 4, unitAttackDistance + 4, 0);

        if (targetUnit != null && !isBaseInRange)//병사 발견시
        {
            if (isUnitInSight)
            {
                if (isUnitInRange)
                {
                    Enemy_State.fsm.ChangeState(EnemyState.Attack);
                }
                else
                {
                    Enemy_State.fsm.ChangeState(EnemyState.Move);
                }
            }
        }
        else 
        {
            moveTargetPos = targetBase.transform.position; // 성 공격
            if (isBaseInRange)
            {
                Enemy_State.fsm.ChangeState(EnemyState.Attack);
            }
            else
            {
                Enemy_State.fsm.ChangeState(EnemyState.Move);
            }

        }

        if (!isUnitInRange && !isBaseInRange)
        {
            Enemy_State.fsm.ChangeState(EnemyState.Move);
        }

    }

    public void Enemy_Attack()
    {
        if (isBaseInRange)
        {
            transform.LookAt(targetBase.transform.position);
        }
        else if(targetUnit != null)
        {
            transform.LookAt(targetUnit.transform.position);
        }

        Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown);
    }

    //스폰 하기전 스폰 데이터 값을 불러옴.
    public void Init(EnemySpawnData data)
    {
        speed = data.speed;
        maxHealth = data.HP;
        health = data.HP;
    }
}
