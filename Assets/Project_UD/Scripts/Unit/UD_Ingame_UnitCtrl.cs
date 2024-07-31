using MonsterLove.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AllyMode
{
    Siege,
    Free,
    Change
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

    public float unitStateChangeTime;
    public AllyMode previousAllyMode;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        Ally_State = GetComponent<UD_Ingame_UnitState>();
        Enemy_State = GetComponent<UD_Ingame_EnemyState>();

        targetBase = UD_Ingame_GameManager.inst.Base;

        moveTargetPos = transform.position;

        unitStateChangeTime = 0.0f;

        Ally_Mode = AllyMode.Siege;

        attackDistance = 10.0f;
    }

    

    // Update is called once per frame
    void Update()
    {
        Selected_Particle.SetActive(isSelected);
        //findEnemyRange.SetActive(isSelected);

        sightRangeSensor.radius = sightDistance;
        findEnemyRange.transform.localScale = new Vector3(attackDistance + 4, attackDistance + 4, 0);

        

        if (this.gameObject.tag == UD_CONSTANT.TAG_UNIT)
        {
            MeshRenderer.material.color = colorAlly;

            // Change 상태일 때는 다른 행동을 하지 않음
            if (Ally_Mode == AllyMode.Change)
            {
                if (unitStateChangeTime > 0)
                {
                    unitStateChangeTime -= Time.deltaTime;
                    if (Ally_State != null && Ally_State.fsm != null)
                    {
                        Ally_State.fsm.ChangeState(UnitState.Idle); // 움직임을 멈추게 함
                    }
                    else
                    {
                        Debug.LogError("Ally_State or FSM is null in " + this.gameObject.name);
                    }
                    return;
                }
                else
                {
                    if (previousAllyMode == AllyMode.Free)
                    {
                        Ally_Mode = AllyMode.Siege;
                        SearchEnemy();
                    }
                    else if (previousAllyMode == AllyMode.Siege)
                    {
                        Ally_Mode = AllyMode.Free;
                    }
                }
            }
            else if (Ally_Mode == AllyMode.Siege)
            {
                if (targetEnemy == null || !isEnemyInSight)
                {
                    SearchEnemy();
                }

                if (targetEnemy != null && !haveToMovePosition)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
                    isEnemyInRange = (distanceToEnemy <= attackDistance);

                    if (isEnemyInRange && Ally_State != null && Ally_State.fsm != null)
                    {
                        Ally_State.fsm.ChangeState(UnitState.Attack);
                    }
                }
            }
            else if (Ally_Mode == AllyMode.Free)
            {
                //navMeshObstacle.carving = false;
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
        }

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
        Ally_Mode = AllyMode.Siege;

    }

}
