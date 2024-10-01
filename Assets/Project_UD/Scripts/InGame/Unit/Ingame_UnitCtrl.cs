using MonsterLove.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnitDataManager;


public enum DefenseType
{
    cloth,
    metal,
    leather
}


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

[Serializable]
public class Debuff
{
    public int debuffTypeCode;  //디버프 코드
    public bool isStackable;
    public int debuffStack;
    public int debuffTime;
}

[Serializable]
public class Buff
{
    public int buffTypeCode;  //버프 코드
    public bool isStackable;
    public int buffStack;
    public int buffTime;
}


public class Ingame_UnitCtrl : MonoBehaviour
{
    [HideInInspector] public UD_Ingame_UnitState Ally_State;
    [HideInInspector] public EnemyUnitState Enemy_State;
    [HideInInspector] public NavMeshObstacle NavObstacle;
    [HideInInspector] public NavMeshAgent NavAgent;
    [HideInInspector] public UnitSkillManager UnitSkill;
   

    UnitModelSwapManager ModelSwap;

    //Data를 다른 스크립트로 빼는게 좋지 않을까? - 폴라오
    [Header("====Data====")]
    public string unitName;
    public int modelType;
    int cur_modelType;
    public int curLevel = 1;
    public int HP;
    public int maxHP;
    public float moveSpeed;
    public float weaponCooldown;
    public float skillCooldown;
    public int mental = 1;
    public float sightRange = 0;
    public float attackRange = 0;
    public int attackPoint = 1;
    public int critChanceRate;
    public int generalSkillCode = 101;
    public int specialSkillCode = 101;
    public UnitType unitType;
    public DefenseType defenseType;
    public TargetSelectType targetSelectType;

    // LoPol 추가
    public string ID;
    public string g_SkillName;
    public string s_SkillName;
    public int cost;
    public string unitCode;
    //public string DefenseType;

    [Header("====Status====")]
    public AllyMode Ally_Mode;

    public Vector2 unitPos = Vector2.zero;

    public Transform VisualModel;
    public GameObject Selected_Particle;
    public bool isSelected = false;

    public Debuff[] debuffInfo;
    public Buff[] buffInfo;



    [Header("====AI====")]
    bool SpawnDelay = true;

    public GameObject targetBase;

    public Vector3 moveTargetBasePos;
    public Vector3 moveTargetPos;
    public bool haveToMovePosition = false;
    public GameObject targetEnemy = null;
    public RangeCtrl sightRangeSensor;

    public bool isEnemyInSight = false;
    public bool isEnemyInRange = false;
    public bool enemy_isBaseInRange = false;
    public bool enemy_isPathBlocked = false;

    public GameObject findEnemyRange = null;
    public GameObject Bow = null;

    public float testSpeed = 1;

    public float unitStateChangeTime;
    public AllyMode previousAllyMode;


        void OnMouseDown()
    {
        if (Ingame_UIManager.instance != null)
        {
            //UD_Ingame_UIManager.instance.UpdateUnitInfoPanel(this.unitName);
        }
    }

