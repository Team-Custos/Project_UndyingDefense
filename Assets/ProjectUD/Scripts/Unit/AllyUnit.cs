using UnityEngine;
using UnityEngine.AI;

public class AllyUnit : Unit
{
    public enum Mode
    {
        FREE,
        SEIGE,
        CHANGE,
        UPGRADE
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
    public Mode ModeType => mode;

    private State state;

    public override UnitData Data => data;

    private AllyUnitSpawner spawner;

    private float changeDuration = 3.0f;        // 모드 변경 시간
    private float upgradeDuraiton = 3.0f;
    public Mode previousMode;                  // 이전 모드 확인을 위한 변수

    [SerializeField] private GameObject chagneEffet;
    [SerializeField] private GameObject siegeEffect;
    [SerializeField] private ParticleSystem siegeParticle;
    [SerializeField] private UnitGrid unitGrid;


    public UnitGrid UnitGrid => unitGrid;

    private Vector3 destinationPosition;  // 프리모드 목적지

    private bool isMoving = false;          // 이동 명령 중인지 확인

    [SerializeField] private float particleDuration = 0.3f;

    private bool isSiegeActive = false;
    private bool isAvailableToSiege = false; // 시즈 모드 가능한지 확인
    private bool isSpawned = true;
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

    public Vector3 DestinationPosition
    {
        get => destinationPosition;
        set => destinationPosition = value;
    }


    public override void Initialize()
    {
        base.Initialize();
        mode = Mode.SEIGE;
        previousMode = mode;
        state = State.IDLE;
        //mode = Mode.FREE;
    }

    public void UpgradeInitialize()
    {
        base.Initialize();
        mode = Mode.UPGRADE;
    }

    public void Initialize(AllyUnitData data, ObjectPoolWithList<AllyUnit> pool, AllyUnitSpawner spawner)
    {
        this.data = data;
        this.pool = pool;
        this.spawner = spawner;
    }


