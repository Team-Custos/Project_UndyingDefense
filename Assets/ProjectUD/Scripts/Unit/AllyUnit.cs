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
                        navAgent.isStopped = false;
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
                                                // hasTargetPos = false;
                                                return;
                                            }


                                            if (IsTargetInRange(targetUnit, UnitStats.attackRange))
                                                ActivateSkill(skill, targetUnit);
                                            else
                                            {
                                                targetUnit = null;
                                                // hasTargetPos = false;
                                            }   


                                        }
                                        else
                                        {
                                            targetUnit = SearchNearestTargetInLine(UnitStats.sightRange);
                                        }




                                    }
                                    break;

                                case SkillBase.TargetType.SELF:
                                    {
                                        ActivateSkill(skill, this);
                                        //interval = intervalCheck;
                                    }
                                    break;

                                case SkillBase.TargetType.ALLY:
                                    {
                                        Debug.Log(1111);
                                        ActivateSkill(skill, null);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (targetUnit == null)
                        {
                            targetUnit = SearchNearestTargetInLine(unitStats.sightRange);
                        }

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

                            if (idleState == IdleState.DEFAULT)
                                break;

                            if (targetUnit != null)
                            {
                                if (targetUnit.IsDead)
                                {
                                    targetUnit = null;
                                    //hasTargetPos = false;
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
                                    MoveToTargetUnit(targetUnit);
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
                                {
                                    state = State.IDLE;
                                    navAgent.isStopped = true;
                                }
                            }

                            break;
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

    private Unit SearchTarget(float range)
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
    }

    public override Unit SearchNearestTarget(float range) // 프리 모드 유닛이 경로상 가장 가까운 적탐색
    {
        Unit result = null;
        float nearestPathLength = float.MaxValue;


        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if (targetCount <= 0)
            return null;


        for (int i = 0; i < targetCount; i++)
        {
            Unit target = collidersInRange[i].GetComponent<Unit>();

            if (target.IsDead)
                continue;

            NavMesh.CalculatePath(transform.position, target.transform.position, navAgent.areaMask, path);

            if (path.status != NavMeshPathStatus.PathComplete)
                continue;

            float pathLength = float.MaxValue;
            pathLength = CalculatePathLength(path);

            if (pathLength < nearestPathLength)
            {
                nearestPathLength = pathLength;
                result = target;
            }
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

        interval = intervalCheck;

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
