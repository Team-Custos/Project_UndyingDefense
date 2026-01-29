using System.Collections.Generic;
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

    public void MoveCommandDestination(Vector3 pos)
    {
        freeModeState = FreeModeState.MOVECOMMAND;
        commandDestination = pos;
    }


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
                            if (stateDurationCheck >= skill.AnimationStateTime)
                            {
                                base.ActivateSkill(skill, targetUnit);
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
                            if (stateDurationCheck >= skill.AnimationStateTime)
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
                        if (targetUnit == null)
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

        switch (mode)
        {
            case Mode.SEIGE:
                {
                    if (!navObstacle.enabled)
                        Debug.Log("obstacle 꺼져있음");

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

                    if (idleState == IdleState.DEFAULT)
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
                            case SkillBase.TargetType.NONE: // 바로 발동
                                {
                                    ActivateSkill(currentSkill, null);
                                    break;
                                }
                            case SkillBase.TargetType.ALLY:
                                {
                                    if (targetUnit is EnemyUnit)
                                        targetUnit = null;  // 공격 스킬 대상 초기화

                                    targetUnit = SearchTarget(currentSkill.Data.Range, allyLayer, currentSkill);
                                    if (targetUnit != null)
                                    {
                                        ActivateSkill(currentSkill, targetUnit);
                                        targetUnit = null;
                                    }

                                    break;
                                }
                            case SkillBase.TargetType.ENEMY:
                                {
                                    if (IsTargetValid(targetUnit, currentSkill.Data.Range, enemyLayer))
                                    {
                                        ActivateSkill(currentSkill, targetUnit);
                                    }
                                    else
                                    {
                                        targetUnit = SearchTarget(currentSkill.Data.Range, enemyLayer, currentSkill);
                                        if (targetUnit != null)
                                        {
                                            ActivateSkill(currentSkill, targetUnit);
                                        }
                                    }

                                    break;
                                }

                    }


                    }
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
                                                ActivateSkill(currentSkill, null);
                                                break;
                                            }
                                        case SkillBase.TargetType.ALLY:
                                            {
                                                if(targetUnit is EnemyUnit)
                                                    targetUnit = null;  // 공격 스킬 대상 초기화

                                                if (targetUnit != null) // 탐색된 대상이 있음
                                                {
                                                    if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
                                                    {
                                                        ActivateSkill(currentSkill, targetUnit);
                                                        targetUnit = null;
                                                    }
                                                    else
                                                    {
                                                        if (IsPathBlocked(targetUnit))   // 추적 중 길 막혔는지 확인
                                                        {
                                                            targetUnit = null;  // null 처리 후 다시 탐색
                                                        }
                                                        else
                                                        {
                                                            MoveToTargetUnit(targetUnit);
                                                        }
                                                    }

                                                }
                                                else    // 처음 탐색
                                                {
                                                    targetUnit = SearchTarget(currentSkill.Data.Range, allyLayer, currentSkill);  // 스킬 사거리내로 먼저 검사
                                                   
                                                    if(targetUnit != null)
                                                    {
                                                        ActivateSkill(currentSkill, targetUnit);
                                                        targetUnit = null;
                                                    }
                                                    else    
                                                    {
                                                        SearchReachableTargets(unitStats.sightRange, allyLayer); //  시야 범위 내 이동 가능 유닛
                                                    
                                                        if(targets.Count > 0)
                                                        {
                                                            targetUnit = SearchTargetInTargets(currentSkill);  //대상 선택

                                                            if (targetUnit != null)
                                                            {
                                                                MoveToTargetUnit(targetUnit);
                                                            }
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                        case SkillBase.TargetType.ENEMY:
                                            {
                                                if (IsTargetValid(targetUnit, unitStats.sightRange, enemyLayer)) // 시야 사거리 내 유효
                                                {
                                                    if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range)) // 스킬 사거리내 존재
                                                    {
                                                        ActivateSkill(currentSkill, targetUnit);
                                                    }
                                                    else // 스킬 사거리 < 대상과 거리 < 시야 사거리
                                                    {
                                                        if (IsPathBlocked(targetUnit))   // 이동 가능 여부 확인
                                                        {
                                                            targetUnit = null;  // 막힘
                                                        }
                                                        else
                                                        {
                                                            MoveToTargetUnit(targetUnit);
                                                            //if (IsTargetInAttackRange(targetUnit, skill.Data.Range))
                                                            //{
                                                            //    ActivateSkill(skill, targetUnit);
                                                            //    targetUnit = null;
                                                            //}
                                                        }
                                                    }
                                                }
                                                else      // 새 대상 탐색
                                                {
                                                    targetUnit = null;

                                                    targetUnit = SearchTarget(currentSkill.Data.Range, enemyLayer, currentSkill);
                                                    if (targetUnit != null)
                                                    {
                                                        ActivateSkill(currentSkill, targetUnit);
                                                    }
                                                    else
                                                    {
                                                        SearchReachableTargets(unitStats.sightRange, enemyLayer); // 이동 가능한 대상 탐색
                                                        targetUnit = SearchTargetInTargets(currentSkill); // 시야 내로 다시 검사

                                                        if (targetUnit != null)
                                                        {
                                                            MoveToTargetUnit(targetUnit);
                                                        }
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


                                //if (interval <= 0f)
                                //{
                                //    SkillBase skill = GetAvailableSkill();

                                //    if (skill != null) // 사용 가능한 스킬이 존재할 경우
                                //    {
                                //        SkillBase.TargetType skillTargetType = skill.GetTargetType(); // 스킬 대상 종류 확인

                                //        switch (skillTargetType)
                                //        {
                                //            case SkillBase.TargetType.NONE:
                                //                {
                                //                    ActivateSkill(skill, null);
                                //                    break;
                                //                }
                                //            case SkillBase.TargetType.ALLY:     // 탐색 -> 스킬 발동 or 이동
                                //                {
                                //                    targetUnit = SearchTarget(skill.Data.Range, allyLayer, skill);  // 스킬 사거리내로 먼저 검사

                                //                    if(targetUnit != null)
                                //                    {
                                //                        ActivateSkill(skill, targetUnit);
                                //                        targetUnit = null;
                                //                    }
                                //                    else
                                //                    {
                                //                        SearchReachableTargets(unitStats.sightRange, allyLayer); //  시야 범위 내 이동 가능 유닛
                                //                        targetUnit = SearchTargetInTargets(skill); // 시야 내로 다시 검사

                                //                        if(targetUnit != null)
                                //                        {
                                //                            MoveToTargetUnit(targetUnit);
                                //                            if(IsTargetInAttackRange(targetUnit, skill.Data.Range))
                                //                            {
                                //                                ActivateSkill(skill, targetUnit);
                                //                                targetUnit = null;
                                //                            }
                                //                        }
                                //                    }

                                //                    break;
                                //                }
                                //            case SkillBase.TargetType.ENEMY:
                                //                {
                                //                    if(IsTargetValid(targetUnit, unitStats.sightRange, enemyLayer)) // 시야 사거리 내 유효
                                //                    {
                                //                        if(IsTargetInAttackRange(targetUnit, skill.Data.Range)) // 스킬 사거리내 존재
                                //                        {
                                //                            ActivateSkill(skill, targetUnit);
                                //                        }
                                //                        else // 스킬 사거리 < 대상과 거리 < 시야 사거리
                                //                        {
                                //                            if(IsPathBlocked(targetUnit))   // 이동 가능 여부 확인
                                //                            {
                                //                                targetUnit = null;  // 막힘
                                //                            }
                                //                            else
                                //                            {
                                //                                MoveToTargetUnit(targetUnit);
                                //                                //if (IsTargetInAttackRange(targetUnit, skill.Data.Range))
                                //                                //{
                                //                                //    ActivateSkill(skill, targetUnit);
                                //                                //    targetUnit = null;
                                //                                //}
                                //                            }
                                //                        }
                                //                    }
                                //                    else      // 새 대상 탐색
                                //                    {
                                //                        targetUnit = null;

                                //                        targetUnit = SearchTarget(skill.Data.Range, enemyLayer, skill);
                                //                        if(targetUnit != null)
                                //                        {
                                //                            ActivateSkill(skill, targetUnit);
                                //                        }
                                //                        else
                                //                        {
                                //                            SearchReachableTargets(unitStats.sightRange, enemyLayer); // 이동 가능한 대상 탐색
                                //                            targetUnit = SearchTargetInTargets(skill); // 시야 내로 다시 검사

                                //                            if (targetUnit != null)
                                //                            {
                                //                                MoveToTargetUnit(targetUnit);
                                //                            }
                                //                        }
                                //                    }

                                //                    break;
                                //                }
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    navAgent.isStopped = true;
                                //}

                                //break;
                            }
                    }

                    break;
                }
        }

    }

   public void SetIdleState(bool isWaveEnd)
    {
        if(isWaveEnd)
            idleState = IdleState.DEFAULT;
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
                            navObstacle.enabled = true;

                            // carve 회전 조정
                            Vector3 direction = Vector3.left;
                            Quaternion rot = Quaternion.LookRotation(direction);

                            navObstacle.transform.rotation = rot;

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
                upgradedUnit.navObstacle.enabled = true;
            }
            else
            {
                upgradedUnit.navAgent.enabled = true;
                upgradedUnit.navObstacle.enabled = false;
                upgradedUnit.freeModeState = FreeModeState.COMBAT;
            }

            Debug.Log(upgradedUnit.mode);

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


    public override void GetProvoked(Unit ProvokedTarget)
    {
        //Debug.Log(gameObject.name + " Has Provoked to " + ProvokedTarget.name);
        targetUnit = ProvokedTarget;
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

    private Unit SearchTarget(float range, LayerMask targetLayer)
    {
        Unit result = null;

        result = SearchMarkedTarget(range);

        if (result != null)
            return result;
        else
        {
            switch (data.TargetingType)
            {
                case TargetingType.NEAR:
                    result = SearchNearestTarget(range, targetLayer);
                    break;
                case TargetingType.LOWHP:
                    result = SearchLowHPTarget(range, targetLayer);
                    break;
                case TargetingType.HIGHTIER:
                    result = SearchHighTierTarget(range);
                    break;
            }

            return result;
        }
    }

    //public override Unit SearchNearestTarget(float range) // 프리 모드 유닛이 경로상 가장 가까운 적탐색
    //{
    //    Unit result = null;
    //    float nearestPathLength = float.MaxValue;


    //    int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
    //    if (targetCount <= 0)
    //        return null;


    //    for (int i = 0; i < targetCount; i++)
    //    {
    //        Unit target = collidersInRange[i].GetComponent<Unit>();

    //        if (target.IsDead)
    //            continue;

    //        NavMesh.CalculatePath(transform.position, target.transform.position, navAgent.areaMask, path);

    //        if (path.status != NavMeshPathStatus.PathComplete)
    //            continue;

    //        float pathLength = float.MaxValue;
    //        pathLength = CalculatePathLength(path);

    //        if (pathLength < nearestPathLength)
    //        {
    //            nearestPathLength = pathLength;
    //            result = target;
    //        }
    //    }

    //    return result;
    //}

    private Unit SearchReachableTarget(float range)
    {
        Unit result = null;
        switch (data.TargetingType)
        {
            case TargetingType.NEAR:
                result = SearchNearestReachableTarget(range);
                break;
            case TargetingType.LOWHP:
                result = SearchLowHPReachableTarget(range);
                break;
            case TargetingType.HIGHTIER:
                result = SearchHighTierReachableTarget(range);
                break;
        }

        return result;
    }

    private EnemyUnit SearchMarkedTarget(float range)
    {
        EnemyUnit result = null;
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if (targetCount > 0)
        {
            for (int i = 0; i < targetCount; i++)
            {
                EnemyUnit unit = collidersInRange[i].GetComponent<EnemyUnit>();

                if (unit.HasExecuteMark)
                {
                    if (unit.HpPercent <= 0f || !unit.gameObject.activeInHierarchy)
                        continue;
                    else
                        result = unit;
                }
            }
        }
        return result;
    }

    private EnemyUnit SearchReachableMarkedTarget(float range)
    {
        EnemyUnit result = null;
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if (targetCount > 0)
        {
            for (int i = 0; i < targetCount; i++)
            {
                EnemyUnit unit = collidersInRange[i].GetComponent<EnemyUnit>();

                if (unit.HasExecuteMark)
                {
                    if (unit.HpPercent <= 0f || !unit.gameObject.activeInHierarchy)
                        continue;

                    if (IsReachable(unit))
                    {
                        result = unit;
                    }
                }
            }
        }

        return result;
    }

    protected override void ActivateSkill(SkillBase skill, Unit target)
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


        base.Die();

        AddVFX(UnitDeathVFX.GetComponent<ParticleSystem>());

        unitGrid.ClearTile();

        SoundManager.Instance.PlaySFX(this.transform.position, allyDeadSFX);
    }




}