    protected override void Update()
    {
        base.Update();

        switch (state)
        {
            case State.STUN:
                //{
                //    if (stateDuration <= 0f)
                //        return;
                //    if (stateDurationCheck < stateDuration)
                //    {
                //        stateDurationCheck += Time.deltaTime;
                //        navAgent.speed = 0f;
                //        navAgent.isStopped = true;
                //    }
                //    else
                //    {
                //        stateDurationCheck = 0f;
                //        stateDuration = 0f;

                //        state = State.IDLE;
                //        navAgent.isStopped = false;
                //        navAgent.speed = data.MoveSpeed * moveSpeedMultiplier;

                //        modelAnimator.SetBool("isStun", false);
                //    }
                //}
                //break;
            case State.GENERALSKILL:
            case State.SPECIALSKILL:
            case State.DEAD:
                {
                    if(state != State.DEAD)
                    {
                        if (navAgent.enabled)
                        {
                            navAgent.enabled = false;
                            modelAnimator.SetBool("isRunning", false);
                        }

                        if (!navObstacle.enabled)
                            navObstacle.enabled = true;
                    }

                    if (state == State.SPECIALSKILL)
                    {
                        if(targetUnit != null)
                            LookAt(targetUnit.transform.position);
                        SkillBase skill = GetSpecialSkill();
                        //Debug.Log("Special Skill " + skill + "사용");
                        if (skill != null)
                        {
                            if (stateDurationCheck >= skill.AnimationStateTime)
                            {
                                Debug.Log(skill.name);
                                base.ActivateSkill(skill, targetUnit);
                            }
                        }
                    }

                    if (state == State.GENERALSKILL)
                    {
                        if(targetUnit != null)
                            LookAt(targetUnit.transform.position);
                        SkillBase skill = GetGeneralSkill();

                        if (skill != null)
                        {
                            if (stateDurationCheck >= skill.AnimationStateTime)
                            {
                                Debug.Log(skill.name);
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
                        stateDurationCheck = 0f;
                        stateDuration = 0f;

                        if (state == State.DEAD)
                            gameObject.SetActive(false);

                        state = State.IDLE;

                        if(mode == Mode.FREE)
                            navObstacle.enabled = false;
                    }
                }
                break;
            case State.IDLE:
                {
                    if (mode == Mode.CHANGE)
                    {
                        modelAnimator.SetBool("isRunning", false);
                        navAgent.enabled = false;
                    }
                    else
                    {
                        if (navAgent.enabled && navAgent.velocity.magnitude > 0f)
                        {
                            state = State.RUN;
                            modelAnimator.SetBool("isRunning", true);
                        }
                        
                        if(spawner != null && targetUnit == null)
                            spawner.ResetAllyUnitRotation(this);


                    }

                    UpdateMode();
                }
                break;
            case State.RUN:
                {
                    if (!navAgent.enabled || navAgent.velocity.magnitude <= 0f)
                    {
                        if(navAgent.enabled)
                        {
                            navAgent.enabled = false;
                            navObstacle.enabled = true;
                        }

                        state = State.IDLE;
                        modelAnimator.SetBool("isRunning", false);
                    }

                    UpdateMode();
                }
                break;
        }

        
        /*SkillBase skill = GetAvailableSkill();
        //if (skill != null) // 사용 가능한 스킬이 존재할 경우
        //{
        //    SkillBase.TargetType skillTargetType = skill.GetTargetType(); // 스킬 대상 종류 확인
        //    switch (skillTargetType)
        //    {
        //        case SkillBase.TargetType.ENEMY:
        //            {
        //                if (skillTarget != null && skillTarget.gameObject.activeInHierarchy)
        //                {
        //                    if (IsTargetInRange(skillTarget, Data.AttackRange))
        //                    {
        //                        ActivateSkill(skill, skillTarget);
        //                        return;
        //                    }
        //                    else if (IsTargetInRange(skillTarget, Data.SightRange))
        //                    {
        //                        // 추적
        //                        chaseTarget = skillTarget;
        //                        skillTarget = null;

        //                        Vector3 targetPos = chaseTarget.transform.position;
        //                        LookAt(targetPos);
        //                        if (mode == Mode.FREE)
        //                            SetDestination(targetPos);

        //                        return;
        //                    }
        //                    else
        //                    {
        //                        skillTarget = null;
        //                        chaseTarget = null;
        //                    }
        //                }
        //                else
        //                {
        //                    // 대상이 있긴 한데, 활성화 상태가 아닌 경우
        //                    if (skillTarget != null)
        //                        skillTarget = null;

        //                    switch (Data.TargetingType)
        //                    {
        //                        case TargetingType.NEAR:
        //                            {
        //                                chaseTarget = SearchNearestTarget(Data.SightRange);
        //                            }
        //                            break;
        //                        case TargetingType.LOWHP:
        //                            {
        //                                chaseTarget = SearchLowHPTarget(Data.SightRange);
        //                            }
        //                            break;
        //                        case TargetingType.HIGHTIER:
        //                            {
        //                                chaseTarget = SearchHighTierTarget(Data.SightRange);
        //                            }
        //                            break;
        //                    }

        //                    if (chaseTarget != null)
        //                    {
        //                        if (IsTargetInRange(chaseTarget, Data.AttackRange))
        //                        {
        //                            // 스킬 발동
        //                            skillTarget = chaseTarget;
        //                            ActivateSkill(skill, skillTarget);
        //                            return;
        //                        }
        //                        else
        //                        {
        //                            // 추적
        //                            Vector3 targetPos = chaseTarget.transform.position;
        //                            LookAt(targetPos);
        //                            if (mode == Mode.FREE)
        //                                SetDestination(targetPos);

        //                            return;
        //                        }
        //                    }
        //                }
        //            }
        //            break;
        //        case SkillBase.TargetType.ALLY:
        //            {
        //                ActivateSkill(skill, null);
        //            }
        //            break;
        //    }
        //}
        */
    }

    private void UpdateMode()
    {
        if(state == State.STUN)
            return;

        switch (mode)
        {
            case Mode.SEIGE:
                {
                    OnOffSiefeEffect(true);
                    chagneEffet.SetActive(false);

                    //navObstacle.enabled = true;

                    SkillBase skill = GetAvailableSkill();
                    if (skill != null) // 사용 가능한 스킬이 존재할 경우
                    {
                        if (state == State.STUN)
                            return;

                        SkillBase.TargetType skillTargetType = skill.GetTargetType(); // 스킬 대상 종류 확인
                        switch (skillTargetType)
                        {
                            case SkillBase.TargetType.ENEMY:
                                {
                                    if (SearchMarkedTarget(UnitStats.sightRange) != null)
                                    {
                                        targetUnit = SearchMarkedTarget(UnitStats.sightRange);
                                    }

                                    if (targetUnit != null && targetUnit.HpPercent > 0f && targetUnit.gameObject.activeInHierarchy)
                                    {
                                        if (IsTargetInRange(targetUnit, UnitStats.attackRange))
                                        {
                                            if (navAgent.enabled && !navAgent.isStopped)
                                            {
                                                navAgent.isStopped = true;
                                                modelAnimator.SetBool("isRunning", false);
                                            }


                                            ActivateSkill(skill, targetUnit);

                                            //modelAnimator.SetTrigger("GeneralSkill");
                                        }
                                        else
                                            targetUnit = null;
                                    }
                                    else targetUnit = SearchTarget(UnitStats.sightRange);
                                }
                                break;
                            case SkillBase.TargetType.ALLY:
                                {
                                    //if (stateDurationCheck >= stateDuration)
                                    //{
                                    //    ActivateSkill(skill, null);
                                    //    stateDurationCheck = 0f;
                                    //    stateDuration = 0f;
                                    //}

                                    ActivateSkill(skill, null);
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (targetUnit == null || targetUnit.HpPercent <= 0f || !targetUnit.gameObject.activeInHierarchy)
                            targetUnit = SearchTarget(UnitStats.sightRange);
                        else
                            LookAt(targetUnit.transform.position);   
                    }
                }
                break;
            case Mode.FREE:
                {
                    OnOffSiefeEffect(false);
                    chagneEffet.SetActive(false);

                    if (destinationPosition != Vector3.zero)
                    {
                        targetUnit = null;

                        navAgent.SetDestination(destinationPosition);
                        modelAnimator.SetBool("isRunning", true);
                        navAgent.stoppingDistance = 0.1f;

                        if (!navAgent.pathPending)
                        {
                            if (navAgent.remainingDistance > navAgent.stoppingDistance)
                            {
                                isMoving = true;
                            }
                            else
                            {
                                modelAnimator.SetBool("isRunning", false);
                                destinationPosition = Vector3.zero;
                                isMoving = false;
                                targetUnit = SearchTarget(UnitStats.sightRange);
                                navAgent.stoppingDistance = 2.4f;
                            }
                        }
                    }

                    if (SearchMarkedTarget(UnitStats.sightRange) != null)
                    {
                        targetUnit = SearchMarkedTarget(UnitStats.sightRange);
                    }

                    if (targetUnit != null && targetUnit.HpPercent > 0f && targetUnit.gameObject.activeInHierarchy
                       && !isMoving)
                    {

                        SkillBase skill = GetAvailableSkill();
                        if(skill != null)
                        {
                            if (state == State.STUN)
                                return;

                            SkillBase.TargetType skillTargetType = skill.GetTargetType(); // 스킬 대상 종류 확인
                            switch (skillTargetType)
                            {
                                case SkillBase.TargetType.ENEMY:
                                    {
                                        if (IsTargetInRange(targetUnit, UnitStats.attackRange))
                                        {
                                            //float dist = Vector3.Distance(transform.position, targetUnit.transform.position);

                                            //Debug.Log(dist);

                                            //if (dist > UnitStats.attackRange)
                                            //    return;
                                            

                                            ActivateSkill(skill, targetUnit);

                                            if (stateDurationCheck >= stateDuration)
                                            {
                                                
                                                stateDurationCheck = 0f;
                                                stateDuration = 0f;
                                            }
                                            return;
                                        }
                                    }
                                    break;
                                case SkillBase.TargetType.ALLY:
                                    {
                                        ActivateSkill(skill, null);

                                        if (stateDurationCheck >= stateDuration)
                                        {
                                            
                                            stateDurationCheck = 0f;
                                            stateDuration = 0f;
                                        }
                                    }
                                    return;
                            }
                        }


                        if(IsTargetInAttackRange(targetUnit, UnitStats.attackRange))
                        {
                            // 공격 범위 내에 적이 있으면 코드 종료
                            return;
                        }

                        if (IsTargetInRange(targetUnit, UnitStats.sightRange))
                        {
                            MoveTo(targetUnit);
                            modelAnimator.SetBool("isRunning", true);
                            if (path.status != NavMeshPathStatus.PathComplete)
                                targetUnit = null;
                        }
                        else
                        {
                            targetUnit = null;
                        }

                    }
                    else
                    {
                        bool navAgentEnabled = navAgent.enabled;
                        if (!navAgentEnabled) // navAgent가 비활성화 상태일 경우
                        {
                            navObstacle.enabled = false;
                            navAgent.enabled = true;
                        }

                        targetUnit = SearchReachableTarget(UnitStats.sightRange);

                        //if (!navAgentEnabled) // navAgent가 비활성화 상태일 경우
                        //{
                        //    navObstacle.enabled = false;
                        //    navAgent.enabled = true;
                        //}
                    }
                }
                break;
            case Mode.CHANGE:
                {
                    destinationPosition = Vector3.zero;

                    if (previousMode == Mode.FREE)      // 시즈로 변경
                    {
                        if (!isAvailableToSiege)
                        {
                            Tile targetTile = unitGrid.GetAvailableTile();

                            if (targetTile != null)
                            {
                                transform.position = targetTile.transform.position;
                                navObstacle.enabled = true;

                                navAgent.enabled = false;
                            }
                            else
                            {
                                mode = Mode.FREE;
                                return;
                            }
                                

                            //transform.position = unitGrid.GetAvailableTile().transform.position;

                            OnOffSiefeEffect(false);
                            chagneEffet.SetActive(true);

                            particleDuration = 0.3f;
                            isSiegeActive = false;

                            isAvailableToSiege = true;

                        }

                        if (isAvailableToSiege)
                        {
                            if (changeDuration >= 0)
                            {
                                changeDuration -= Time.deltaTime;
                                state = State.IDLE;

                                if (selectedUnitUI != null)
                                {
                                    selectedUnitUI.ShowUnitDurtion(1 - (changeDuration / 3.0f));
                                }
                            }
                            else
                            {
                                mode = Mode.SEIGE;
                                changeDuration = 3.0f;
                                previousMode = mode;
                                isAvailableToSiege = false;

                                if (selectedUnitUI != null && isSelected)
                                {
                                    selectedUnitUI.ShowAllyUI(this);
                                    selectedUnitUI.HideUnitDuration();
                                }
                            }
                        }
                    }
                    else if (previousMode == Mode.SEIGE)
                    {
                        unitGrid.ClearTile();

                        OnOffSiefeEffect(false);
                        chagneEffet.SetActive(true);

                        particleDuration = 0.3f;
                        isSiegeActive = false;

                        if (changeDuration >= 0)
                        {
                            changeDuration -= Time.deltaTime;
                            state = State.IDLE;

                            if(selectedUnitUI != null)
                            {
                                selectedUnitUI.ShowUnitDurtion(1 - (changeDuration / 3.0f));
                            }

                        }
                        else
                        {
                            mode = Mode.FREE;

                            changeDuration = 3.0f;
                            previousMode = mode;


                            if (selectedUnitUI != null && isSelected)
                            {
                                selectedUnitUI.ShowAllyUI(this);
                                selectedUnitUI.HideUnitDuration();
                            }
                        }
                    }



                    //if (changeDuration >= 0)
                    //{
                    //    changeDuration -= Time.deltaTime;
                    //    state = State.IDLE;
                    //}
                    //else
                    //{
                    //    if (previousMode == Mode.FREE)
                    //    {
                    //        mode = Mode.SEIGE;
                    //    }
                    //    else if (previousMode == Mode.SEIGE)
                    //    {
                    //        mode = Mode.FREE;
                    //    }

                    //    changeDuration = 3.0f;
                    //    previousMode = mode;
                    //}
                }
                break;

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

                                AllyUnit upgradedUnit = spawner.CreateUpgradeUnit(prefab, (AllyUnitData)upgradeUnitData, transform, previousMode, unitGrid.TargetTile);

                                if (previousMode == Mode.FREE)
                                    upgradedUnit.ChangeMode(Mode.FREE);
                                else if (previousMode == Mode.SEIGE)
                                    upgradedUnit.ChangeMode(Mode.SEIGE);
                            }

                            

                            pool.Pool.Release(this);
                            gameObject.SetActive(false);

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

        if (target != this)
            transform.LookAt(target.transform);

        //base.ActivateSkill(skill, target);
    }

    public void ChangeMode(Mode mode)
    {
        this.mode = mode;
        if(selectedUnitUI != null)
            selectedUnitUI.HideAllyUI();
    }

    //public void Upgrade(int index)
    //{
    //    if (data.UpgradeUnits.Length <= 0)
    //    {
    //        Debug.Log("데이터가 없습니다.");
    //        return;
    //    }
    //    else
    //    {
    //        UnitData upgradeUnitData = data.UpgradeUnits[index];
    //        if (data.UpgradeUnits[index] == null)
    //            return;

    //        selectedUnitUI.HideAllyUI();

    //        GameObject obj = upgradeUnitData.Prefab;
    //        spawner.CreateUpgradeUnit(obj, (AllyUnitData)upgradeUnitData, this.transform, this.mode, unitGrid.TargetTile);

    //        pool.Pool.Release(this);
    //        gameObject.SetActive(false);

    //    }


    //    // 새 유닛 데이터를 가져와 스포너 방식의 풀로
    //    // 프리팹 데이터를 가져옴, 기존 유닛 반환후 새 유닛생성, 생성된 유닛으로 풀 생성 -> 키 : 프리팹 , 벨류 allyunit 
    //}

    private int requestedUpgradeIndex = -1;

    public void RequestUpgrade(int index)
    {
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
        navAgent.enabled = false;
        navObstacle.enabled = false;
        collider.enabled = false;

        base.Die();

        state = State.DEAD;
        //modelAnimator.SetTrigger("Die");
        AddVFX(UnitDeathVFX.GetComponent<ParticleSystem>());
        unitGrid.ClearTile();

        if (allyDeadSFX == null)
            return;

        if (allyDeadSFX.Length > 0)
        {
            AudioClip clip = allyDeadSFX[Random.Range(0, allyDeadSFX.Length)];
            SoundManager.Instance.PlaySFX(clip);
        }
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
