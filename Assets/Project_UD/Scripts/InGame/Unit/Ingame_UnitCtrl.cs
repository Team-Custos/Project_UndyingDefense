using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//이 스크립트는 유닛의 전반적인 상태와 행동을 관리하기 위한 스크립트입니다. (아군과 적군 통합.)

//public enum UnitType //유닛의 종류.
//{
//    None = -1,
//    민병,
//    사냥꾼,
//    창병,
//    한량,
//    척후병,
//    포수,
//}

//public enum DefenseType //유닛의 방어속성.
//{
//    cloth,
//    metal,
//    leather
//}

//public enum AllyMode//아군의 모드.
//{
//    Siege,
//    Free,
//    Change
//}

//public enum TargetSelectType//유닛의 타겟 선정 방식.
//{
//    Nearest,
//    LowestHP,
//    Fixed
//}


// 유닛(아군, 적)을 관리하는 스크립트
// 

public class Ingame_UnitCtrl : MonoBehaviour
{
    [HideInInspector] public AllyUnitState Ally_State;
    [HideInInspector] public EnemyUnitState Enemy_State;
    [HideInInspector] public NavMeshObstacle NavObstacle;
    [HideInInspector] public NavMeshAgent NavAgent;
    [HideInInspector] public UnitSkillManager UnitSkill;


    public Ingame_UnitData unitData; //유닛의 데이터 (스크립터블 오브젝트.)
    public UnitDebuffManager debuffManager; //디버프 관리.

    public UnitSoundManager soundManager; //유닛의 SFX 관리.

    UnitUICtrl unitUiCtrl;

    private GridTile currentTile;

    [Header("====Status====")]
    public AllyMode Ally_Mode;

    public Vector2 unitPos = Vector2.zero;
    public Quaternion unitDefaultRotation;

    public Transform VisualModel; //모델 변경을 위한 오브젝트.
    //public UnitAnimationParaCtrl AnimationCtrl;
    public GameObject Selected_Particle; //선택 여부를 위한 효과.
    public bool isSelected = false; //선택 여부.
    public int cur_modelType; //현재 적용된 모델.
    public float HP; //유닛의 현재 체력.
    public float cur_moveSpeed = 1;//유닛의 현재 이동속도. (디버프로 인한 속도 관리를 위함.)
    public float cur_attackSpeed = 1;//유닛의 현재 공격속도. (디버프로 인한 속도 관리를 위함.)
    public bool unActable = false;//행동할 수 없는가?

    public int maxHp;//최대 체력.
    public Image hpBar;//유닛의 체력 바.
    public bool isDead = false;//유닛이 죽었는가?

    public bool isAttacking = false;

    [Header("====AI====")]
    bool SpawnDelay = true;
    public bool SpawnIdleEnd = false;

    public GameObject targetBase; //적군을 위한 성의 위치.

    public Vector3 moveTargetBasePos; //성으로 가기위한 목적지 좌표.
    public Vector3 moveTargetPos;//목적지 좌표.
    public bool haveToMovePosition = false; //모든 행동을 무시하고 이동해야 하는가?
    public GameObject targetEnemy = null;//현재 타겟 유닛.
    public RangeCtrl sightRangeSensor;//시야범위 센서.

    public bool isEnemyInSight = false;//적이 시야 범위 내에 있는가?
    public bool isEnemyInRange = false;//적이 공격 범위 내에 있는가?
    float isEnemyInRangeDelay = 0.7f;
    float isEnemyInRangeDelay_Cur = 0;


    public bool enemy_isBaseInRange = false;//성이 공격 범위 내에 있는가? (적군 전용)
    public bool enemy_isPathBlocked = false;//성까지 가는 길이 막혀있는가? (적군 전용)

    public GameObject findEnemyRange = null; //시야범위
    public GameObject Bow = null; //활 오브젝트

    public float unitStateChangeTime; //유닛의 모드를 변경할때의 대기 시간.
    public AllyMode previousAllyMode; //유닛의 마지막으로 설정된 모드.

    public int enmeyRewardGold = 10; // 적 처치시 보상

    public System.Action<GameObject> OnDisable;
    public System.Action<Ingame_UnitCtrl> OnUnitDead;

