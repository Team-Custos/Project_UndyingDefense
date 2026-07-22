using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class AllyUnit : Unit
{
    public enum Mode
    {
        SEIGE,
        FREE
    }

    public enum FreeModeState
    {
        MOVECOMMAND,
        COMBAT
    }

    public enum TargetingType    //아군 유닛의 타겟 선정 방식.
    {
        NEAR,           // 가까운 적
        LOWHP,          // HP가 낮은 적
        HIGHTIER        // 티어가 높은 적
    }

    private enum State
    {
        IDLE,
        RUN,
        GENERALSKILL,
        SPECIALSKILL,
        STUN,
        DEAD,
    }

    private enum IdleState
    {
        DEFAULT,    // 기본 대기 모드
        COMBAT      // 전투 대기 모드
    }

    private AllyUnitData data;
    private ObjectPoolWithList<AllyUnit> pool;
    private WaveManager waveManager;

    private Mode mode;

    private bool hasExecutionTarget;   // 척살 명령 대상 존재 여부
    private bool isChange = false;
    private bool isUpgrade = false;
    private int upgradeIndex = -1;
    public Mode ModeType => mode;

    private State state;
    private IdleState idleState;
    private FreeModeState freeModeState;

    public bool IsChange => isChange;
    public bool IsUpgrade => isUpgrade;

    public override UnitData Data => data;

    private AllyUnitSpawner spawner;

    private float changeDuration = 3.0f;        // 모드 변경 시간
    private float upgradeDuraiton = 3.0f;


    [SerializeField] private NavMeshObstacle navObstacle;
    [SerializeField] private GameObject changeEffet;
    [SerializeField] private GameObject defaultSiegeEffect;
    [SerializeField] private GameObject activeSiegeEffect;
    [SerializeField] private UnitGrid unitGrid;
    [SerializeField] private AudioClip[] allyDeadSFX;

    public UnitGrid UnitGrid => unitGrid;

    private Vector3 commandDestination;  // 이동명령 목적지

    [SerializeField] private float siegeEffectInterval = 0.3f;

    private bool isSiegeActivated = true;
    private bool isAvailableToSiege = false; // 시즈 모드 가능한지 확인



    //protected static AudioClip[] AllyDeadSFX
    //{
    //    get
    //    {
    //        if (allyDeadSFX == null)
    //        {
    //            allyDeadSFX = Resources.LoadAll<AudioClip>("Sound/SFX/효과음/캐릭터/DeathSFX/AllyDeath");
    //        }
    //        return allyDeadSFX;
    //    }
    //}

    public override void Initialize() // 유닛 소환시
    {
        base.Initialize();
        isDead = false;
        collider.enabled = true;
        state = State.IDLE;

        SetIdleState(waveManager.IsWaveEnd);


        navObstacle.enabled = true;
        navAgent.enabled = false;

        mode = Mode.SEIGE;
        isSiegeActivated = true;
        siegeEffectInterval = 0.3f;

        navAgent.avoidancePriority = 2;

        navObstacle.carvingMoveThreshold = moveThresholdOnStop;


        //mode = Mode.FREE;
    }

    //public void UpgradeInitialize()
    //{
    //    base.Initialize();
    //    mode = Mode.UPGRADE;
    //    isDead = false;
    //}

    public void Initialize(AllyUnitData data, ObjectPoolWithList<AllyUnit> pool, AllyUnitSpawner spawner, WaveManager waveManager)
    {
        this.data = data;
        this.pool = pool;
        this.spawner = spawner;
        this.waveManager = waveManager;
    }


    protected override void Update()
    {
        if (isStop)
            return;

        //if (navObstacle.enabled)
        //    navObstacle.transform.rotation = Quaternion.identity;


        interval -= Time.deltaTime;

        if (isDeferredState)
        {
            deferredStateDurationCheck -= Time.deltaTime;
            if(targetUnit != null && targetUnit != this)
                Rotation(targetUnit.transform);

            if (deferredStateDurationCheck <= 0f)
            {
                targetUnit = null;
                isDeferredState = false;
                deferredStateDurationCheck = deferredStateDuration;
                deferredStateObj.SetActive(false);
            }
        }

        if (isChange)
        {
            ChangeMode();
            return;
        }


        if (isUpgrade)
        {
            UpgradeUnit();
            return;
        }

        switch (state)
        {
            case State.STUN:
                break;
            case State.GENERALSKILL:
            case State.SPECIALSKILL:
            case State.DEAD:
                {

                    if (state != State.DEAD)
                    {
                        if (navAgent.enabled)
                        {
                            modelAnimator.SetBool("isRunning", false);
                        }
                    }


                    if (state == State.SPECIALSKILL)
                    {
                        if (targetUnit != null && targetUnit != this)
                            LookAt(targetUnit.transform.position);
                        SkillBase skill = GetSpecialSkill();
                        if (skill != null)
                        {
                            if (stateDurationCheck >= skill.AnimationStateTime && isSkillActive)
                            {
                                base.ActivateSkill(skill, targetUnit);

                                SkillBase.TargetType skillTargetType = skill.GetTargetType();
                                if(skillTargetType ==  SkillBase.TargetType.ALLY ||
                                    skillTargetType == SkillBase.TargetType.SELF)
                                {
                                    targetUnit = null;
                                }
                                    
                            }
                        }
                    }

                    if (state == State.GENERALSKILL)
                    {
                        if (targetUnit != null)
                            LookAt(targetUnit.transform.position);
                        SkillBase skill = GetGeneralSkill();

                        if (skill != null)
                        {
                            if (stateDurationCheck >= skill.AnimationStateTime && isSkillActive)
                            {
                                
                                base.ActivateSkill(skill, targetUnit);
                            }
                        }
                    }



                    if (stateDuration <= 0f)
                        return;

                    if (stateDurationCheck < stateDuration)
                    {
                        stateDurationCheck += Time.deltaTime;
                    }
                    else
                    {
                        if (state == State.DEAD)
                        {
                            gameObject.SetActive(false);
                            pool.Pool.Release(this);
                        }

                        stateDurationCheck = 0f;
                        stateDuration = 0f;

                        state = State.IDLE;

                        //if(mode == Mode.FREE)
                        //    navObstacle.enabled = false;
                    }
                }
                break;
            case State.IDLE:
                {
                    //if(mode == Mode.FREE)
                    //    navAgent.isStopped = true;


                    if (navAgent.enabled && navAgent.velocity.magnitude > 0f) // 이동 중일 때
                    {
                        navAgent.isStopped = false;
                        state = State.RUN;
                        modelAnimator.SetBool("isRunning", true);
                    }
                    else
                    {
                        if (idleState == IdleState.DEFAULT)
                        {
                            Vector3 direction = Vector3.left;
                            Quaternion rot = Quaternion.LookRotation(direction);
                            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10.0f);
                            modelAnimator.SetBool("isRunning", false);
                        }
                    }

                    UpdateMode();
                }
                break;
            case State.RUN:
                {
                    navAgent.isStopped = false;

                    if (!navAgent.enabled || navAgent.velocity.magnitude <= 0f)
                    {
                        state = State.IDLE;
                        modelAnimator.SetBool("isRunning", false);
                    }

                    UpdateMode();
                }
                break;
        }
    }


    private void UpdateMode()
    {
        //if (isDeferredState)
        //    return;

        // 척살 명령 상태인지 확인 -> 대상 강제 지정 -> 척살 명령 상태 해제
        if (hasExecutionTarget)
        {
            targetUnit = executionUnit;
            hasExecutionTarget = false;
            executionUnit = null;
        }

        switch (mode)
        {
            case Mode.SEIGE:
                {
                    if (!navObstacle.enabled)
                    {
                        Debug.Log("obstacle 꺼져있음");
                    }

                    if (isSiegeActivated)
                    {
                        siegeEffectInterval -= Time.deltaTime;

                        if (siegeEffectInterval > 0)
                        {
                            activeSiegeEffect.SetActive(true);
                        }
                        else
                        {
                            activeSiegeEffect.SetActive(false);
                            defaultSiegeEffect.SetActive(true);
                            siegeEffectInterval = 0.3f;
                            isSiegeActivated = false;
                        }
                    }

                    if (idleState == IdleState.DEFAULT || isDeferredState)
                    {
                        break;
                    }
                        

                    if (interval <= 0f && currentSkill == null)     // 인터벌 중이 아니고, 보유 스킬이 없는 경우 스킬 선택
                    {
                        currentSkill = GetAvailableSkill();
                    }

                    if (currentSkill != null) // 사용 가능한 스킬이 존재할 경우
                    {
                        SkillBase.TargetType skillTargetType = currentSkill.GetTargetType(); // 스킬 대상 종류 확인

                        switch (skillTargetType)
                        {
                            case SkillBase.TargetType.NONE: // 바로 발동
                                {
                                    UpdateSkillState(currentSkill, null);
                                    break;
                                }
                            case SkillBase.TargetType.SELF:
                                {
                                    targetUnit = this;
                                    UpdateSkillState(currentSkill, this);
                                    break;
                                }
                            case SkillBase.TargetType.ALLY:
                                {
                                    if (targetUnit is EnemyUnit)
                                        targetUnit = null;  // 공격 스킬 대상 초기화

                                    targetUnit = base.SearchTarget(currentSkill.Data.Range, allyLayer, currentSkill);
                                    if (targetUnit != null)
                                    {
                                        UpdateSkillState(currentSkill, targetUnit);
                                        //targetUnit = null;
                                    }
                                    else
                                    {
                                        targetUnit = base.SearchTarget(unitStats.sightRange, allyLayer, currentSkill);
                                        if (targetUnit != null)  // 시야 범위내 대상 발견
                                        {
                                            if (!IsTargetInRange(targetUnit, currentSkill.Data.Range))
                                            {
                                                //Rotation(targetUnit.transform);
                                                SetDeferredState();
                                            }
                                            else
                                            {
                                                UpdateSkillState(currentSkill, targetUnit);
                                                //targetUnit = null;
                                            }
                                        }
                                        else
                                        {
                                            currentSkill = null;
                                        }
                                    }
                                    break;
                                }
                            case SkillBase.TargetType.ENEMY:
                                {
                                    if (IsTargetValid(targetUnit, enemyLayer)) // 살아는 있음
                                    {
                                        if (!IsTargetInRange(targetUnit, currentSkill.Data.Range))  // 범위내에 없음
                                        {
                                            //Rotation(targetUnit.transform);
                                            SetDeferredState();
                                        }
                                        else
                                        {
                                            UpdateSkillState(currentSkill, targetUnit);
                                        }
                                    }
                                    else
                                    {
                                        targetUnit = SearchTarget(currentSkill.Data.Range, enemyLayer, currentSkill);
                                        if(targetUnit != null)
                                        {
                                            UpdateSkillState(currentSkill, targetUnit);
                                        }
                                        else
                                        {
                                            targetUnit = SearchTarget(unitStats.sightRange, enemyLayer, currentSkill);
                                            if(targetUnit != null)
                                            {
                                                if (!IsTargetInRange(targetUnit, currentSkill.Data.Range))  // 범위내에 없음
                                                {
                                                    //Rotation(targetUnit.transform);
                                                    SetDeferredState();
                                                }
                                                else
                                                {
                                                    UpdateSkillState(currentSkill, targetUnit);
                                                }
                                            }
                                            else
                                            {
                                                currentSkill = null;
                                            }
                                        }
                                    }

                                    break;
                                }

                        }

                    }
                    //else
                    //{
                    //    if (!IsTargetValid(targetUnit, enemyLayer))
                    //    {
                    //        targetUnit = null;
                    //    }
                    //}
                    break;


                }
            case Mode.FREE:
                {
                    if (!navAgent.enabled)
                        break;

                    switch (freeModeState)
                    {
                        case FreeModeState.MOVECOMMAND:
                            {
                                MoveTo(commandDestination);

                                if (navAgent.remainingDistance < navAgent.stoppingDistance)
                                {
                                    freeModeState = FreeModeState.COMBAT;
                                }

                                break;
                            }
                        case FreeModeState.COMBAT:
                            {
                                if (idleState == IdleState.DEFAULT)
                                {
                                    targetUnit = null;
                                    navAgent.isStopped = true;
                                    break;
                                }

                                if (isDeferredState)
                                    break;

                                if (interval <= 0f && currentSkill == null)     // 인터벌 중이 아니고, 보유 스킬이 없는 경우 스킬 선택
                                {
                                    currentSkill = GetAvailableSkill();
                                }


                                if (currentSkill != null) // 사용 가능한 스킬이 존재할 경우
                                {
                                    SkillBase.TargetType skillTargetType = currentSkill.GetTargetType(); // 스킬 대상 종류 확인

                                    switch (skillTargetType)
                                    {
                                        case SkillBase.TargetType.NONE:
                                            {
                                                UpdateSkillState(currentSkill, null);
                                                break;
                                            }
                                        case SkillBase.TargetType.SELF:
                                            {
                                                targetUnit = this;
                                                UpdateSkillState(currentSkill, this);
                                                break;
                                            }
                                        case SkillBase.TargetType.ALLY:
                                            {
                                                if(targetUnit is EnemyUnit)
                                                    targetUnit = null;  // 공격 스킬 대상 초기화

                                                if (IsTargetValid(targetUnit, allyLayer)) // 기존 대상 유효
                                                {
                                                    if(IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
                                                    {
                                                        UpdateSkillState(currentSkill, targetUnit);
                                                        //targetUnit = null;
                                                    }
                                                    else
                                                    {
                                                        if(!IsPathBlocked(targetUnit))
                                                        {
                                                            MoveToTargetUnit(targetUnit);
                                                        }
                                                        else
                                                        {
                                                            SetDeferredState();
                                                        }
                                                    }
                                                }
                                                else    // 처음 탐색
                                                {
                                                    SearchReachableTargets(unitStats.sightRange, allyLayer); // 이동 가능한 대상 탐색
                                                    targetUnit = SearchTargetInTargets(currentSkill);

                                                    if (targetUnit != null)
                                                    {
                                                        if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range) || targetUnit == this)
                                                        {
                                                            UpdateSkillState(currentSkill, targetUnit);
                                                            //targetUnit = null;
                                                        }
                                                        else
                                                        {
                                                            MoveToTargetUnit(targetUnit);
                                                        }
                                                    }

                                                }

                                                break;
                                            }
                                        case SkillBase.TargetType.ENEMY:
                                            {
                                                // 기존 대상 유효성 확인
                                                if (IsTargetValid(targetUnit, enemyLayer)) 
                                                {
                                                    if(!IsTargetInRange(targetUnit, unitStats.sightRange))
                                                    {
                                                        // 범위 밖에 있음 -> 판단 유예 상태로
                                                        SetDeferredState();
                                                        return;
                                                    }

                                                    if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range)) // 스킬 사거리내 존재
                                                    {
                                                        UpdateSkillState(currentSkill, targetUnit);
                                                    }
                                                    else // 스킬 사거리 < 대상과 거리 < 시야 사거리
                                                    {
                                                        if (IsPathBlocked(targetUnit))   // 이동 가능 여부 확인
                                                        {
                                                            SetDeferredState();
                                                        }
                                                        else
                                                        {
                                                            MoveToTargetUnit(targetUnit);
                                                        }
                                                    }
                                                }
                                                else      // 새 대상 탐색
                                                {
                                                    targetUnit = SearchExecutionTarget(unitStats.sightRange);
                                                    if (targetUnit != null && !IsPathBlocked(targetUnit))
                                                        return;

                                                    SearchReachableTargets(unitStats.sightRange, enemyLayer); // 이동 가능한 대상 탐색
                                                    targetUnit = SearchTargetInTargets(currentSkill);
                                                    if(targetUnit != null)
                                                    {
                                                        if(IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
                                                        {
                                                            UpdateSkillState(currentSkill, targetUnit);
                                                        }
                                                        else
                                                        {
                                                            MoveToTargetUnit(targetUnit);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        currentSkill = null;
                                                    }

                                                }

                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    navAgent.isStopped = true;
                                }

                                break;


                            }
                    }

                    break;
                }
        }

    }

   public void SetIdleState(bool isWaveEnd)
    {
        if(isWaveEnd)
        {
            targetUnit = null;
            idleState = IdleState.DEFAULT;
        }
        else
            idleState = IdleState.COMBAT;
    }

    public void ChangeMode()
    {
        state = State.IDLE;
        modelAnimator.SetBool("isRunning", false);

        changeDuration -= Time.deltaTime;

        switch (mode)
        {
            case Mode.SEIGE:     // 프리로 변경
                {
                    if (changeDuration > 0)      // 변경 중
                    {
                        if (!changeEffet.activeSelf)
                            changeEffet.SetActive(true);

                        if (selectedUnitUI != null)
                        {
                            selectedUnitUI.ShowUnitDurtion(1 - (changeDuration / 3.0f));
                        }

                        if (defaultSiegeEffect.activeSelf)
                            defaultSiegeEffect.SetActive(false);
                    }
                    else    // 변경 끝
                    {
                        unitGrid.ClearTile();
                        navObstacle.enabled = false;

                        NavMesh.CalculatePath(transform.position, transform.position, navAgent.areaMask, path);
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            if (selectedUnitUI != null)
                            {
                                selectedUnitUI.ShowAllyUI(this);
                                selectedUnitUI.HideUnitDuration();
                            }

                            mode = Mode.FREE;
                            freeModeState = FreeModeState.COMBAT;

                            navAgent.enabled = true;

                            changeEffet.SetActive(false);


                            changeDuration = 3f;
                            isChange = false;
                        }
                    }
                }
                break;

            case Mode.FREE:    // 시즈로 변경
                {
                    if (!isAvailableToSiege)
                    {
                        Tile targetTile = unitGrid.GetAvailableTile();

                        if (targetTile != null)
                        {
                            transform.position = targetTile.transform.position;

                            navAgent.enabled = false;

                            navObstacle.transform.rotation = Quaternion.Euler(0, 90, 0);
                            navObstacle.enabled = true;

                            // carve 회전 조정
                            //Vector3 direction = Vector3.left;
                            //Quaternion rot = Quaternion.LookRotation(direction);

                            //navObstacle.transform.rotation = Quaternion.Euler(0, 90, 0);

                        }
                        else // 주변 이동 가능 타일이 없으면 변경 취소
                        {
                            mode = Mode.FREE;
                            isChange = false;
                            break;
                        }
                        isAvailableToSiege = true;
                    }

                    if (isAvailableToSiege)
                    {
                        if (changeDuration > 0)
                        {
                            if (!changeEffet.activeSelf)
                                changeEffet.SetActive(true);

                            if (selectedUnitUI != null)
                            {
                                selectedUnitUI.ShowUnitDurtion(1 - (changeDuration / 3.0f));
                            }
                        }
                        else
                        {
                            if (selectedUnitUI != null)
                            {
                                selectedUnitUI.ShowAllyUI(this);
                                selectedUnitUI.HideUnitDuration();
                            }

                            changeEffet.SetActive(false);


                            isAvailableToSiege = false;
                            mode = Mode.SEIGE;
                            isSiegeActivated = true;

                            changeDuration = 3f;
                            isChange = false;

                        }
                    }
                }
                break;
        }
    }

    public void ChangeOrder()
    {
        isChange = true;

        if (selectedUnitUI != null)
            selectedUnitUI.HideAllyUI();
    }

    private void UpgradeUnit()
    {
        if (isDead) return;

        state = State.IDLE;
        modelAnimator.SetBool("isRunning", false);

        upgradeDuraiton -= Time.deltaTime;

        if (upgradeDuraiton > 0)
        {
            if (navAgent.enabled)
                navAgent.isStopped = true;

            if (!changeEffet.activeSelf)
                changeEffet.SetActive(true);

            defaultSiegeEffect.SetActive(false);

            if (selectedUnitUI != null)
            {
                selectedUnitUI.ShowUnitDurtion(1 - (upgradeDuraiton / 3.0f));
            }
        }
        else
        {
            UnitData upgradeUnitData = data.UpgradeUnits[upgradeIndex];

            GameObject obj = upgradeUnitData.Prefab;

            AllyUnit upgradedUnit = spawner.CreateUpgradeUnit(obj, (AllyUnitData)upgradeUnitData, transform, unitGrid.TargetTile);
            upgradedUnit.mode = mode;

            if (mode == Mode.SEIGE)
            {
                upgradedUnit.navAgent.enabled = false;

                upgradedUnit.navObstacle.transform.rotation = Quaternion.Euler(0, 90, 0);
                upgradedUnit.navObstacle.enabled = true;
            }
            else
            {
                upgradedUnit.navAgent.enabled = true;
                upgradedUnit.navObstacle.enabled = false;
                upgradedUnit.freeModeState = FreeModeState.COMBAT;
            }


            RemoveAllEffect();

            if (isSelected)
            {
                isSelected = false;
                upgradedUnit.isSelected = true;

                selectedUnitUI.UpdateUnitInfo(upgradedUnit);


                upgradedUnit.SetSelectedUnitManager(selectedUnitManager);
                upgradedUnit.selectedUnitManager.SetSelectedUnit(upgradedUnit);

                selectedUnitUI.ShowAllyUI(upgradedUnit);
                selectedUnitUI.ShowHp(upgradedUnit);
            }

            upgradeIndex = -1;
            upgradeDuraiton = 3f;
            isUpgrade = false;

            pool.Pool.Release(this);
            gameObject.SetActive(false);
        }
    }


    public void UpgradeOrder(int index)
    {
        isUpgrade = true;
        upgradeIndex = index;

        if (selectedUnitUI != null)
            selectedUnitUI.HideAllyUI();
    }

    public void UpdateCommandDestination(Vector3 pos)
    {

        freeModeState = FreeModeState.MOVECOMMAND;
        commandDestination = pos;
    }


    public void SetExecutionUnit(Unit target)
    {
        executionUnit = target;
        hasExecutionTarget = true;
    }

    

    public override void GetStun()
    {
        if (state == State.RUN)
        {
            modelAnimator.SetBool("isRunning", false);
        }
        if (navAgent.enabled)
        {
            navAgent.isStopped = true;
        }
        base.GetStun();
        state = State.STUN;
    }

    public override void RemoveStun()
    {
        base.RemoveStun();

        if (!isDead)
        {
            //if (navAgent.enabled)
            //    navAgent.isStopped = false;
            state = State.IDLE;
        }

        //if (navAgent.enabled)
        //    navAgent.isStopped = false;

    }

    // 척살 명령 대상 우선 탐색 후 기존 탐색
    public override Unit SearchTarget(float range, LayerMask targetLayer, SkillBase skill)
    {
        Unit unit = null;
        unit = SearchExecutionTarget(range);
        if(unit != null)
        {
            return unit;
        }
        else
        {
            unit = base.SearchTarget(range, targetLayer, skill);
            return unit;
        }
    }


    //private Unit SearchReachableTarget(float range)
    //{
    //    Unit result = null;
    //    switch (data.TargetingType)
    //    {
    //        case TargetingType.NEAR:
    //            result = SearchNearestReachableTarget(range);
    //            break;
    //        case TargetingType.LOWHP:
    //            result = SearchLowHPReachableTarget(range);
    //            break;
    //        case TargetingType.HIGHTIER:
    //            result = SearchHighTierReachableTarget(range);
    //            break;
    //    }

    //    return result;
    //}

    private EnemyUnit SearchExecutionTarget(float range)    // 척살 명령 지정된 적 탐색
    {
        EnemyUnit result = null;
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if (targetCount > 0)
        {
            for (int i = 0; i < targetCount; i++)
            {
                EnemyUnit unit = collidersInRange[i].GetComponent<EnemyUnit>();

                if (unit.HasExecutionMark)
                {
                    if (unit.IsDead || !unit.gameObject.activeInHierarchy)
                        continue;
                    else
                        result = unit;
                }
            }
        }
        return result;
    }

    

    private void UpdateSkillState(SkillBase skill, Unit target)
    {
        if (isDead) return;

        if (skill == GeneralSkill)
        {
            state = State.GENERALSKILL;

            PlayAnimation("GeneralSkill");
            //modelAnimator.SetTrigger("GeneralSkill");
        }
        else if (skill == SpecialSkill)
        {
            state = State.SPECIALSKILL;

            PlayAnimation("SpecialSkill");
            //modelAnimator.SetTrigger("SpecialSkill");
        }

        if (target != null && target != this)
            transform.LookAt(target.transform);

        if(navAgent.enabled)
            navAgent.isStopped = true;
        modelAnimator.SetBool("isRunning", false);

        //AttackSkillData atData = skill.Data as AttackSkillData;

        //Debug.Log($"{currentSkill.Data.Name} : {atData.Damage}");

        //float dist = Vector3.Distance(transform.position, target.transform.position);
        //Debug.Log("실제 거리 : " + dist);
        //Debug.Log("스킬 사용 거리 : " + CurrentSKill.Data.Range);

        isSkillActive = true;
        interval = intervalCheck;
        currentSkill = null;

        

        if (stateDurationCheck >= stateDuration)
        {
            stateDurationCheck = 0f;
            stateDuration = 0f;
        }

    }



    public override void Die()
    {
        if (isDead) return;

        isDead = true;


        if (state == State.STUN)
        {
            base.RemoveStun();
        }

        state = State.DEAD;



        navObstacle.enabled = false;

        // 체인지 또는 업그레이드 중 사망 시
        if (isChange)
        {
            changeDuration = 3.0f;
            changeEffet.SetActive(false);
            isChange = false;
        }

        if (isUpgrade)
        {
            upgradeDuraiton = 3.0f;
            changeEffet.SetActive(false);
            isUpgrade = false;
        }

        hasExecutionTarget = false;
        base.Die();

        AddVFX(UnitDeathVFX.GetComponent<ParticleSystem>());

        unitGrid.ClearTile();

        SoundManager.Instance.PlaySFX(this.transform.position, allyDeadSFX);
    }


    public override void SetDeferredState()
    {
        base.SetDeferredState();
        state = State.IDLE;
    }

}
