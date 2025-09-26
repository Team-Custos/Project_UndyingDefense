using UnityEngine;
using UnityEngine.AI;

public class AllyUnit : Unit
{
    public enum Mode
    {
        SEIGE,
        FREE,
        UPGRADE
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
        DEAD
    }

    private Tile tile;
    private AllyUnitData data;
    private ObjectPoolWithList<AllyUnit> pool;

    private Mode mode;
    private bool isChange = false;
    public Mode ModeType => mode;

    private State state;

    private FreeModeState freeModeState;

    public bool IsChange => isChange;

    public override UnitData Data => data;

    private AllyUnitSpawner spawner;

    private float changeDuration = 3.0f;        // 모드 변경 시간
    private float upgradeDuraiton = 3.0f;
    public Mode previousMode;                  // 이전 모드 확인을 위한 변수

    [SerializeField] private NavMeshObstacle navObstacle;
    [SerializeField] private GameObject chagneEffet;
    [SerializeField] private GameObject siegeEffect;
    [SerializeField] private ParticleSystem siegeParticle;
    [SerializeField] private UnitGrid unitGrid;


    public UnitGrid UnitGrid => unitGrid;

    private Vector3 commandDestination;  // 이동명령 목적지

    [SerializeField] private float particleDuration = 0.3f;

    private bool isSiegeActive = false;
    private bool isAvailableToSiege = false; // 시즈 모드 가능한지 확인
    [SerializeField] private bool alternativeSkill;
    private bool skillFlague;

    protected static AudioClip[] allyDeadSFX;

    protected static AudioClip[] AllyDeadSFX
    {
        get
        {
            if (allyDeadSFX == null)
            {
                allyDeadSFX = Resources.LoadAll<AudioClip>("Sound/SFX/효과음/캐릭터/DeathSFX/AllyDeath");
            }
            return allyDeadSFX;
        }
    }

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

        navAgent.avoidancePriority = 2;
        //OnOffSiefeEffect(true);