    public Quaternion defaultTargetRotation;
    private void Awake()
    {
        debuffManager = GetComponent<UnitDebuffManager>();
        if (soundManager == null)
        {
            soundManager = GetComponentInChildren<UnitSoundManager>();
        }

        NavAgent = GetComponent<NavMeshAgent>();
        NavObstacle = GetComponent<NavMeshObstacle>();

        if (gameObject.CompareTag(CONSTANT.TAG_UNIT))
        {
            Ally_State = GetComponent<AllyUnitState>();
        }
        else if (gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            Enemy_State = GetComponent<EnemyUnitState>();
        }

        UnitSkill = GetComponentInChildren<UnitSkillManager>();
    }

    public void StatsInit()
    {
        maxHp = unitData.maxHP;//유닛의 최대체력 변수 초기화.
        HP = maxHp; //최대체력의 최종 변수로 현재 체력을 초기화.
        cur_moveSpeed = unitData.moveSpeed;//현재 이동 속도를 데이터의 이동속도로 초기화.
        UnitSkill.AttackTrigger = unitData.attackVFX;
    }


    // Start is called before the first frame update
    void Start()
    {
        StatsInit();

        currentTile = GetComponentInParent<GridTile>();

        unitUiCtrl = GetComponent<UnitUICtrl>();

        transform.rotation = unitDefaultRotation;
        defaultTargetRotation = unitDefaultRotation;

        //유닛 스폰시 모델 설정.
        Instantiate(unitData.modelPrefab, VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);

        SpawnDelay = true;

        targetBase = InGameManager.inst.Base; //성 오브젝트.
        

        //if()//삼계탕 특식 효과
        //{
        //    maxHp = unitData.maxHP + 2;
        //}

        


        Ally_Mode = AllyMode.Siege;//시즈모드로 스폰.

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
        else
        {
            NavAgent.enabled = true;
            NavObstacle.enabled = false;
        }

        moveTargetPos = this.transform.position;
        VisualModel.transform.localRotation = Quaternion.Euler(0, 0, 0);

        // 초기 모델 설정 후 크기 애니메이션 적용
        if (VisualModel.childCount > 0)
        {
            Transform initialModel = VisualModel.GetChild(0);
            StartCoroutine(ScaleAnimation(initialModel));
        }

        //UpdateUnitDataFromExcel(unitData);


        enmeyRewardGold = this.unitData.gold;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(findEnemyRange.transform.position, unitData.attackRange + 0.5f);
    }

    public void ModelSwap()//모델 변경 함수.
    {
        if (unitData.modelPrefab.name != VisualModel.GetChild(0).gameObject.name)
        {
            Destroy(VisualModel.GetChild(0).gameObject);
            GameObject CurModel = Instantiate(unitData.modelPrefab, VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);

            CurModel.name = unitData.modelPrefab.name;

            if(!UnitUpgradeManager.Instance.isUpgrade)
            {
                // 스케일 애니메이션 시작
                StartCoroutine(ScaleAnimation(CurModel.transform));
            }

            //if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
            //{
            //    Instantiate(ModelSwapManager.AllyModel[unitData.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
            //}
            //else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            //{
            //    Instantiate(ModelSwapManager.EnemyModel[unitData.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
            //}

            //cur_modelType = unitData.modelType;
        }
    }

    private void Update()
    {
        if (HP > 0)
        {
            // 유닛이 살아있다면, 해당 타일을 배치 불가(false)로
            GridManager.inst.SetTilePlaceable(transform.position, false, false);
        }
        else
        {
            // 유닛이 죽었거나 체력이 0 이하라면, 해당 타일을 다시 배치 가능(true)로
            GridManager.inst.SetTilePlaceable(transform.position, true, true);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)HP / (float)maxHp;
        }


        //if (HP > 0 && !isDead)
        //{
        //    // 유닛이 살아있다면, 해당 타일을 배치 불가(false)로
        //    GridManager.inst.SetTilePlaceable(transform.position, false, false);
        //}
        //else
        //{
        //    // 유닛이 죽었거나 체력이 0 이하라면, 해당 타일을 다시 배치 가능(true)로
        //    GridManager.inst.SetTilePlaceable(transform.position, true, true);
        //}

