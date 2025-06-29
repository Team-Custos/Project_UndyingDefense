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
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                        Debug.Log("Special Skill " + skill + "사용");
=======
                        //Debug.Log("Special Skill " + skill + "사용");
>>>>>>> Stashed changes
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
                        if(targetUnit != null)
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
<<<<<<< Updated upstream
                        //navAgent.enabled = false;
=======
                        navAgent.enabled = false;
>>>>>>> Stashed changes
                    }
                    else
                    {
                        if (navAgent.enabled && navAgent.velocity.magnitude > 0f)
                        {
                            state = State.RUN;
                            modelAnimator.SetBool("isRunning", true);
                        }
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
<<<<<<< Updated upstream
                                    if (SearchMarkedTarget(data.SightRange) != null)
                                    {
                                        targetUnit = SearchMarkedTarget(data.SightRange);
=======
                                    if (SearchMarkedTarget(UnitStats.sightRange) != null)
                                    {
                                        targetUnit = SearchMarkedTarget(UnitStats.sightRange);
>>>>>>> Stashed changes
                                    }

                                    if (targetUnit != null && targetUnit.HpPercent > 0f && targetUnit.gameObject.activeInHierarchy)
                                    {
<<<<<<< Updated upstream
                                        if (IsTargetInRange(targetUnit, data.AttackRange))
=======
                                        if (IsTargetInRange(targetUnit, UnitStats.attackRange))
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                                    else targetUnit = SearchTarget(data.SightRange);
=======
                                    else targetUnit = SearchTarget(UnitStats.sightRange);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                            targetUnit = SearchTarget(data.SightRange);
=======
                            targetUnit = SearchTarget(UnitStats.sightRange);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                                targetUnit = SearchTarget(data.SightRange);
=======
                                targetUnit = SearchTarget(UnitStats.sightRange);
>>>>>>> Stashed changes
                                navAgent.stoppingDistance = 2.4f;
                            }
                        }
                    }

<<<<<<< Updated upstream
                    if (SearchMarkedTarget(data.SightRange) != null)
                    {
                        targetUnit = SearchMarkedTarget(data.SightRange);
=======
                    if (SearchMarkedTarget(UnitStats.sightRange) != null)
                    {
                        targetUnit = SearchMarkedTarget(UnitStats.sightRange);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                                        if (IsTargetInRange(targetUnit, data.AttackRange))
=======
                                        if (IsTargetInRange(targetUnit, UnitStats.attackRange))
>>>>>>> Stashed changes
                                        {
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

<<<<<<< Updated upstream
                        if(IsTargetInAttackRange(targetUnit, data.AttackRange))
=======
                        if(IsTargetInAttackRange(targetUnit, UnitStats.attackRange))
>>>>>>> Stashed changes
                        {
                            return;
                        }

<<<<<<< Updated upstream
                        if (IsTargetInRange(targetUnit, data.SightRange))
=======
                        if (IsTargetInRange(targetUnit, UnitStats.sightRange))
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                        //SetNavMode(true);

                        //bool navAgentEnabled = navAgent.enabled;
                        //if (!navAgentEnabled) // navAgent가 비활성화 상태일 경우
                        //{
                        //    navObstacle.enabled = false;
                        //    navAgent.enabled = true;
                        //}

                        targetUnit = SearchReachableTarget(data.SightRange);

                        //if (!navAgentEnabled) // navAgent가 비활성화 상태일 경우
                        //{
                        //    navObstacle.enabled = false;
                        //    navAgent.enabled = true;
                        //}
                        //SetNavMode(true);
=======
                        bool navAgentEnabled = navAgent.enabled;
                        if (!navAgentEnabled) // navAgent가 비활성화 상태일 경우
                        {
                            navObstacle.enabled = false;
                            navAgent.enabled = true;
                        }

                        targetUnit = SearchReachableTarget(UnitStats.sightRange);

                        if (!navAgentEnabled) // navAgent가 비활성화 상태일 경우
                        {
                            navObstacle.enabled = false;
                            navAgent.enabled = true;
                        }
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
                                navAgent.enabled = false;
                            }
                            else
                            {
                                mode = Mode.FREE;
                                return;
                            }
<<<<<<< Updated upstream
                           
=======
                                

>>>>>>> Stashed changes
                            //transform.position = unitGrid.GetAvailableTile().transform.position;

                            OnOffSiefeEffect(false);
                            chagneEffet.SetActive(true);

                            particleDuration = 0.3f;
                            isSiegeActive = false;

                            isAvailableToSiege = true;
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                            if (selectedUnitUI != null)
=======

                            if(selectedUnitUI != null)
>>>>>>> Stashed changes
                            {
                                selectedUnitUI.ShowUnitDurtion(1 - (changeDuration / 3.0f));
                            }

                        }
                        else
                        {
                            mode = Mode.FREE;
<<<<<<< Updated upstream
                            SetNavMode(true);
=======
>>>>>>> Stashed changes

                            changeDuration = 3.0f;
                            previousMode = mode;

<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
                            if (selectedUnitUI != null && isSelected)
                            {
                                selectedUnitUI.ShowAllyUI(this);
                                selectedUnitUI.HideUnitDuration();
                            }
                        }
                    }
<<<<<<< Updated upstream
=======



>>>>>>> Stashed changes
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
                        if (previousMode == Mode.FREE)
                        {
                            mode = Mode.FREE;
                        }
                        else if (previousMode == Mode.SEIGE)
                        {
                            mode = Mode.SEIGE;
                        }

                        upgradeDuraiton = 3.0f;
                        previousMode = mode;

                        if (selectedUnitUI != null && isSelected)
                        {
                            selectedUnitUI.ShowAllyUI(this);
                            selectedUnitUI.HideUnitDuration();
                        }
                            
                    }
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

    public void Upgrade(int index)
    {
        if (data.UpgradeUnits.Length <= 0)
        {
            Debug.Log("데이터가 없습니다.");
            return;
        }
        else
        {
            UnitData upgradeUnitData = data.UpgradeUnits[index];
            if (data.UpgradeUnits[index] == null)
                return;

            selectedUnitUI.HideAllyUI();

            GameObject obj = upgradeUnitData.Prefab;
            spawner.CreateUpgradeUnit(obj, (AllyUnitData)upgradeUnitData, this.transform, this.mode, unitGrid.TargetTile);

            pool.Pool.Release(this);
            gameObject.SetActive(false);

        }


        // 새 유닛 데이터를 가져와 스포너 방식의 풀로
        // 프리팹 데이터를 가져옴, 기존 유닛 반환후 새 유닛생성, 생성된 유닛으로 풀 생성 -> 키 : 프리팹 , 벨류 allyunit 
    }

    public override void Die()
    {
        //navAgent.enabled = false;
        //navObstacle.enabled = false;
        //collider.enabled = false;

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