    private void Awake()
    {
        ModelSwap = UnitModelSwapManager.inst;

        NavAgent = GetComponent<NavMeshAgent>();
        NavObstacle = GetComponent<NavMeshObstacle>();

        Ally_State = GetComponent<UD_Ingame_UnitState>();
        Enemy_State = GetComponent<EnemyUnitState>();

        UnitSkill = GetComponentInChildren<UnitSkillManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
        {
            Instantiate(ModelSwap.AllyModel[modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
        }
        else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            Instantiate(ModelSwap.EnemyModel[modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
        }

        SpawnDelay = true;

        targetBase = InGameManager.inst.Base;
        HP = maxHP;

        Ally_Mode = AllyMode.Siege;

        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
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

        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(findEnemyRange.transform.position, attackRange + 0.5f);
    }


    // Update is called once per frame
    void Update()
    {
        //유닛의 현재 위치에 따른 타일 배치 가능 설정
        GridManager.inst.SetTilePlaceable(this.transform.position,false,false);

        //유닛의 현재 위치에 따른 타일 위치 가져오기
        unitPos = GridManager.inst.GetTilePos(this.transform.position);


        //모델 변경
        if (modelType != cur_modelType)
        {
            Destroy(VisualModel.GetChild(0).gameObject);
            if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
            {
                Instantiate(ModelSwap.AllyModel[modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
            }
            else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                Instantiate(ModelSwap.EnemyModel[modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
            }

            cur_modelType = modelType;
        }

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
        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
        {

            if (Ally_Mode == AllyMode.Free)
            {
                NavObstacle.enabled = false;
                NavAgent.enabled = true;
            }
            else if (Ally_Mode == AllyMode.Siege)
            {
                
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
                    InGameManager.inst.AllUnitSelectOff();
                    if (previousAllyMode == AllyMode.Free)
                    {
                        Ally_Mode = AllyMode.Siege;
                        
                        IEnumerator ModeChangeDelayCoroutine()
                        {
                            NavAgent.enabled = false;
                            this.transform.rotation = new Quaternion(0, 0, 0, 0);
                            yield return new WaitForSeconds(0.5f);
                        }
                        StartCoroutine(ModeChangeDelayCoroutine());

                        NavObstacle.enabled = true;
                        SearchEnemy();
                    }
                    else if (previousAllyMode == AllyMode.Siege)
                    {
                        //NavAgent.updatePosition = false;

                        NavObstacle.enabled = false;
                        
                        IEnumerator ModeChangeDelayCoroutine()
                        {
                            yield return new WaitForEndOfFrame();
                            yield return new WaitForEndOfFrame();

                            NavAgent.enabled = true;
                        }
                        StartCoroutine(ModeChangeDelayCoroutine());

                        //NavAgent.Warp(transform.position);

                        Ally_Mode = AllyMode.Free;
                        //NavAgent.updatePosition = true;
                    }
                    unitStateChangeTime = 3;
                }
            }
            //시즈모드일때
            else if (Ally_Mode == AllyMode.Siege)
            {
                UnitSkill.UnitSpecialSkill(specialSkillCode, moveTargetPos, skillCooldown);
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
                UnitSkill.UnitSpecialSkill(specialSkillCode, moveTargetPos, skillCooldown);
                if (haveToMovePosition)
                {
                    targetEnemy = null;
                    Vector2 CurPos = new Vector2(transform.position.x, transform.position.z);

                    if (Vector2.Distance(CurPos, new Vector2(moveTargetPos.x, moveTargetPos.z)) >= 0.15f)
                    {
                        Ally_State.fsm.ChangeState(UnitState.Move);
                    }
                    else
                    {
                        haveToMovePosition = false;
                        this.transform.position = moveTargetPos;
                        Ally_State.fsm.ChangeState(UnitState.Idle);
                    }
                }
                else
                {
                    SearchEnemy();

                    if (targetEnemy != null)
                    {
                        isEnemyInRange = (Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRange);
                        //TODO : 이동하려는 칸에 적이 있을경우 시야범위 안에 들어오면 추격 모드로 변경.
                    }

                    if (targetEnemy != null && !haveToMovePosition)
                    {
                        if (isEnemyInSight)
                        {
                            haveToMovePosition = false;

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
                }
            }
        }
        #endregion

        #region 적 제어
        else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
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
                sightRangeSensor.NearestObjectSearch(attackRange, this.gameObject.CompareTag(CONSTANT.TAG_ENEMY));

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
        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
        {
            if (targetEnemy == null)
            {
                Ally_State.fsm.ChangeState(UnitState.Search);
                return;
            }
            else
            {
                VisualModel.transform.LookAt(targetEnemy.transform.position);
                UnitSkill.UnitGeneralSkill(generalSkillCode, targetEnemy.transform.position, false);
                //Bow.transform.LookAt(targetEnemy.transform.position);
                //Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown, attackPoint, false);
            }
        }
        else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            if (enemy_isBaseInRange)
            {
                UnitSkill.UnitGeneralSkill(generalSkillCode, moveTargetPos, true);
            }
            else
            {
                if (targetEnemy != null)
                {
                    UnitSkill.UnitGeneralSkill(generalSkillCode, targetEnemy.transform.position, true);
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
        moveSpeed = data.moveSpeed;
        weaponCooldown = data.attackSpeed;
        skillCooldown = data.skillCooldown;
        attackPoint = data.attackPoint;
        critChanceRate = data.critChanceRate;

        sightRange = data.sightRange;
        attackRange = data.attackRange;

        generalSkillCode = data.generalSkill;
        specialSkillCode = data.specialSkill;

        unitType = data.unitType;
        defenseType = data.defenseType;
        targetSelectType = data.targetSelectType;
        unitCode = data.unitCode;
        unitName = data.unitName;
        curLevel = data.level;
        skillCooldown = data.skillCooldown;
    }

    public void EnemyInit(EnemySpawnData data)
    {
        modelType = data.modelType;
        maxHP = data.HP;
        moveSpeed = data.moveSpeed;
        attackPoint = data.attackPoint;
        critChanceRate = data.critChanceRate;

        sightRange = data.sightRange;
        attackRange = data.attackRange;

        generalSkillCode = data.generalSkill;
        specialSkillCode = data.specialSkill;

        unitType = data.unitType;
        defenseType = data.defenseType;
        targetSelectType = data.targetSelectType;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject OBJ = other.gameObject;
        //피격 판정
        if (OBJ.CompareTag(CONSTANT.TAG_ATTACK))
        {
            AttackCtrl Attack = OBJ.GetComponent<AttackCtrl>();

            if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT) && Attack.isEnemyAttack)
            {
                //Debug.Log(this.gameObject.name + " attack hit!");
                this.HP -= Attack.Atk;
                Destroy(OBJ);
            }
            else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY) && !Attack.isEnemyAttack)
            {
                //Debug.Log(this.gameObject.name + " attack hit!");

                if (Attack.Type == AttackType.Magic)
                {

                }
                else
                {
                    ReceivePhysicalDamage(Attack, Attack.Atk, Attack.CritPercent, Attack.Type);
                    if (Attack.MethodType == AttackMethod.Trap)
                    {
                        Destroy(OBJ);
                        //GameObject.FindObjectOfType<UD_Ingame_GridManager>().SetTilePlaceable( , true);
                    }
                    else
                    {
                        this.HP -= Attack.Atk;
                        Destroy(OBJ);
                    }
                }
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(this.gameObject.name + " attack collision hit!");

        if (!collision.gameObject.CompareTag(CONSTANT.TAG_GROUND) && !collision.gameObject.CompareTag(CONSTANT.TAG_TILE))
        {
            Debug.Log(this.gameObject.name + " collision hit at : " + collision.gameObject.name);
            NavAgent.ResetPath();

            if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
            {
                Ally_State.fsm.ChangeState(UnitState.Idle);
            }
            else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                Enemy_State.fsm.ChangeState(EnemyState.Idle);
            }

        }

    }

    void ReceivePhysicalDamage(AttackCtrl AtkObj, int Damage, float Crit, AttackType attackType)
    {
        if (defenseType == DefenseType.cloth)
        {
            if (attackType == AttackType.Slash)
            {
                Crit += 30;
                Damage += (int)(Damage * 0.4f);
            }
            else if (attackType == AttackType.Pierce)
            {
                Crit -= 30;
                Damage -= (int)(Damage * 0.3f);
            }
            else if (attackType == AttackType.Crush)
            {
                Crit += 0;
                Damage += 0;
            }
        }
        else if (defenseType == DefenseType.leather)
        {
            if (attackType == AttackType.Slash)
            {
                Crit += 0;
                Damage += 0;
            }
            else if (attackType == AttackType.Pierce)
            {
                Crit += 30;
                Damage += (int)(Damage * 0.4f);
            }
            else if (attackType == AttackType.Crush)
            {
                Crit -= 30;
                Damage -= (int)(Damage * 0.3f);
            }
        }
        else if (defenseType == DefenseType.metal)
        {
            if (attackType == AttackType.Slash)
            {
                Crit -= 30;
                Damage -= (int)(Damage * 0.3f);
            }
            else if (attackType == AttackType.Pierce)
            {
                Crit += 0;
                Damage += 0;
            }
            else if (attackType == AttackType.Crush)
            {
                Crit += 30;
                Damage += (int)(Damage * 0.4f);
            }
        }

        if (Damage <= 0)
        {
            Damage = 0;
        }

        if (Crit <= 0)
        {
            Crit = 0;
        }
        else if (Crit >= 100)
        {
            Crit = 100;
        }

        this.HP -= Damage;
        if (UnityEngine.Random.Range(0, 100) <= Crit)
        {
            Debug.Log("Critical Hit!");
        }

    }
}