        //유닛의 현재 위치에 따른 타일 배치 가능 설정


        //유닛의 현재 위치에 따른 타일 위치 가져오기
        unitPos = GridManager.inst.GetTilePos(this.transform.position);

        //모델 변경 -> 업그레이드 시에만 호출
        //ModelSwap();

        Vector3 targetPosition = BaseStatus.instance.GetNearestPosition(transform.position);
        moveTargetBasePos = targetPosition;//성의 좌표 초기화.

        NavAgent.speed = cur_moveSpeed;//이동속도 설정 

        //GridManager.inst.SetTilePlaceable(this.transform.position, false, false);

        if (Input.GetKeyDown(KeyCode.H) && isSelected) //선택된 유닛 삭제. 디버그용.
        {
            if(this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                WaveManager.inst.MonsterDead(this.gameObject, 0.0f);
            }
            else 
            {
                Destroy(this.gameObject);
                Ingame_UIManager.instance.DestroyUnitStateChangeBox();
                Ingame_UIManager.instance.DestroyUnitUpgradeMenu();
                Ingame_UIManager.instance.DestorypgradeMenuConfirmBox();
            }

            Ingame_UIManager.instance.unitInfoPanel.SetActive(false);

            GridManager.inst.SetTilePlaceable(transform.position, true, true);

        }

        #region 아군 제어
        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
        {
            AllyCtrl();
        }
        #endregion

