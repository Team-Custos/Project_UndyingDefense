using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyMode
{
    Seige,
    Free
}

//TODO : 유닛 상태 변수 추가

public class UD_Ingame_UnitCtrl : MonoBehaviour
{
    [HideInInspector] public UD_Ingame_UnitState Ally_State;
    [HideInInspector] public UD_Ingame_EnemyState Enemy_State;
    MeshRenderer MeshRenderer;

    [Header("====Data====")]
    public int modelType;
    public float health;
    public float maxHealth;
    public float speed;
    public UnitType unitType;

    [Header("====Status====")]
    public AllyMode Ally_Mode;

    public Vector2 unitPos = Vector2.zero;
    public Color32 colorAlly = Color.blue;
    public Color32 colorEnemy = Color.red;

    public GameObject Selected_Particle;
    public bool isSelected = false;

    public float weaponCooldown = 0;
    public float skillCooldown = 0;

    [Header("====AI====")]
    public GameObject targetBase;

    public Vector3 moveTargetPos = Vector3.zero;
    public bool haveToMovePosition = false;
    public GameObject targetEnemy = null;
    public UD_Ingame_RangeCtrl sightRangeSensor;

    public float sightDistance = 0;
    public float attackDistance = 0;

    public bool isEnemyInSight = false;
    public bool isEnemyInRange = false;
    public bool enemy_isBaseInRange = false;

    public GameObject findEnemyRange = null;
    public GameObject Bow = null;

    public float testSpeed = 1;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        Ally_State = GetComponent<UD_Ingame_UnitState>();
        Enemy_State = GetComponent<UD_Ingame_EnemyState>();

        targetBase = UD_Ingame_GameManager.inst.Base;

        moveTargetPos = transform.position;
    }

    

    // Update is called once per frame
    void Update()
    {
        Selected_Particle.SetActive(isSelected);
        //findEnemyRange.SetActive(isSelected);

        sightRangeSensor.radius = sightDistance;
        findEnemyRange.transform.localScale = new Vector3(attackDistance + 4, attackDistance + 4, 0);

        #region 아군 병사 제어
        if (this.gameObject.tag == UD_CONSTANT.TAG_UNIT)
        {
            MeshRenderer.material.color = colorAlly;
            if (targetEnemy != null && !haveToMovePosition)
            {
                if (isEnemyInSight)
                {
                    haveToMovePosition = false;
                    isEnemyInRange = (Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackDistance);
                    if (isEnemyInRange)
                    {
                        moveTargetPos = this.transform.position;

                        Ally_State.fsm.ChangeState(UnitState.Attack);
                    }
                    else
                    {
                        Ally_State.fsm.ChangeState(UnitState.Chase);
                    }
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, moveTargetPos) > 0.1f)
                {
                    Ally_State.fsm.ChangeState(UnitState.Move);
                }
                else
                {
                    haveToMovePosition = false;
                    Ally_State.fsm.ChangeState(UnitState.Idle);
                }
            }
            
        }
        #endregion


        #region 적 제어
        else if (this.gameObject.tag == UD_CONSTANT.TAG_ENEMY)
        {
            if (Input.GetKeyDown(KeyCode.H) && isSelected)
            {
                Destroy(this.gameObject);
            }

            MeshRenderer.material.color = colorEnemy;
            enemy_isBaseInRange =
            (Vector3.Distance(transform.position, targetBase.transform.position) <= attackDistance);

            if (targetEnemy != null && !enemy_isBaseInRange)//병사 발견시
            {
                isEnemyInRange =
                    (Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackDistance);

                if (isEnemyInSight)
                {
                    if (isEnemyInRange)
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
                if (enemy_isBaseInRange)
                {
                    Enemy_State.fsm.ChangeState(EnemyState.Attack);
                }
                else
                {
                    Enemy_State.fsm.ChangeState(EnemyState.Move);
                }

            }

            if (!isEnemyInRange && !enemy_isBaseInRange)
            {
                Enemy_State.fsm.ChangeState(EnemyState.Move);
            }
        }
        #endregion
    }



    public void SearchEnemy()
    {
        if (sightRangeSensor == null)
        {
            Debug.LogError("Range Error in : " + this.gameObject.name);
            return;
        }
        else
        {
            GameObject TargetObj = 
                sightRangeSensor.NearestObjectSearch(attackDistance, this.gameObject.CompareTag(UD_CONSTANT.TAG_ENEMY));

            if (TargetObj != null)
            {
                isEnemyInSight = true;
                moveTargetPos = TargetObj.transform.position;
                targetEnemy = TargetObj;
            }
            else
            {
                isEnemyInSight = false;
                targetEnemy = null;
            }
        }
    }

    public void Unit_Attack()
    {
        if (this.gameObject.tag == UD_CONSTANT.TAG_UNIT)
        {
            if (targetEnemy == null)
            {
                Ally_State.fsm.ChangeState(UnitState.Search);
                return;
            }
            else
            {
                transform.LookAt(targetEnemy.transform.position);
                Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown);
            }
        }
        else if (this.gameObject.tag == UD_CONSTANT.TAG_ENEMY)
        {
            if (enemy_isBaseInRange)
            {
                transform.LookAt(targetBase.transform.position);
                Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown);
            }
            else
            {
                if (targetEnemy != null)
                {
                    transform.LookAt(targetEnemy.transform.position);
                    Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown);
                }
                else if (targetEnemy == null)
                {
                    Enemy_State.fsm.ChangeState(EnemyState.Search);
                }
            }
        }

        

        
    }

    public void Init(UnitSpawnData data)
    {
        modelType = data.modelType;
        //HP = data.HP;
        speed = data.speed;
        unitType = data.unitType;

    }
}
