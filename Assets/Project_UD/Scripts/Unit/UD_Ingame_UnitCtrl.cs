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
    [HideInInspector] public UD_Ingame_UnitSkillManager UnitSkill;

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
    bool SpawnDelay = true;

    public GameObject targetBase;

    public Vector3 moveTargetBasePos;
    public Vector3 moveTargetPos;
    public bool haveToMovePosition = false;
    public GameObject targetEnemy = null;
    public UD_Ingame_RangeCtrl sightRangeSensor;

    public bool isEnemyInSight = false;
    public bool isEnemyInRange = false;
    public bool enemy_isBaseInRange = false;
    public bool enemy_isPathBlocked = false;

    public GameObject findEnemyRange = null;
    public GameObject Bow = null;

    public float testSpeed = 1;

    public float unitStateChangeTime;
    public AllyMode previousAllyMode;

    public string unitName;

    void OnMouseDown()
    {
        if (UD_Ingame_UIManager.instance != null)
        {
            UD_Ingame_UIManager.instance.UpdateUnitInfoPanel(this.unitName);
        }
    }

    private void Awake()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        NavAgent = GetComponent<NavMeshAgent>();
        NavObstacle = GetComponent<NavMeshObstacle>();

        Ally_State = GetComponent<UD_Ingame_UnitState>();
        Enemy_State = GetComponent<UD_Ingame_EnemyState>();

        UnitSkill = GetComponentInChildren<UD_Ingame_UnitSkillManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnDelay = true;

        targetBase = UD_Ingame_GameManager.inst.Base;
        HP = maxHP;

        Ally_Mode = AllyMode.Siege;

        if (this.gameObject.tag == UD_CONSTANT.TAG_UNIT)
        {
            if (Ally_Mode == AllyMode.Free)
            {
                NavObstacle.enabled = false;
                NavAgent.enabled = true;
            }
            else if (Ally_Mode == AllyMode.Siege)
            {
                NavAgent.enabled = false;
                NavObstacle.enabled = true;
            }
        }

        moveTargetPos = this.transform.position;

        unitStateChangeTime = 0.0f;
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
        moveTargetBasePos = new Vector3(targetBase.transform.position.x, this.transform.position.y, this.transform.position.z);
        sightRangeSensor.radius = sightRange;

        NavAgent.speed = moveSpeed;

        if (HP <= 0)
        {
            Debug.Log(this.gameObject.name + " Destroyed");
            Destroy(this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.H) && isSelected)
        {
            Destroy(this.gameObject);
        }


        #region 아군 제어
        if (this.gameObject.tag == UD_CONSTANT.TAG_UNIT)
        {
            
            if (Ally_Mode == AllyMode.Free)
            {
                NavObstacle.enabled = false;
                NavAgent.enabled = true;
                MeshRenderer.material.color = Color.cyan;
            }
            else if (Ally_Mode == AllyMode.Siege)
            {
                NavObstacle.enabled = true;
                NavAgent.enabled = false;
                MeshRenderer.material.color = colorAlly;
            }

            if (isSelected && Input.GetKeyDown(KeyCode.Q))
            {
                previousAllyMode = Ally_Mode;
                //isSelected = false;
                Ally_Mode = AllyMode.Change;
            }

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
                        MoveUnitToNearestTile();
                        SearchEnemy();
                    }
                    else if (previousAllyMode == AllyMode.Siege)
                    {
                        //NavAgent.updatePosition = false;
                        transform.position = moveTargetPos;

                        Ally_Mode = AllyMode.Free;
                    }
                    unitStateChangeTime = 3;
                }
            }
            //시즈모드일때
            else if (Ally_Mode == AllyMode.Siege)
            {
                if (targetEnemy == null && !isEnemyInSight)
                {
                    SearchEnemy();
                }

                if (targetEnemy != null && !haveToMovePosition)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
                    isEnemyInRange = (distanceToEnemy <= attackRange);

                    if (isEnemyInRange && Ally_State != null && Ally_State.fsm != null)
                    {
                        Ally_State.fsm.ChangeState(UnitState.Attack);
                    }
                }
            }
            //프리모드일때
            else if (Ally_Mode == AllyMode.Free)
            {
                SearchEnemy();

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
                    else
                    {
                        Ally_State.fsm.ChangeState(UnitState.Idle);
                    }
                }
                else
                {
                    //transform.position = moveTargetPos;
                    Ally_State.fsm.ChangeState(UnitState.Idle);
                }

                if (haveToMovePosition)
                {
                    if (Vector3.Distance(transform.position, moveTargetPos) > 0.12f)
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
        #endregion

        #region 적 제어
        else if (this.gameObject.tag == UD_CONSTANT.TAG_ENEMY)
        {
            if (SpawnDelay)
            {
                IEnumerator SpawnDelayCoroutine()
                {
                    NavAgent.enabled = true;
                    yield return new WaitForSeconds(0.5f);
                }

                StartCoroutine(SpawnDelayCoroutine());
                SpawnDelay = false;
            }

            MeshRenderer.material.color = colorEnemy;
            enemy_isBaseInRange =
            (Vector3.Distance(transform.position, moveTargetBasePos) <= attackRange);

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
                moveTargetPos = moveTargetBasePos; 
                
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


    public void ChangeAllyMode()
    {
        if (isSelected)
        {
            previousAllyMode = Ally_Mode;
            Ally_Mode = AllyMode.Change;
        }
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
                UnitSkill.UnitGeneralSkill(generalSkillCode, moveTargetPos);
            }
            else
            {
                if (targetEnemy != null)
                {
                    UnitSkill.UnitGeneralSkill(generalSkillCode, targetEnemy.transform.position);
                    //Bow.transform.LookAt(targetEnemy.transform.position);
                    //Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown, attackPoint, true);
                }
                else if (targetEnemy == null)
                {
                    Debug.Log("경로 다시 검색...");
                    Enemy_State.fsm.ChangeState(EnemyState.Move);
                }
            }
        }
    }

    public void UnitInit(UnitSpawnData data)
    {
        modelType = data.modelType;
        maxHP = data.HP;
        moveSpeed = data.speed;
        attackPoint = data.atk;
        sightRange = data.sightRange;
        attackRange = data.attackRange;

        generalSkillCode = data.generalSkill;
        specialSkillCode = data.specialSkill;

        unitType = data.unitType;
    }

    public void EnemyInit(EnemySpawnData data)
    {
        modelType = data.modelType;
        maxHP = data.HP;
        moveSpeed = data.speed;
        attackPoint = data.atk;
        sightRange = data.sightRange;
        attackRange = data.attackRange;
        generalSkillCode = data.generalSkill;
        specialSkillCode = data.specialSkill;

        unitType = data.enemyType;
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

    void MoveUnitToNearestTile()
    {
        float searchRadius = 2.0f;

        Vector3 unitPosition = transform.position;

        Collider[] nearbyTiles = Physics.OverlapSphere(unitPosition, searchRadius);

        float closetDistance = Mathf.Infinity;
        UD_Ingame_GridTile closestTile = null;

        foreach (Collider collider in nearbyTiles)
        {
            UD_Ingame_GridTile tile = collider.GetComponent<UD_Ingame_GridTile>();

            if (tile != null)
            {
                float distanceToTile = Vector3.Distance(unitPosition, tile.transform.position);

                if (distanceToTile < closetDistance)
                {
                    closetDistance = distanceToTile;
                    closestTile = tile;
                }
            }
        }

        if (closestTile != null && !closestTile.isTileOccupied)
        {
            transform.position = closestTile.transform.position;
            closestTile.SetTileOccupied(true);
            closestTile.currentPlacedUnit = this.gameObject;
        }
        else
        {
            searchRadius += 5.0f;
            MoveUnitToNearestTile();
        }
    }

}