        #region 적 제어
        else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            EnemyCtrl();
        }
        #endregion

        if(this.gameObject == GameOrderSystem.instance.selectedUnit)
        {
            if (HP <= 0)
            {
                HP = 0;
                Ingame_UIManager.instance.hpText.text = 0 + " / " + maxHp;
            }
            else
            {
                Ingame_UIManager.instance.hpText.text = HP + " / " + maxHp;
            }


        }
    }


    void AllyCtrl()
    {
        if (isDead)
        {
            Ally_State.fsm.ChangeState(PUState.Dead);
            return;
        }


        //if (targetEnemy != null && targetEnemy.GetComponent<Ingame_UnitCtrl>().HP <= 0)
        //{
        //    sightRangeSensor.ListTargetDelete(targetEnemy);
        //    targetEnemy = null;
        //}


        sightRangeSensor.SetRadius(unitData.sightRange);

        if (Input.GetKeyDown(KeyCode.B) && isSelected)//유닛의 디버프 적용. 디버그용.
        {
            debuffManager.AddDebuff(UnitDebuff.Bleed);
        }

        if (isSelected && Input.GetKeyDown(KeyCode.Z))//선택된 유닛의 모드 변경. 디버그용. 삭제 예정.
        {
            previousAllyMode = Ally_Mode;
            Ally_Mode = AllyMode.Change;
            Ingame_ParticleManager.Instance.PlayUnitModeChangeParticleEffect(transform, -0.8f);

            Ingame_UIManager.instance.DestroyUnitStateChangeBox();
            Ingame_UIManager.instance.DestroyUnitUpgradeMenu();
            Ingame_UIManager.instance.DestorypgradeMenuConfirmBox();
        }

        // Change 상태일 때는 다른 행동을 하지 않음
        if (Ally_Mode == AllyMode.Change)
        {
            //Ingame_UIManager.instance.ShowUnitStateUI(this.gameObject, false, false);
            //Ingame_ParticleManager.Instance.PlaySiegeModeEffect(this.gameObject, false);
            unitUiCtrl.OnOffSiegeEffect(false);

            if (unitStateChangeTime > 0)
            {
                unitStateChangeTime -= Time.deltaTime;
                if (Ally_State != null && Ally_State.fsm != null)
                {
                    Ally_State.fsm.ChangeState(PUState.Idle); // 움직임을 멈추게 함
                }
                else
                {
                    Debug.LogError("Ally_State or FSM is null in " + this.gameObject.name);
                }
                return;
            }
            else
            {
                InGameManager.inst.AllUnitSelectOff(); //모든 유닛의 선택 해제.

                //유닛의 모드 변경.
                if (previousAllyMode == AllyMode.Free)
                {
                    Ally_Mode = AllyMode.Siege;

                    IEnumerator ModeChangeDelayCoroutine()
                    {
                        NavAgent.enabled = false;
                        this.transform.rotation = unitDefaultRotation;
                        yield return new WaitForSeconds(0.5f);
                    }
                    StartCoroutine(ModeChangeDelayCoroutine());

                    NavObstacle.carving = true;
                    NavObstacle.enabled = true;
                    SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_toSiege);

                    SearchEnemy();
                }
                else if (previousAllyMode == AllyMode.Siege)
                {
                    IEnumerator ModeChangeDelayCoroutine()
                    {
                        NavObstacle.carving = false;
                        NavObstacle.enabled = false;
                        yield return new WaitForSeconds(0.5f);
                        NavAgent.enabled = true;
                    }
                    StartCoroutine(ModeChangeDelayCoroutine());

                    Ally_Mode = AllyMode.Free;

                    SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_toFree);
                }
                unitStateChangeTime = 3;
            }
        }
        //시즈모드일때
        else if (Ally_Mode == AllyMode.Siege)
        {
            if (targetEnemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
                //Debug.Log("distanceToEnemy : " + distanceToEnemy);
                isEnemyInRange = (distanceToEnemy <= unitData.attackRange);

                if (targetEnemy.GetComponent<Ingame_UnitCtrl>().HP <= 0f || !isEnemyInRange)
                {
                    targetEnemy = null;
                }
            }

            //Ingame_ParticleManager.Instance.PlaySiegeModeEffect(this.gameObject, true);
            unitUiCtrl.OnOffSiegeEffect(true);

            haveToMovePosition = false;
            //Ingame_UIManager.instance.ShowUnitStateUI(this.gameObject, false, true);

            UnitSkill.UnitSpecialSkill(unitData.specialSkill, unitData.specialSkill.coolTime);//유닛의 특수 스킬.
            if (targetEnemy == null || sightRangeSensor.detectedObjects.Count <= 0)//적군이 시야 범위 내에 없을경우.
            {
                isEnemyInSight = false;
                Ally_State.fsm.ChangeState(PUState.Idle);
            }


            if (targetEnemy != null && !haveToMovePosition) // 타겟 적군이 있을 경우.
            {
                //float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
                //isEnemyInRange = (distanceToEnemy <= unitData.attackRange);//적군이 유닛 공격 범위 내에 있는가?

                if (isEnemyInRange && Ally_State != null && Ally_State.fsm != null)
                {
                    Ally_State.fsm.ChangeState(PUState.Attack);
                }
            }
            else
            {
                isEnemyInRange = false;
            }

        }
        //프리모드일때
        else if (Ally_Mode == AllyMode.Free)
        {
            if (targetEnemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);

                isEnemyInSight = (distanceToEnemy <= unitData.sightRange);
                isEnemyInRange = (distanceToEnemy <= unitData.attackRange);

                if (targetEnemy.GetComponent<Ingame_UnitCtrl>().HP <= 0f || !isEnemyInSight)
                {
                    targetEnemy = null;
                }
            }

            unitUiCtrl.OnOffSiegeEffect(false);

            UnitSkill.UnitSpecialSkill(unitData.specialSkill, unitData.specialSkill.coolTime);//유닛의 특수 스킬.
            if (haveToMovePosition)//모든 행동을 무시하고 이동해야하는가?
            {
                targetEnemy = null;

                Vector2 CurPos = new Vector2(transform.position.x, transform.position.z);

                if (Vector2.Distance(CurPos, new Vector2(moveTargetPos.x, moveTargetPos.z)) >= 0.15f)//목적지에 도착할떄까지
                {
                    Ally_State.fsm.ChangeState(PUState.Move);//이동 상태 설정.
                }
                else
                {
                    //초기화 후 상태 변경.
                    haveToMovePosition = false;
                    this.transform.position = moveTargetPos;
                    Ally_State.fsm.ChangeState(PUState.Idle);
                }
            }
            else
            {
                if (targetEnemy == null || sightRangeSensor.detectedObjects.Count <= 0)
                {
                    isEnemyInSight = false;
                    Ally_State.fsm.ChangeState(PUState.Idle);
                }

                if (targetEnemy != null)
                {
                    if (isEnemyInRange)
                    {
                        isEnemyInRangeDelay_Cur = 0;
                    }
                    //else if (isEnemyInRange)
                    //{
                    //    isEnemyInRangeDelay_Cur += Time.deltaTime;

                    //    if (isEnemyInRangeDelay_Cur >= isEnemyInRangeDelay)
                    //    {
                    //        isEnemyInRange = false;
                    //    }
                    //}
                }
                else
                {
                    isEnemyInRange = false;
                }

                if (targetEnemy != null && !haveToMovePosition)
                {
                    if (isEnemyInSight)//시야 범위 내에 있는가?
                    {
                        if (isEnemyInRange)//공격 범위 내에 있는가?
                        {
                            moveTargetPos = this.transform.position;
                            Ally_State.fsm.ChangeState(PUState.Attack);
                        }
                        else if(!isAttacking)
                        {
                            Ally_State.fsm.ChangeState(PUState.Chase);
                        }
                    }
                    else
                    {
                        Ally_State.fsm.ChangeState(PUState.Idle);
                    }
                }
            }
        }
    }
    void EnemyCtrl()
    {
        if (isDead)
        {
           Enemy_State.fsm.ChangeState(EnemyState.Dead);
            return;
        }

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

        if (!SpawnIdleEnd)
        {
            Enemy_State.fsm.ChangeState(EnemyState.Idle);
            return;
        }

        enemy_isBaseInRange =
        (Vector3.Distance(transform.position, moveTargetBasePos) <= unitData.attackRange); //성이 적군의 공격 범위 내에 있는지 판단.

        sightRangeSensor.SetRadius(unitData.sightRange);

        if (Input.GetKeyDown(KeyCode.B) && isSelected)//유닛의 디버프 적용. 디버그용.
        {
            debuffManager.AddDebuff(UnitDebuff.Bleed);
        }


        if (unActable)//행동 불가 상태인가?
        {
            cur_moveSpeed = 0;
            Enemy_State.fsm.ChangeState(EnemyState.Idle);
            return;
        }
        else
        {
            cur_moveSpeed = unitData.moveSpeed;
        }

        if (enemy_isPathBlocked)//길이 막혀있는가?
        {
            if (targetEnemy != null && !enemy_isBaseInRange)//병사 발견시
            {
                isEnemyInRange =
                    (Vector3.Distance(transform.position, targetEnemy.transform.position) <= unitData.attackRange);

                if (isEnemyInSight)
                {
                    if (isEnemyInRange)
                    {
                        Enemy_State.fsm.ChangeState(EnemyState.Attack);
                    }
                    else
                    {
                        VisualModel.transform.rotation = transform.rotation;
                        Enemy_State.fsm.ChangeState(EnemyState.Move);
                    }
                }
            }
        }
        else // 성 공격
        {
            moveTargetPos = moveTargetBasePos;

            if (enemy_isBaseInRange)
            {
                Enemy_State.fsm.ChangeState(EnemyState.Attack);
            }
            else
            {
                VisualModel.transform.rotation = transform.rotation;
                Enemy_State.fsm.ChangeState(EnemyState.Move);
            }

        }

        if (!isEnemyInRange && !enemy_isBaseInRange)
        {
            VisualModel.transform.rotation = transform.rotation;
            Enemy_State.fsm.ChangeState(EnemyState.Move);
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
            //GameObject TargetObj =
            //    sightRangeSensor.NearestObjectSearch();

            if (sightRangeSensor.NearestUnit != null)
            {
                GameObject targetObj = sightRangeSensor.NearestUnit.gameObject;

                isEnemyInSight = true;
                targetEnemy = targetObj;

                if (Ally_Mode == AllyMode.Free)
                {
                    moveTargetPos = targetObj.transform.position;
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
        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT)) //아군일때
        {
            if (targetEnemy == null)
            {
                Ally_State.fsm.ChangeState(PUState.Search);
                return;
            }
            else if (isEnemyInRange)
            {
                VisualModel.transform.LookAt(targetEnemy.transform.position);
                UnitSkill.UnitGeneralSkill(unitData.generalSkill, targetEnemy, unitData.generalSkill.coolTime, false);
            }
        }
        else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY)) //적일때
        {
            if (enemy_isBaseInRange)
            {
                VisualModel.transform.LookAt(targetBase.transform.position);
                UnitSkill.EnemyGeneralSkill(unitData.generalSkill, targetBase, unitData.generalSkill.coolTime, true);
            }
            else
            {
                if (targetEnemy != null)
                {
                    Debug.Log("SkillCode : " + unitData.generalSkill);
                    Debug.Log("targetEnemy." + targetEnemy.name);

                    VisualModel.transform.LookAt(targetEnemy.transform.position);
                    UnitSkill.EnemyGeneralSkill(unitData.generalSkill, targetEnemy, unitData.generalSkill.coolTime, true);
                }
                else if (targetEnemy == null)
                {
                    Debug.Log("경로 다시 검색...");
                    Enemy_State.SearchPath();
                    Enemy_State.fsm.ChangeState(EnemyState.Move);
                }
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(CONSTANT.TAG_GROUND) && !collision.gameObject.CompareTag(CONSTANT.TAG_TILE))
        {
            Debug.Log(this.gameObject.name + " collision hit at : " + collision.gameObject.name);
            NavAgent.ResetPath();

            if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
            {
                Ally_State.fsm.ChangeState(PUState.Idle);
            }
            else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                Enemy_State.fsm.ChangeState(EnemyState.Idle);
            }
        }
    }

    //틱 데미지용 함수.
    public void ReceiveTickDamage(int Damage = 1)
    {
        HP -= Damage;

        if (HP <= 0 && !isDead)//사망판정
        {
            HP = 0;

            soundManager.PlaySFX(soundManager.DEAD_SFX, soundManager.DeadSound[Random.Range(0, soundManager.DeadSound.Length)]);

            if (gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                isDead = true;
                Ingame_ParticleManager.Instance.EnemyDeathEffect(this.transform);
                WaveManager.inst.MonsterDead(this.gameObject, 2.0f);
                InGameManager.inst.gold += enmeyRewardGold;
                Ingame_UIManager.instance.goldTxt.text = InGameManager.inst.gold.ToString();
            }
            else if (gameObject.CompareTag(CONSTANT.TAG_UNIT))
            {
                isDead = true;
                Ingame_UIManager.instance.DestroyUnitStateChangeBox();
                Ingame_UIManager.instance.DestroyUnitUpgradeMenu();
                Ingame_UIManager.instance.DestorypgradeMenuConfirmBox();
            }

            if (this.gameObject == GameOrderSystem.instance.selectedUnit)
            {
                Ingame_UIManager.instance.unitInfoPanel.SetActive(false);
            }

            OnUnitDead?.Invoke(this);

            //GridManager.inst.SetTilePlaceable(this.transform.position, true, true); // 적 유닛 죽은 타일 배치상

            Debug.Log(this.gameObject.name + " Destroyed");
            return;
        }
        else if (HP <= 0 && isDead)
        {
            //GridManager.inst.SetTilePlaceable(this.transform.position, true, true); // 아군 유닛 죽은 타일 배치상태 최신화(배치 가능)

        }
    }

    //물리적 데미지용 함수.(이 이외에는 절대 사용하지 말 것.)
    public void ReceivePhysicalDamage(float Damage = 1, float Crit = 0, AttackType attackType = AttackType.UnKnown, UnitDebuff Crit2Debuff = UnitDebuff.None, GameObject SpecialAttackVFX = null)
    {
        #region 데미지 계산
        if (unitData.defenseType == DefenseType.완충갑)
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
        else if (unitData.defenseType == DefenseType.방탄갑)
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
        else if (unitData.defenseType == DefenseType.철갑)
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
        #endregion

        HP -= Damage;

        if (HP <= 0 && !isDead)//사망판정
        {
            HP = 0;

            soundManager.PlaySFX(soundManager.DEAD_SFX, soundManager.DeadSound[Random.Range(0, soundManager.DeadSound.Length)]);

            if (gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                isDead = true;
                Ingame_ParticleManager.Instance.EnemyDeathEffect(this.transform);
                WaveManager.inst.MonsterDead(this.gameObject, 2.0f);
                InGameManager.inst.gold += enmeyRewardGold;
                Ingame_UIManager.instance.goldTxt.text = InGameManager.inst.gold.ToString();
            }
            else if (gameObject.CompareTag(CONSTANT.TAG_UNIT))
            {
                isDead = true;
                Ingame_UIManager.instance.DestroyUnitStateChangeBox();
                Ingame_UIManager.instance.DestroyUnitUpgradeMenu();
                Ingame_UIManager.instance.DestorypgradeMenuConfirmBox();
            }

            if(this.gameObject == GameOrderSystem.instance.selectedUnit)
            {
                Ingame_UIManager.instance.unitInfoPanel.SetActive(false);
            }

            OnUnitDead?.Invoke(this);

            //GridManager.inst.SetTilePlaceable(this.transform.position, true, true); // 적 유닛 죽은 타일 배치상

            Debug.Log(this.gameObject.name + " Destroyed");
            return;
        }
        else if (HP <= 0 && isDead)
        {
           // GridManager.inst.SetTilePlaceable(this.transform.position, true, true); // 아군 유닛 죽은 타일 배치상태 최신화(배치 가능)

        }

        if (Random.Range(1f, 101f) <= Crit)//치명타 적용시
        {
            Debug.Log("Critical Hit!");
            debuffManager.AddDebuff(Crit2Debuff);
            for (int idx = 0; idx < soundManager.HitSound.Length; idx++)
            {
                if (attackType == soundManager.HitSound[idx].type)
                {
                    soundManager.PlaySFX(soundManager.HIT_SFX, soundManager.HitSound[idx].hitSoundCrit);
                    break;
                }
            }
            if (SpecialAttackVFX != null)
            {
                Ingame_ParticleManager.Instance.PlayEnemyAttackedParticleEffect(transform, SpecialAttackVFX);
            }
            else
            {
                Ingame_ParticleManager.Instance.PlayAttackedParticleEffect(transform, attackType, true);
            }
        }
        else //치명타 비적용시
        {
            for (int idx = 0; idx < soundManager.HitSound.Length; idx++)
            {
                if (attackType == soundManager.HitSound[idx].type)
                {
                    int HitSoundRandomNum = Random.Range(0, 2);
                    soundManager.PlaySFX(soundManager.HIT_SFX, soundManager.HitSound[idx].hitSound[HitSoundRandomNum]);
                    break;
                }
            }
            if (SpecialAttackVFX != null)
            {
                Ingame_ParticleManager.Instance.PlayEnemyAttackedParticleEffect(transform, SpecialAttackVFX);
            }
            else
            {
                Ingame_ParticleManager.Instance.PlayAttackedParticleEffect(transform, attackType, false);
            }
        }
    }



    // 스케일 애니메이션 코루틴 : 유닛 생성시 모델의 크기가 70% -> 100% 되게 연출 0.5초간
    private IEnumerator ScaleAnimation(Transform modelTransform)
    {
        if (modelTransform == null)
        {
            yield break;
        }

        Vector3 originalScale = modelTransform.localScale;

        // 초기 스케일을 70%로 축소
        modelTransform.localScale = originalScale * 0.7f;

        float duration = 0.5f; // 애니메이션 지속 시간
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (modelTransform == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            modelTransform.localScale = Vector3.Lerp(modelTransform.localScale, originalScale, t);

            yield return null;
        }

        // 최종 스케일 설정
        modelTransform.localScale = originalScale;

        // 위치 재조정 (필요 시)
        Vector3 pos = modelTransform.position;
        modelTransform.position = pos;

    }



    // 유닛 데이터 업데이트
    public void SetUnitData(Ingame_UnitData newData)
    {
        if (newData != null)
        {
            unitData = newData;
            ApplyUnitStats();
        }
        else
        {
            Debug.LogWarning("새 유닛 데이터가 없습니다!");
        }
    }

    // 유닛 상태 초기화
    private void ApplyUnitStats()
    {
        if (unitData != null)
        {
            maxHp = unitData.maxHP;
            HP = unitData.maxHP;
        }
        else
        {
            Debug.LogError("currentUnitData가 null입니다. 유닛 데이터가 없습니다.");
        }
    }
}