        //navObstacle.carvingMoveThreshold = moveThresholdOnStop;
        //mode = Mode.FREE;
    }

    //public void UpgradeInitialize()
    //{
    //    base.Initialize();
    //    mode = Mode.UPGRADE;
    //    isDead = false;
    //}

    public void Initialize(AllyUnitData data, ObjectPoolWithList<AllyUnit> pool, AllyUnitSpawner spawner)
    {
        this.data = data;
        this.pool = pool;
        this.spawner = spawner;
    }


    protected override void Update()
    {
        if (isStop)
            return;

        interval -= Time.deltaTime;

        if (isChange)
            ChangeMode();

        switch (state)
        {
            case State.STUN:
                return;
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
                        if (targetUnit != null)
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
                        state = State.RUN;
                        modelAnimator.SetBool("isRunning", true);
                    }
                    else
                    {
                        if (targetUnit == null)
                        {
                            Vector3 direction = Vector3.left; //spawnDirection.forward; // 나중에 수정할 것!
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
        //if(state == State.STUN)
        //{
        //    Debug.Log("Stun");
        //    return;
        //}


        switch (mode)
        {
            case Mode.SEIGE:
                {
                    if (!navObstacle.enabled)
                        Debug.Log("obstacle off");


                    //OnOffSiefeEffect(true);
                    //chagneEffet.SetActive(false);

                    if (interval <= 0)
                    {
                        SkillBase skill = GetAvailableSkill();

                        if (skill != null) // 사용 가능한 스킬이 존재할 경우
                        {
                            SkillBase.TargetType skillTargetType = skill.GetTargetType(); // 스킬 대상 종류 확인
                            switch (skillTargetType)
                            {
                                case SkillBase.TargetType.ENEMY:
                                    {
                                        //if (SearchMarkedTarget(unitStats.sightRange) != null)
                                        //{
                                        //    targetUnit = SearchMarkedTarget(unitStats.sightRange);
                                        //}

                                        if (targetUnit != null)
                                        {
                                            if (targetUnit.IsDead)
                                            {
                                                targetUnit = null;
                                                return;
                                            }

                                            if (IsTargetInRange(targetUnit, UnitStats.attackRange))
                                                ActivateSkill(skill, targetUnit);
                                            else
                                                targetUnit = null;

                                        }
                                        else targetUnit = SearchTarget(UnitStats.sightRange);


                                    }
                                    break;
                                case SkillBase.TargetType.SELF:
                                    {
                                        ActivateSkill(skill, this);
                                        //interval = intervalCheck;
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (targetUnit == null)
                            targetUnit = SearchTarget(unitStats.sightRange);
                        else
                            LookAt(targetUnit.transform.position);
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

                            MoveTo(commandDestination);

                            if (navAgent.remainingDistance < navAgent.stoppingDistance)
                            {
                                freeModeState = FreeModeState.COMBAT;
                            }
                            break;

                        case FreeModeState.COMBAT:

                            if (targetUnit != null)
                            {
                                if (targetUnit.IsDead)
                                {
                                    targetUnit = null;
                                    return;
                                }

                                if (IsTargetInAttackRange(targetUnit, UnitStats.attackRange))// + targetUnit.NearbyDistance)) // 목표가 공격 범위 내 -> 공격
                                {
                                    if (interval <= 0)
                                    {
                                        SkillBase skill = GetAvailableSkill();
                                        if (skill != null)
                                        {
                                            SkillBase.TargetType skillTargetType = skill.GetTargetType(); // 스킬 대상 종류 확인
                                            switch (skillTargetType)
                                            {
                                                case SkillBase.TargetType.ENEMY:
                                                    {
                                                        navAgent.isStopped = true; // 스킬 사용시 이동 불가
                                                        modelAnimator.SetBool("isRunning", false);

                                                        if (IsTargetInRange(targetUnit, UnitStats.attackRange))
                                                        {
                                                            ActivateSkill(skill, targetUnit);

                                                            return;
                                                        }
                                                        break;
                                                    }

                                                case SkillBase.TargetType.ALLY:
                                                    {
                                                        ActivateSkill(skill, null);

                                                        break;
                                                    }
                                                case SkillBase.TargetType.SELF:
                                                    {
                                                        ActivateSkill(skill, null);
                                                        break;
                                                    }
                                            }
                                            navAgent.isStopped = false;
                                        }
                                    }
                                }
                                else if (IsTargetInRange(targetUnit, UnitStats.sightRange)) // 목표가 시야 범위 내 -> 이동
                                {
                                    MoveTo(targetUnit);
                                }
                                else
                                {
                                    targetUnit = null;
                                }
                            }
                            else
                            {
                                targetUnit = SearchTarget(UnitStats.sightRange);

                                if (targetUnit == null) // 찾아도 적이 없으면 idle 로
                                    state = State.IDLE;
                            }

                            break;
                    }

                    //OnOffSiefeEffect(false);
                    //chagneEffet.SetActive(false);

                    //if (commandDestination != Vector3.zero)
                    //{
                    //    targetUnit = null;

                    //    navAgent.isStopped = false;
                    //    navAgent.SetDestination(commandDestination);
                    //    modelAnimator.SetBool("isRunning", true);
                    //    navAgent.stoppingDistance = 0.1f;

                    //    if (!navAgent.pathPending)
                    //    {
                    //        if (navAgent.remainingDistance > navAgent.stoppingDistance)
                    //        {
                    //            isMoving = true;
                    //        }
                    //        else
                    //        {
                    //            modelAnimator.SetBool("isRunning", false);
                    //            commandDestination = Vector3.zero;
                    //            isMoving = false;
                    //            targetUnit = SearchTarget(UnitStats.sightRange);
                    //            navAgent.stoppingDistance = 1.0f;

                    //            // 이동 종료
                    //        }
                    //    }
                    //}
                    //else        // 이동 명령 없음 or 종료 -> 적 탐색
                    //{
                    //    if(targetUnit == null)
                    //    {
                    //        targetUnit = SearchTarget(UnitStats.sightRange);
                    //        state = State.IDLE;
                    //    }


                    //}


                    //if (targetUnit != null && targetUnit.HpPercent > 0f && targetUnit.gameObject.activeInHierarchy
                    //   && !isMoving)
                    //{
                    //    if (IsTargetInAttackRange(targetUnit, UnitStats.attackRange))// + targetUnit.NearbyDistance)) // 목표가 공격 범위 내 -> 공격
                    //    {
                    //        if (interval <= 0)
                    //        {
                    //            SkillBase skill = GetAvailableSkill();
                    //            if (skill != null)
                    //            {
                    //                SkillBase.TargetType skillTargetType = skill.GetTargetType(); // 스킬 대상 종류 확인
                    //                switch (skillTargetType)
                    //                {
                    //                    case SkillBase.TargetType.ENEMY:
                    //                        {
                    //                            navAgent.isStopped = true; // 스킬 사용시 이동 불가
                    //                            modelAnimator.SetBool("isRunning", false);

                    //                            if (IsTargetInRange(targetUnit, UnitStats.attackRange))
                    //                            {
                    //                                ActivateSkill(skill, targetUnit);

                    //                                if (stateDurationCheck >= stateDuration)
                    //                                {

                    //                                    stateDurationCheck = 0f;
                    //                                    stateDuration = 0f;
                    //                                }
                    //                                return;
                    //                            }
                    //                            break;
                    //                        }

                    //                    case SkillBase.TargetType.ALLY:
                    //                        {
                    //                            ActivateSkill(skill, null);


                    //                            if (stateDurationCheck >= stateDuration)
                    //                            {

                    //                                stateDurationCheck = 0f;
                    //                                stateDuration = 0f;
                    //                            }
                    //                            break;
                    //                        }
                    //                    case SkillBase.TargetType.SELF:
                    //                        {
                    //                            ActivateSkill(skill, null);

                    //                            if (stateDurationCheck >= stateDuration)
                    //                            {
                    //                                stateDurationCheck = 0f;
                    //                                stateDuration = 0f;
                    //                            }
                    //                            break;
                    //                        }
                    //                }
                    //                navAgent.isStopped = false;
                    //            }


                    //        }
                    //    }
                    //    else if (IsTargetInRange(targetUnit, UnitStats.sightRange)) // 목표가 시야 범위 내 -> 이동
                    //    {
                    //        MoveTo(targetUnit);
                    //        //modelAnimator.SetBool("isRunning", true);
                    //        //if (path.status != NavMeshPathStatus.PathComplete)
                    //        //    targetUnit = null;

                    //        //float dist = Vector3.Distance(transform.position, targetUnit.transform.position);
                    //        //Debug.Log($"2 : {dist}");
                    //    }
                    //    else
                    //    {
                    //        targetUnit = null;
                    //    }
                    //}
                    //else
                    //    targetUnit = null;
                    break;
                }

            //case Mode.CHANGE:
            //    {
            //         if (previousMode == Mode.FREE)      // 시즈로 변경
            //         {
            //             if (!isAvailableToSiege)
            //             {
            //                 Tile targetTile = unitGrid.GetAvailableTile();

            //                 if (targetTile != null)
            //                 {
            //                    transform.position = targetTile.transform.position;

            //                    navAgent.enabled = false;
            //                    navObstacle.enabled = true;
            //                }
            //                 else
            //                 {
            //                     mode = Mode.FREE;
            //                     return;
            //                 }


            //                 //transform.position = unitGrid.GetAvailableTile().transform.position;

            //                 OnOffSiefeEffect(false);
            //                 chagneEffet.SetActive(true);

            //                 particleDuration = 0.3f;
            //                 isSiegeActive = false;

            //                 isAvailableToSiege = true;

            //             }

            //             if (isAvailableToSiege)
            //             {
            //                 if (changeDuration >= 0)
            //                 {
            //                     changeDuration -= Time.deltaTime;
            //                     state = State.IDLE;

            //                     if (selectedUnitUI != null)
            //                     {
            //                         selectedUnitUI.ShowUnitDurtion(1 - (changeDuration / 3.0f));
            //                     }
            //                 }
            //                 else
            //                 {
            //                    ChangeMode(Mode.SEIGE);
            //                    OnOffSiefeEffect(true);
            //                    chagneEffet.SetActive(false);

            //                    changeDuration = 3.0f;
            //                     previousMode = mode;
            //                     isAvailableToSiege = false;


            //                     if (selectedUnitUI != null && isSelected)
            //                     {
            //                         selectedUnitUI.ShowAllyUI(this);
            //                         selectedUnitUI.HideUnitDuration();
            //                     }
            //                 }
            //             }
            //         }
            //         else if (previousMode == Mode.SEIGE) // 프리로 변경
            //        {
            //             unitGrid.ClearTile();

            //             OnOffSiefeEffect(false);
            //             chagneEffet.SetActive(true);

            //             particleDuration = 0.3f;
            //             isSiegeActive = false;

            //             if (changeDuration >= 0)
            //             {
            //                 state = State.IDLE;

            //                 changeDuration -= Time.deltaTime;
            //                 float progress = 1 - (changeDuration / 3.0f);

            //                 if (progress >= 0.9f && navObstacle.enabled) // 진행도 90% 이상일 때
            //                 {
            //                    navObstacle.enabled = false;
            //                 }

            //                 if (selectedUnitUI != null)
            //                 {
            //                     selectedUnitUI.ShowUnitDurtion(1 - (changeDuration / 3.0f));
            //                 }
            //             }
            //             else    // 체인지 상태 끝 -> 프리 모드 전환 완료
            //             {

            //                 ChangeMode(Mode.FREE);

            //                OnOffSiefeEffect(false);
            //                chagneEffet.SetActive(false);

            //                changeDuration = 3.0f;
            //                 previousMode = mode;

            //                navAgent.enabled = true;

            //                 if (selectedUnitUI != null && isSelected)
            //                 {
            //                     selectedUnitUI.ShowAllyUI(this);
            //                     selectedUnitUI.HideUnitDuration();
            //                 }
            //             }
            //         }



            //         //if (changeDuration >= 0)
            //         //{
            //         //    changeDuration -= Time.deltaTime;
            //         //    state = State.IDLE;
            //         //}
            //         //else
            //         //{
            //         //    if (previousMode == Mode.FREE)
            //         //    {
            //         //        mode = Mode.SEIGE;
            //         //    }
            //         //    else if (previousMode == Mode.SEIGE)
            //         //    {
            //         //        mode = Mode.FREE;
            //         //    }

            //         //    changeDuration = 3.0f;
            //         //    previousMode = mode;
            //         //}
            // }
            // break;

            case Mode.UPGRADE:
                {
                    OnOffSiefeEffect(false);
                    chagneEffet.SetActive(true);

                    particleDuration = 0.3f;
                    isSiegeActive = false;

                    if (upgradeDuraiton >= 0)
                    {
                        upgradeDuraiton -= Time.deltaTime;
                        state = State.IDLE;
                        if (navAgent.enabled)
                            navAgent.isStopped = true;
                        modelAnimator.SetBool("isRunning", false);

                        // 업그레이드 진행 중

                        if (selectedUnitUI != null)
                        {
                            selectedUnitUI.ShowUnitDurtion(1 - (upgradeDuraiton / 3.0f));
                        }
                    }
                    else
                    {
                        // 새 유닛 생성 & 교체
                        if (requestedUpgradeIndex >= 0 && spawner != null)
                        {
                            UnitData upgradeUnitData = data.UpgradeUnits[requestedUpgradeIndex];
                            if (upgradeUnitData != null)
                            {
                                GameObject prefab = upgradeUnitData.Prefab;

                                AllyUnit upgradedUnit = spawner.CreateUpgradeUnit(prefab, (AllyUnitData)upgradeUnitData, transform, unitGrid.TargetTile);

                                //upgradedUnit.ChangeMode(previousMode);
                                //upgradedUnit.navAgent.avoidancePriority = navAgent.avoidancePriority;
                                //upgradedUnit.navObstacle.carvingMoveThreshold = moveThresholdOnStop;

                                if (isSelected && selectedUnitUI != null)
                                {
                                    isSelected = false;
                                    upgradedUnit.isSelected = true;


                                    selectedUnitUI.UpdateUnitInfo(upgradedUnit);


                                    upgradedUnit.SetSelectedUnitManager(selectedUnitManager);
                                    upgradedUnit.selectedUnitManager.SetSelectedUnit(upgradedUnit);

                                    selectedUnitUI.ShowAllyUI(upgradedUnit);
                                    selectedUnitUI.ShowHp(upgradedUnit);
                                }
                            }


                            pool.Pool.Release(this);
                            gameObject.SetActive(false);
                            //mode = Mode.SEIGE;

                            requestedUpgradeIndex = -1;
                        }

                        upgradeDuraiton = 3.0f;
                        previousMode = mode;

                        if (selectedUnitUI != null && isSelected)
                        {
                            selectedUnitUI.ShowAllyUI(this);
                            selectedUnitUI.HideUnitDuration();
                        }
                    }
                    //else
                    //{
                    //    if (previousMode == Mode.FREE)
                    //    {
                    //        mode = Mode.FREE;
                    //    }
                    //    else if (previousMode == Mode.SEIGE)
                    //    {
                    //        mode = Mode.SEIGE;
                    //    }

                    //    upgradeDuraiton = 3.0f;
                    //    previousMode = mode;

                    //    if (selectedUnitUI != null && isSelected)
                    //    {
                    //        selectedUnitUI.ShowAllyUI(this);
                    //        selectedUnitUI.HideUnitDuration();
                    //    }

                    //}
                }

                break;
        }

    }




    public override void GetProvoked(Unit ProvokedTarget)
    {
        //Debug.Log(gameObject.name + " Has Provoked to " + ProvokedTarget.name);
        targetUnit = ProvokedTarget;
    }

    public override void GetStun()
    {
        base.GetStun();
        state = State.STUN;
    }

    public override void RemoveStun()
    {
        base.RemoveStun();
        state = State.IDLE;
    }

    private Unit SearchTarget(float range)
    {
        Unit result = null;
        switch (data.TargetingType)
        {
            case TargetingType.NEAR:
                result = SearchNearestTarget(range);
                break;
            case TargetingType.LOWHP:
                result = SearchLowHPTarget(range);
                break;
            case TargetingType.HIGHTIER:
                result = SearchHighTierTarget(range);
                break;
        }

        return result;
    }

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
        if (alternativeSkill)
        {
            if (skill != null)
            {
                if (skillFlague)
                {
                    modelAnimator.SetTrigger("GeneralSkill");
                }
                else
                {
                    modelAnimator.SetTrigger("SpecialSkill");
                }

                if (skill == GeneralSkill)
                {
                    state = State.GENERALSKILL;
                }
                else if (skill == SpecialSkill)
                {
                    state = State.SPECIALSKILL;
                }
            }

            skillFlague = !skillFlague;
        }
        else
        {
            if (skill == GeneralSkill)
            {
                state = State.GENERALSKILL;
                modelAnimator.SetTrigger("GeneralSkill");
            }
            else if (skill == SpecialSkill)
            {
                state = State.SPECIALSKILL;
                modelAnimator.SetTrigger("SpecialSkill");
            }
        }

        if (target != null && target != this)
            transform.LookAt(target.transform);

        interval = intervalCheck;

        if (stateDurationCheck >= stateDuration)
        {
            stateDurationCheck = 0f;
            stateDuration = 0f;
        }

        //base.ActivateSkill(skill, target);
    }

    public void ModeToChange()
    {
        isChange = true;

        if (selectedUnitUI != null)
            selectedUnitUI.HideAllyUI();
    }

    public void ChangeMode()
    {
        changeDuration -= Time.deltaTime;
        state = State.IDLE;

        switch (mode)
        {
            case Mode.SEIGE:     // 프리로 변경
                {
                    if (changeDuration > 0)      // 변경 중
                    {


                        if (selectedUnitUI != null)
                        {
                            selectedUnitUI.ShowUnitDurtion(1 - (changeDuration / 3.0f));
                        }

                    }
                    else    // 변경 끝
                    {

                        unitGrid.ClearTile();
                        navObstacle.enabled = false;

                        Debug.Log(1111);

                        NavMesh.CalculatePath(transform.position, transform.position, navAgent.areaMask, path);
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            Debug.Log(2222);

                            if (selectedUnitUI != null)
                            {
                                selectedUnitUI.ShowAllyUI(this);
                                selectedUnitUI.HideUnitDuration();
                            }

                            mode = Mode.FREE;
                            freeModeState = FreeModeState.COMBAT;

                            navAgent.enabled = true;

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
                        }
                        else // 주변 가능 타일이 없으면 변경 취소
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
                            state = State.IDLE;

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

                            //navAgent.enabled = false;
                            isAvailableToSiege = false;
                            mode = Mode.SEIGE;
                            changeDuration = 3f;
                            isChange = false;
                        }
                    }
                }
                break;

        }

        //this.mode = mode;


        //public void ChangeMode(Mode newMode)
        //{

        //    switch (newMode)
        //    {
        //        case Mode.CHANGE:
        //            newMode = Mode.CHANGE;
        //            if(selectedUnitUI != null)
        //                selectedUnitUI.HideAllyUI();
        //            break;

        //        case Mode.SEIGE:
        //            navAgent.enabled = false;
        //            navObstacle.enabled = true;
        //            break;

        //        case Mode.FREE:
        //            navObstacle.enabled = false;
        //            navAgent.enabled = true;
        //            freeModeState = FreeModeState.COMBAT;
        //            break;
        //    }

        //}
    }


    private int requestedUpgradeIndex = -1;

    public void RequestUpgrade(int index)
    {
        previousMode = mode;
        requestedUpgradeIndex = index;
        mode = Mode.UPGRADE;
        state = State.IDLE;

        upgradeDuraiton = 3.0f;
        if (selectedUnitUI != null)
        {
            selectedUnitUI.ShowUnitDurtion(0f);
        }
    }

    public override void Die()
    {
        if (isDead) return;

        navObstacle.enabled = false;

        if (state == State.STUN)
        {
            base.RemoveStun();
        }

        base.Die();


        state = State.DEAD;
        AddVFX(UnitDeathVFX.GetComponent<ParticleSystem>());
        unitGrid.ClearTile();

        if (allyDeadSFX == null)
            return;

        if (allyDeadSFX.Length > 0)
        {
            AudioClip clip = allyDeadSFX[Random.Range(0, allyDeadSFX.Length)];
            SoundManager.Instance.PlaySFX(clip);
        }

        //if (!isDead)
        //{

        //    state = State.DEAD;
        //    //modelAnimator.SetTrigger("Die");
        //    AddVFX(UnitDeathVFX.GetComponent<ParticleSystem>());
        //    unitGrid.ClearTile();

        //    if (allyDeadSFX == null)
        //        return;

        //    if (allyDeadSFX.Length > 0)
        //    {
        //        AudioClip clip = allyDeadSFX[Random.Range(0, allyDeadSFX.Length)];
        //        SoundManager.Instance.PlaySFX(clip);
        //    }

        //    isDead = true;
        //}
    }



    private void OnOffSiefeEffect(bool isSiege)
    {
        if (isSiege)
        {
            if (!isSiegeActive)
            {
                siegeParticle.gameObject.SetActive(true);
                isSiegeActive = true;
            }

            if (particleDuration > 0)
            {
                particleDuration -= Time.deltaTime;
            }
            else
            {
                siegeParticle.gameObject.SetActive(false);
                siegeEffect.SetActive(true);
                isSiegeActive = false;
            }
        }
        else
        {
            siegeParticle.gameObject.SetActive(false);
            siegeEffect.SetActive(false);
        }
    }

}
