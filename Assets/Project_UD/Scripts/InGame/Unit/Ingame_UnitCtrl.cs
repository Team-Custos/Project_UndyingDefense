using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

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

    public Ingame_UnitData unitData;
    UnitDebuffManager debuffManager;
    
    UnitModelSwapManager ModelSwapManager;
    public UnitSoundManager soundManager;
    

    
    [Header("====Status====")]
    public AllyMode Ally_Mode;

    public Vector2 unitPos = Vector2.zero;

    public Transform VisualModel;
    //public UnitAnimationParaCtrl AnimationCtrl;
    public GameObject Selected_Particle;
    public bool isSelected = false;
    public int cur_modelType;
    public int HP;
    public float cur_moveSpeed = 1;
    public float cur_attackSpeed = 1;
    public bool unActable = false;

    public int maxHp;
    public Image hpBar;

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

    public float unitStateChangeTime;
    public AllyMode previousAllyMode;

    //public string unitCode;
    //public int level;
    //public int cost;
    //public string name;
    //public string gSkillName;
    //public string sSkillName;
    //public string gSkillInfo;
    //public string sSkillInfo;

    // 수정 예정
    public string defenstype;



    void OnMouseDown()
    {
        if (Ingame_UIManager.instance != null)
        {
            //Ingame_UIManager.instance.UpdateUnitInfoPanel(this.unitName);
        }
    }

    private void Awake()
    {

        ModelSwapManager = UnitModelSwapManager.inst;
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

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
        {
            Instantiate(ModelSwapManager.AllyModel[unitData.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
        }
        else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            Instantiate(ModelSwapManager.EnemyModel[unitData.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
            
        }

        SpawnDelay = true;

        targetBase = InGameManager.inst.Base;
        maxHp = unitData.maxHP;

        //if()//삼계탕 특식 효과
        {
            maxHp = unitData.maxHP + 2;
        }

        HP = maxHp;
        cur_moveSpeed = unitData.moveSpeed;
        

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



        //UpdateUnitDataFromExcel(unitData);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(findEnemyRange.transform.position, unitData.attackRange + 0.5f);
    }

    public void ModelSwap()
    {
        if (unitData.modelType != cur_modelType)
        {
            Destroy(VisualModel.GetChild(0).gameObject);
            if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
            {
                Instantiate(ModelSwapManager.AllyModel[unitData.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
            }
            else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                Instantiate(ModelSwapManager.EnemyModel[unitData.modelType], VisualModel.transform.position + Vector3.down, this.transform.rotation, VisualModel);
            }

            cur_modelType = unitData.modelType;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)HP / (float)maxHp;
        }

        //유닛의 현재 위치에 따른 타일 배치 가능 설정
        GridManager.inst.SetTilePlaceable(this.transform.position, false, false);

        //유닛의 현재 위치에 따른 타일 위치 가져오기
        unitPos = GridManager.inst.GetTilePos(this.transform.position);

        //모델 변경
        ModelSwap();

        //Selected_Particle.SetActive(isSelected);
        //findEnemyRange.SetActive(isSelected);
        moveTargetBasePos = new Vector3(targetBase.transform.position.x, this.transform.position.y, this.transform.position.z);

        NavAgent.speed = unitData.moveSpeed;

        if (HP <= 0)
        {
            HP = 0;

            
            EnemySpawner.inst.OnMonsterDead(this.gameObject);

            Destroy(this.gameObject);
            Debug.Log(this.gameObject.name + " Destroyed");

        }

        if (Input.GetKeyDown(KeyCode.H) && isSelected)
        {
            Destroy(this.gameObject);
        }


        #region 아군 제어
        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
        {
            sightRangeSensor.radius = unitData.sightRange;
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

            if (Input.GetKeyDown(KeyCode.B) && isSelected)
            {
                debuffManager.AddDebuff(UnitDebuff.Dizzy);
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

                Ingame_UIManager.instance.ShowUnitStateUI(this.gameObject, false, false);


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
                Ingame_UIManager.instance.ShowUnitStateUI(this.gameObject, false, true);

                UnitSkill.UnitSpecialSkill(unitData.specialSkillCode, unitData.skillCooldown);
                if (targetEnemy == null && !isEnemyInSight)
                {
                    SearchEnemy();
                }

                if (targetEnemy != null && !haveToMovePosition)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
                    isEnemyInRange = (distanceToEnemy <= unitData.attackRange);

                    if (isEnemyInRange && Ally_State != null && Ally_State.fsm != null)
                    {
                        Ally_State.fsm.ChangeState(UnitState.Attack);
                    }
                }
            }
            //프리모드일때
            else if (Ally_Mode == AllyMode.Free)
            {

                UnitSkill.UnitSpecialSkill(unitData.specialSkillCode, unitData.skillCooldown);
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
                        isEnemyInRange = (Vector3.Distance(transform.position, targetEnemy.transform.position) <= unitData.attackRange);
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
            (Vector3.Distance(transform.position, moveTargetBasePos) <= unitData.attackRange);

            if (unActable)
            {
                return;
            }

            if (enemy_isPathBlocked)
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
                sightRangeSensor.NearestObjectSearch(unitData.sightRange, this.gameObject.CompareTag(CONSTANT.TAG_ENEMY));

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
        if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT)) //아군일때
        {
            if (targetEnemy == null)
            {
                Ally_State.fsm.ChangeState(UnitState.Search);
                return;
            }
            else if(isEnemyInRange)
            {
                VisualModel.transform.LookAt(targetEnemy.transform.position);
                UnitSkill.UnitGeneralSkill(unitData.generalSkillCode, targetEnemy, unitData.weaponCooldown, false);
            }
        }
        else if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY)) //적일때
        {
            if (enemy_isBaseInRange)
            {
                UnitSkill.EnemyGeneralSkill(unitData.generalSkillCode, targetBase, unitData.weaponCooldown, true);
            }
            else
            {
                if (targetEnemy != null)
                {
                    Debug.Log("SkillCode : " + unitData.generalSkillCode);
                    Debug.Log("targetEnemy." + targetEnemy.name);
                    //Debug.Log("weaponCooldown : " + unitData.weaponCooldown);

                    UnitSkill.EnemyGeneralSkill(unitData.generalSkillCode, targetEnemy, unitData.weaponCooldown, true);
                }
                else if (targetEnemy == null)
                {
                    Debug.Log("경로 다시 검색...");
                    Enemy_State.fsm.ChangeState(EnemyState.Move);
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
    public void ReceivePhysicalDamage(int Damage = 1, float Crit = 0, AttackType attackType = AttackType.UnKnown, UnitDebuff Crit2Debuff = UnitDebuff.None)
    {
        if (unitData.defenseType == DefenseType.cloth)
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
        else if (unitData.defenseType == DefenseType.leather)
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
        else if (unitData.defenseType == DefenseType.metal)
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

        HP -= Damage * 100;

        if (Random.Range(1, 101) <= Crit)//치명타 적용시
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
            Ingame_ParticleManager.Instance.PlayAttackedParticleEffect(transform, attackType, true);
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
            Ingame_ParticleManager.Instance.PlayAttackedParticleEffect(transform, attackType, false);
        }
    }

    //public void UpdateUnitDataFromExcel(Ingame_UnitData unitData)
    //{
    //    UnitExcelData excelData = UnitExcelDataManager.inst.GetUnitData(unitData.unitCode);

    //    if (excelData != null)
    //    {
    //        unitData.name = excelData.name;
    //        unitData.level = excelData.level;
    //        unitData.g_SkillName = excelData.g_SkillName;
    //        unitData.s_SkillName = excelData.s_SkillName;
    //        unitData.g_SkillInfo = excelData.g_SkillInfo;
    //        unitData.s_SkillInfo = excelData.s_SkillInfo;
    //    }
    //}
}