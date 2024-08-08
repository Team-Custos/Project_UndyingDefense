using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum AllyMode
{
    Seige,
    Free
}

public enum TargetSelectType
{
    Nearest,
    LowestHP,
    Fixed
}



public class UD_Ingame_UnitCtrl : MonoBehaviour
{
    [HideInInspector] public UD_Ingame_UnitState Ally_State;
    [HideInInspector] public UD_Ingame_EnemyState Enemy_State;
    [HideInInspector] public NavMeshObstacle NavObstacle;
    [HideInInspector] public NavMeshAgent NavAgent;

    MeshRenderer MeshRenderer;


    [Header("====Data====")]
    public int modelType;
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


    [Header("====Status====")]
    public AllyMode Ally_Mode;

    public Vector2 unitPos = Vector2.zero;
    public Color32 colorAlly = Color.blue;
    public Color32 colorEnemy = Color.red;

    public GameObject Selected_Particle;
    public bool isSelected = false;

    public float weaponCooldown = 0;
    public float skillCooldown = 0;

    public int[][] debuffs;

    

    [Header("====AI====")]
    public GameObject targetBase;

    public Vector3 moveTargetBasePos;
    public Vector3 moveTargetPos = Vector3.zero;
    public bool haveToMovePosition = false;
    public GameObject targetEnemy = null;
    public UD_Ingame_RangeCtrl sightRangeSensor;

    public bool isEnemyInSight = false;
    public bool isEnemyInRange = false;
    public bool enemy_isBaseInRange = false;
    public bool enemy_isPathBlocked = false;

    public GameObject findEnemyRange = null;
    public GameObject Bow = null;

    private void OnEnable()
    {

        Ally_Mode = AllyMode.Seige;

        if (this.gameObject.tag == UD_CONSTANT.TAG_UNIT)
        {
            if (Ally_Mode == AllyMode.Free)
            {
                NavObstacle.enabled = false;
                NavAgent.enabled = true;
            }
            else if (Ally_Mode == AllyMode.Seige)
            {
                NavAgent.enabled = false;
                NavObstacle.enabled = true;
            }
        }

        HP = maxHP;
    }

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        NavAgent = GetComponent<NavMeshAgent>();
        NavObstacle = GetComponent<NavMeshObstacle>();

        Ally_State = GetComponent<UD_Ingame_UnitState>();
        Enemy_State = GetComponent<UD_Ingame_EnemyState>();

        targetBase = UD_Ingame_GameManager.inst.Base;

        moveTargetPos = transform.position;
        moveTargetBasePos = new Vector3(this.transform.position.x, targetBase.transform.position.y, targetBase.transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(findEnemyRange.transform.position, attackRange + 0.5f);
    }


    // Update is called once per frame
    void Update()
    {
        Selected_Particle.SetActive(isSelected);
        //findEnemyRange.SetActive(isSelected);

        sightRangeSensor.radius = sightRange;
        findEnemyRange.transform.localScale = new Vector3(attackRange + 4, attackRange + 4, 0);

        if (HP <= 0)
        {
            Debug.Log(this.gameObject.name + " Destroyed");
            Destroy(this.gameObject);
        }

        #region 아군 병사 제어
        if (this.gameObject.tag == UD_CONSTANT.TAG_UNIT)
        {
            MeshRenderer.material.color = colorAlly;
            if (Ally_Mode == AllyMode.Free)
            {
                NavObstacle.enabled = false;
                NavAgent.enabled = true;
            }
            else if (Ally_Mode == AllyMode.Seige)
            {
                NavObstacle.enabled = true;
                NavAgent.enabled = false;
            }


            if (targetEnemy != null && !haveToMovePosition)
            {
                if (isEnemyInSight)
                {
                    haveToMovePosition = false;
                    isEnemyInRange = (Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRange);
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
            (Vector3.Distance(transform.position, new Vector3(this.transform.position.x, targetBase.transform.position.y, targetBase.transform.position.z)) <= attackRange);

            if (enemy_isPathBlocked)
            {
                if (targetEnemy != null && !enemy_isBaseInRange)//병사 발견시
                {
                    isEnemyInRange =
                        (Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRange);

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
            }
            else // 성 공격
            {
                moveTargetPos = new Vector3(this.transform.position.x, targetBase.transform.position.y, targetBase.transform.position.z); 
                
                //NavAgent.SetDestination(new Vector3(this.transform.position.x, targetBase.transform.position.y, targetBase.transform.position.z)); 
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
            Debug.LogError("Range NullError in : " + this.gameObject.name);
            return;
        }
        else
        {
            GameObject TargetObj = 
                sightRangeSensor.NearestObjectSearch(attackRange, this.gameObject.CompareTag(UD_CONSTANT.TAG_ENEMY));

            if (TargetObj != null)
            {
                isEnemyInSight = true;
                targetEnemy = TargetObj;

                if (Ally_Mode == AllyMode.Free)
                {
                    moveTargetPos = TargetObj.transform.position;
                }
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
                Bow.transform.LookAt(targetEnemy.transform.position);
                Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown, attackPoint, false);
            }
        }
        else if (this.gameObject.tag == UD_CONSTANT.TAG_ENEMY)
        {
            if (enemy_isBaseInRange)
            {
                Bow.transform.LookAt(moveTargetPos);
                Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown, attackPoint, true);
            }
            else
            {
                if (targetEnemy != null)
                {
                    Bow.transform.LookAt(targetEnemy.transform.position);
                    Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown, attackPoint, true);
                }
                else if (targetEnemy == null)
                {
                    Debug.Log("경로 다시 검색...");
                    Enemy_State.fsm.ChangeState(EnemyState.Move);
                }
            }
        }
    }

    public void Init(UnitSpawnData data)
    {
        modelType = data.modelType;
        maxHP = data.HP;
        moveSpeed = data.speed;
        attackPoint = data.atk;
        unitType = data.unitType;

    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject OBJ = other.gameObject;
        //피격 판정
        if (OBJ.CompareTag(UD_CONSTANT.TAG_ATTACK))
        {
            UD_Ingame_ArrowCtrl Arrow = OBJ.GetComponent<UD_Ingame_ArrowCtrl>();

            if (this.gameObject.CompareTag(UD_CONSTANT.TAG_UNIT) && Arrow.isEnemyAttack)
            {
                Debug.Log(this.gameObject.name + " attack hit!");
                this.HP -= Arrow.Atk;
                Destroy(OBJ);
            }
            else if (this.gameObject.CompareTag(UD_CONSTANT.TAG_ENEMY) && !Arrow.isEnemyAttack)
            {
                Debug.Log(this.gameObject.name + " attack hit!");
                this.HP -= Arrow.Atk;
                Destroy(OBJ);
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(this.gameObject.name + " attack collision hit!");

        if (!collision.gameObject.CompareTag(UD_CONSTANT.TAG_GROUND) && !collision.gameObject.CompareTag(UD_CONSTANT.TAG_TILE))
        {
            Debug.Log(this.gameObject.name + " collision hit at : " + collision.gameObject.name);
            NavAgent.ResetPath();

            if (this.gameObject.CompareTag(UD_CONSTANT.TAG_UNIT))
            {
                Ally_State.fsm.ChangeState(UnitState.Idle);
            }
            else if (this.gameObject.CompareTag(UD_CONSTANT.TAG_ENEMY))
            {
                Enemy_State.fsm.ChangeState(EnemyState.Idle);
            }

        }
        
    }
}
