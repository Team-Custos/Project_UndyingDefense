using MonsterLove.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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

public class Ingame_UnitCtrl : MonoBehaviour
{
    [HideInInspector] public AllyUnitState Ally_State;
    [HideInInspector] public EnemyUnitState Enemy_State;
    [HideInInspector] public NavMeshObstacle NavObstacle;
    [HideInInspector] public NavMeshAgent NavAgent;
    [HideInInspector] public UnitSkillManager UnitSkill;

    public Ingame_UnitDataStatus DataStatus;
    UnitDebuffManager debuffManager;
    UnitModelSwapManager ModelSwap;
    
    [Header("====Status====")]
    public AllyMode Ally_Mode;

    public Vector2 unitPos = Vector2.zero;

    public Transform VisualModel;
    public GameObject Selected_Particle;
    public bool isSelected = false;

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

    public string unitName;

    void OnMouseDown()
    {
        if (Ingame_UIManager.instance != null)
        {
            Ingame_UIManager.instance.UpdateUnitInfoPanel(this.unitName);
        }
    }

    private void Awake()
    {
        ModelSwap = UnitModelSwapManager.inst;
        if (DataStatus == null)
        {
            DataStatus = GetComponent<Ingame_UnitDataStatus>();
        }
        debuffManager = GetComponent<UnitDebuffManager>();


        NavAgent = GetComponent<NavMeshAgent>();
        NavObstacle = GetComponent<NavMeshObstacle>();

        Ally_State = GetComponent<AllyUnitState>();
        Enemy_State = GetComponent<EnemyUnitState>();

        UnitSkill = GetComponentInChildren<UnitSkillManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
        {
            Instantiate(ModelSwap.AllyModel[DataStatus.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
        }
        else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            Instantiate(ModelSwap.EnemyModel[DataStatus.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
        }

        SpawnDelay = true;

        targetBase = InGameManager.inst.Base;
        DataStatus.HP = DataStatus.maxHP;

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
        Gizmos.DrawWireSphere(findEnemyRange.transform.position, DataStatus.attackRange + 0.5f);
    }


    // Update is called once per frame
    void Update()
    {
        


        //유닛의 현재 위치에 따른 타일 배치 가능 설정
        GridManager.inst.SetTilePlaceable(this.transform.position,false,false);

        //유닛의 현재 위치에 따른 타일 위치 가져오기
        unitPos = GridManager.inst.GetTilePos(this.transform.position);


        //모델 변경
        if (DataStatus.modelType != DataStatus.cur_modelType)
        {
            Destroy(VisualModel.GetChild(0).gameObject);
            if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
            {
                Instantiate(ModelSwap.AllyModel[DataStatus.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
            }
            else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                Instantiate(ModelSwap.EnemyModel[DataStatus.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
            }

            DataStatus.cur_modelType = DataStatus.modelType;
        }

        Selected_Particle.SetActive(isSelected);
        //findEnemyRange.SetActive(isSelected);
        moveTargetBasePos = new Vector3(targetBase.transform.position.x, this.transform.position.y, this.transform.position.z);
        sightRangeSensor.radius = DataStatus.sightRange;

        NavAgent.speed = DataStatus.moveSpeed;

        if (DataStatus.HP <= 0)
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

            if (Input.GetKeyDown(KeyCode.B) && isSelected)
            {
                debuffManager.AddUnitDebuff(UnitDebuff.Bleed);
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
                UnitSkill.UnitSpecialSkill(DataStatus.specialSkillCode, DataStatus.skillCooldown);
                if (targetEnemy == null && !isEnemyInSight)
                {
                    SearchEnemy();
                }

                if (targetEnemy != null && !haveToMovePosition)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
                    isEnemyInRange = (distanceToEnemy <= DataStatus.attackRange);

                    if (isEnemyInRange && Ally_State != null && Ally_State.fsm != null)
                    {
                        Ally_State.fsm.ChangeState(UnitState.Attack);
                    }
                }
            }
            //프리모드일때
            else if (Ally_Mode == AllyMode.Free)
            {
                UnitSkill.UnitSpecialSkill(DataStatus.specialSkillCode, DataStatus.skillCooldown);
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
                        isEnemyInRange = (Vector3.Distance(transform.position, targetEnemy.transform.position) <= DataStatus.attackRange);
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
            (Vector3.Distance(transform.position, moveTargetBasePos) <= DataStatus.attackRange);

            if (enemy_isPathBlocked)
            {
                if (targetEnemy != null && !enemy_isBaseInRange)//병사 발견시
                {
                    isEnemyInRange =
                        (Vector3.Distance(transform.position, targetEnemy.transform.position) <= DataStatus.attackRange);

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
                sightRangeSensor.NearestObjectSearch(DataStatus.attackRange, this.gameObject.CompareTag(CONSTANT.TAG_ENEMY));

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
                UnitSkill.UnitGeneralSkill(DataStatus.generalSkillCode, targetEnemy, DataStatus.weaponCooldown,false);
            }
        }
        else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            if (enemy_isBaseInRange)
            {
                UnitSkill.UnitGeneralSkill(DataStatus.generalSkillCode, targetBase, DataStatus.weaponCooldown, true);
            }
            else
            {
                if (targetEnemy != null)
                {
                    UnitSkill.UnitGeneralSkill(DataStatus.generalSkillCode, targetEnemy,DataStatus.weaponCooldown , true);
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
        DataStatus.modelType = data.modelType;
        DataStatus.maxHP = data.HP;
        DataStatus.moveSpeed = data.moveSpeed;
        DataStatus.weaponCooldown = data.attackSpeed;
        DataStatus.skillCooldown = data.skillCooldown;
        DataStatus.attackPoint = data.attackPoint;
        DataStatus.critChanceRate = data.critChanceRate;

        DataStatus.sightRange = data.sightRange;
        DataStatus.attackRange = data.attackRange;

        DataStatus.generalSkillCode = data.generalSkill;
        DataStatus.specialSkillCode = data.specialSkill;
        

        DataStatus.unitType = data.unitType;
        DataStatus.defenseType = data.defenseType;
        DataStatus.targetSelectType = data.targetSelectType;
    }

    public void EnemyInit(EnemySpawnData data)
    {
        DataStatus.modelType = data.modelType;
        DataStatus.maxHP = data.HP;
        DataStatus.moveSpeed = data.moveSpeed;
        DataStatus.attackPoint = data.attackPoint;
        DataStatus.critChanceRate = data.critChanceRate;

        DataStatus.sightRange = data.sightRange;
        DataStatus.attackRange = data.attackRange;

        DataStatus.generalSkillCode = data.generalSkill;
        DataStatus.specialSkillCode = data.specialSkill;

        DataStatus.unitType = data.unitType;
        DataStatus.defenseType = data.defenseType;
        DataStatus.targetSelectType = data.targetSelectType;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject OBJ = other.gameObject;
        //피격 판정 -> 삭제 예정
        if (OBJ.CompareTag(CONSTANT.TAG_ATTACK))
        {
            AttackCtrl Attack = OBJ.GetComponent<AttackCtrl>();

            if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT) && Attack.isEnemyAttack)
            {
                //Debug.Log(this.gameObject.name + " attack hit!");
                DataStatus.HP -= Attack.Atk;
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
                    ReceivePhysicalDamage(Attack.Atk, DataStatus.critChanceRate + Attack.CritPercentAdd, Attack.Type);
                    if (Attack.MethodType == AttackMethod.Trap)
                    {
                        Destroy(OBJ);
                        //GameObject.FindObjectOfType<UD_Ingame_GridManager>().SetTilePlaceable( , true);
                    }
                    else
                    {
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

    //물리적 데미지
    public void ReceivePhysicalDamage(int Damage, float Crit, AttackType attackType)
    {
        if (DataStatus.defenseType == DefenseType.cloth)
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
        else if (DataStatus.defenseType == DefenseType.leather)
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
        else if (DataStatus.defenseType == DefenseType.metal)
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

        DataStatus.HP -= Damage;
        if (UnityEngine.Random.Range(0, 100) <= Crit)
        {
            Debug.Log("Critical Hit!");


        }
    }


}