using System.Collections.Generic;
using System.Data;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyUnit : Unit
{
    private enum Mode
    {
        MOVE,
        COMBAT,
        ATTACKFORTRESS
    }

    private enum State
    {
        IDLE,
        BATTLECRY,
        RUN,
        GENERALSKILL,
        SPECIALSKILL,
        STUN,
        FORTRESSSKILL,
        DEAD
    }

    public enum TargetingType    //유닛의 타겟 선정 방식.
    {
        NEAR,           // 가까운 적
        LOWHP,          // HP가 낮은 적
        HIGHTIER        // 티어가 높은 적
    }

    public enum AIStance
    {
        NORMAL,
        AGGRESSIVE
    }

    private enum Mission
    {
        SIEGE,
        FIGHTER,
    }

    private EnemyUnitData data;
    private ObjectPoolWithList<EnemyUnit> pool;
    private EnemyUnitSpawner enemySpawner;

    private Mode mode;
    private State state;
    //private AIStance aiStance;
    [SerializeField] private Mission mission;

    private Fortress fortress;
    private Vector3 fortressPos;

    // 시즈 모드 유닛을 향해 갈때 사용하는 변수
    private Vector3 targetPos;
    private bool hasTargetPos = false;
    private bool isFortressPathBlocked = false;     // 성으로의 이동 경로가 막혔는지 여부 확인

    private const float angerTriggerPercent = 100f; // 분노 발동 기준 퍼센트

    private ExecutionEffect executionEffect;


    public override UnitData Data => data;

    private float fortressAttackCool;

    [SerializeField] private AudioClip[] enemyDeadSFX;

    protected static GameObject coinDropVFX;

    protected static GameObject warCryVFX;

    //protected static AudioClip[] EnemyDeadSFX
    //{
    //    get
    //    {
    //        if (enemyDeadSFX == null)
    //        {
    //            enemyDeadSFX = Resources.LoadAll<AudioClip>("Sound/SFX/효과음/캐릭터/DeathSFX/EnemyDeath");
    //        }
    //        return enemyDeadSFX;
    //    }
    //}

    protected static GameObject CoinDropVFX
    {
        get
        {
            if (coinDropVFX == null)
            {
                coinDropVFX = Resources.Load<GameObject>("Prefabs/VFX/UnitDeath/vfx_coinReward");
            }
            return coinDropVFX;
        }
    }

    protected static GameObject WarCryVFX
    {
        get
        {
            if (warCryVFX == null)
            {
                warCryVFX = Resources.Load<GameObject>("Prefabs/VFX/WarCry/vfx_warCry");
            }
            return warCryVFX;
        }
    }

    public void SetExecuted(ExecutionEffect executionEffect, bool executed, GameObject effect)
    {
        hasExecutedMark = executed;

        if(!executed)   // 제거
        {
            effect.SetActive(false);
            this.executionEffect = null;
            isPriorityTarget = false;
        }
        else
        {
            this.executionEffect = executionEffect;
            effect.transform.SetParent(effectParent);
            effect.transform.position = heightPos.position + Vector3.up * 1.5f;
            effect.SetActive(true);
            executionEffect.ActivateExecution();
            isPriorityTarget = true;
        }
    }

    public void Initialize(EnemyUnitData data, ObjectPoolWithList<EnemyUnit> pool, Fortress fortress, EnemyUnitSpawner enemySpawner)
    {
        this.data = data;
        this.pool = pool;
        this.fortress = fortress;
        this.enemySpawner = enemySpawner;
    }

    public void Initialize(Vector3 fortressPos)
    {
        Initialize();
        this.fortressPos = fortressPos;
    }

    public override void Initialize()
    {
        base.Initialize();
        navAgent.enabled = true;
        state = State.BATTLECRY;
        isDead = false;
        //aiStance = data.aiStance;
        mode = Mode.MOVE;
        //behaviorPriority = BehaviorPriority.Move;
        navAgent.avoidancePriority = 1;

        fortressAttackCool = GeneralSkill.Data.CoolTime;
    }



    protected override void Update()
    {
        //base.Update();

        if (isStop)
            return;
            

        interval -= Time.deltaTime;

        switch (state)
        {
            case State.STUN:
                break;
            case State.GENERALSKILL:
            case State.SPECIALSKILL:
            case State.BATTLECRY:
            case State.FORTRESSSKILL:
            case State.DEAD:
                {

                    if (state != State.DEAD)
                    {
                        modelAnimator.SetBool("isRunning", false);


                        //if (navAgent.enabled)
                        //{
                        //    navAgent.enabled = false;
                        //}


                        //if (!navObstacle.enabled)
                        //    navObstacle.enabled = true;
                    }

                    if (state == State.SPECIALSKILL)
                    {
                        if (targetUnit != null)
                            LookAt(targetUnit.transform.position);
                        SkillBase skill = GetSpecialSkill();
                        if (skill != null)
                        {
                            if (stateDurationCheck >= skill.AnimationStateTime && isSkillActive)
                            {
                                base.ActivateSkill(skill, targetUnit);
                                SkillBase.TargetType skillTargetType = skill.GetTargetType();
                                if (skillTargetType == SkillBase.TargetType.ALLY)
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

                    if(state == State.FORTRESSSKILL)
                    {
                        SkillBase skill = GetGeneralSkill();

                        if (skill != null)
                        {
                            if (stateDurationCheck >= skill.AnimationStateTime && isSkillActive)
                            {
                                ActivateFortressSkil();
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

                        if (state == State.BATTLECRY)
                            MoveTo(fortressPos);
                        else if (state == State.DEAD)
                        {
                            gameObject.SetActive(false);
                            pool.Pool.Release(this);
                        }
                            

                        state = State.IDLE;

                    }
                }
                break;
            case State.IDLE:
                {
                    if (navAgent.enabled && navAgent.velocity.magnitude > 0f)
                    {
                        state = State.RUN;
                        modelAnimator.SetBool("isRunning", true);
                    }

                    UpdateMode();
                }
                break;
            case State.RUN:
                {
                    if (!navAgent.enabled || navAgent.velocity.magnitude <= 0f)
                    {
                        //if (navAgent.enabled)
                        //{
                        //    navAgent.enabled = false;
                        //    navObstacle.enabled = true;
                        //}

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

        switch (mission)
        {
            case Mission.SIEGE:
                {
                    float distance = Vector3.Distance(transform.position, fortressPos);
                    float range = GeneralSkill.Data.Range;

                    if (distance <= range)
                    {
                        SkillBase generalSkill = GetGeneralSkill();
                        if (generalSkill != null)
                            ActivateSkill(fortress, data);
                        return;
                    }

                    if (isFortressPathBlocked)
                    {
                        if (interval <= 0f && currentSkill == null)     // 인터벌 중이 아니고, 보유 스킬이 없는 경우 스킬 선택
                        {
                            currentSkill = GetAvailableSkill();
                        }

                        if (currentSkill != null)
                        {
                            SearchReachableTargets(unitStats.sightRange, enemyLayer);

                            if (targets.Count > 0) // 적 존재
                            {
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
                                        case SkillBase.TargetType.ALLY:     // 탐색 -> 스킬 발동 or 이동
                                            {
                                                if (targetUnit is AllyUnit)
                                                    targetUnit = null;  // 공격 스킬 대상 초기화

                                                if (targetUnit != null) // 탐색된 대상이 있음
                                                {
                                                    if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
                                                    {
                                                        UpdateSkillState(currentSkill, targetUnit);
                                                        //targetUnit = null;
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

                                                    if (targetUnit != null)
                                                    {
                                                        UpdateSkillState(currentSkill, targetUnit);
                                                        //targetUnit = null;
                                                    }
                                                    else
                                                    {
                                                        SearchReachableTargets(unitStats.sightRange, allyLayer); //  시야 범위 내 이동 가능 유닛

                                                        if (targets.Count > 0)
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
                                                        hasTargetPos = false;
                                                        UpdateSkillState(currentSkill, targetUnit);
                                                    }
                                                    else // 스킬 사거리 < 대상과 거리 < 시야 사거리
                                                    {
                                                        if (IsPathBlocked(targetUnit))   // 이동 가능 여부 확인
                                                        {
                                                            hasTargetPos = false;
                                                            targetUnit = null;  // 막힘
                                                        }
                                                        else
                                                        {
                                                            MoveToTargetUnit(targetUnit);
                                                        }
                                                    }
                                                }
                                                else      // 새 대상 탐색
                                                {
                                                    targetUnit = null;
                                                    hasTargetPos = false;

                                                    targetUnit = SearchTarget(currentSkill.Data.Range, enemyLayer, currentSkill);
                                                    if (targetUnit != null)
                                                    {
                                                        hasTargetPos = false;
                                                        UpdateSkillState(currentSkill, targetUnit);
                                                    }
                                                    else
                                                    {
                                                        SearchReachableTargets(unitStats.sightRange, enemyLayer); // 이동 가능한 대상 탐색
                                                        targetUnit = SearchTargetInTargets(currentSkill); // 시야 내로 다시 검사


                                                        if (targetUnit != null)
                                                        {
                                                            MoveToTargetUnit(targetUnit);
                                                        }
                                                        else
                                                            currentSkill = null;
                                                    }
                                                }

                                                break;
                                            }
                                    }
                                }
                            }
                            else    // 시야 내 적 없음  -> 성 길 확인
                            {
                                NavMesh.CalculatePath(transform.position, fortressPos, navAgent.areaMask, path);

                                if (path.status == NavMeshPathStatus.PathComplete)
                                {
                                    isFortressPathBlocked = false;
                                }

                                ForceMoveTo(fortressPos);

                            }
                        }
                        


                    }
                    else
                    {
                        NavMesh.CalculatePath(transform.position, fortressPos, navAgent.areaMask, path);
                        if (path.status != NavMeshPathStatus.PathComplete)
                        {
                            isFortressPathBlocked = true;
                        }
                    }
                    break;
                }
            case Mission.FIGHTER:
                {
                    if (interval <= 0f && currentSkill == null)     // 인터벌 중이 아니고, 보유 스킬이 없는 경우 스킬 선택
                    {
                        currentSkill = GetAvailableSkill();
                    }

                    if (currentSkill != null)
                    {
                        SearchReachableTargets(unitStats.sightRange, enemyLayer); // 시야 범위 내 적 확인

                        if (targets.Count > 0) // 적 존재
                        {
                            SkillBase.TargetType skillTargetType = currentSkill.GetTargetType(); // 스킬 대상 종류 확인

                            switch (skillTargetType)
                            {
                                case SkillBase.TargetType.NONE:
                                    {
                                        UpdateSkillState(currentSkill, null);
                                        break;
                                    }
                                case SkillBase.TargetType.ALLY:     // 탐색 -> 스킬 발동 or 이동
                                    {
                                        if (targetUnit is AllyUnit)
                                            targetUnit = null;  // 공격 스킬 대상 초기화

                                        if (targetUnit != null) // 탐색된 대상이 있음
                                        {
                                            if (IsTargetInAttackRange(targetUnit, base.currentSkill.Data.Range))
                                            {
                                                UpdateSkillState(base.currentSkill, targetUnit);
                                                //targetUnit = null;
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
                                            targetUnit = SearchTarget(base.currentSkill.Data.Range, allyLayer, base.currentSkill);  // 스킬 사거리내로 먼저 검사

                                            if (targetUnit != null)
                                            {
                                                UpdateSkillState(base.currentSkill, targetUnit);
                                                //targetUnit = null;
                                            }
                                            else
                                            {
                                                SearchReachableTargets(unitStats.sightRange, allyLayer); //  시야 범위 내 이동 가능 유닛

                                                if (targets.Count > 0)
                                                {
                                                    targetUnit = SearchTargetInTargets(base.currentSkill);  //대상 선택

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
                                                hasTargetPos = false;
                                                UpdateSkillState(currentSkill, targetUnit);
                                            }
                                            else // 스킬 사거리 < 대상과 거리 < 시야 사거리
                                            {
                                                if (IsPathBlocked(targetUnit))   // 이동 가능 여부 확인
                                                {
                                                    hasTargetPos = false;
                                                    targetUnit = null;  // 막힘
                                                }
                                                else
                                                {
                                                    MoveToTargetUnit(targetUnit);
                                                }
                                            }
                                        }
                                        else      // 새 대상 탐색
                                        {
                                            targetUnit = null;
                                            hasTargetPos = false;

                                            targetUnit = SearchTarget(currentSkill.Data.Range, enemyLayer, currentSkill);
                                            if (targetUnit != null)
                                            {
                                                hasTargetPos = false;
                                                UpdateSkillState(currentSkill, targetUnit);
                                            }
                                            else
                                            {
                                                SearchReachableTargets(unitStats.sightRange, enemyLayer); // 이동 가능한 대상 탐색
                                                targetUnit = SearchTargetInTargets(currentSkill); // 시야 내로 다시 검사

                                                if (targetUnit != null)
                                                {
                                                    MoveToTargetUnit(targetUnit);
                                                }
                                                else
                                                {
                                                    navAgent.isStopped = false;
                                                    ForceMoveTo(fortressPos);
                                                    currentSkill = null;
                                                }
                                            }
                                        }

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            float distance = Vector3.Distance(transform.position, fortressPos);  // 성까지 거리 계산
                            float range = GeneralSkill.Data.Range;

                            if (distance <= range)   // 사거리 내 도달
                            {
                                SkillBase generalSkill = GetGeneralSkill();
                                if (generalSkill != null)
                                    ActivateSkill(fortress, data);
                            }
                            else
                            {
                                ForceMoveTo(fortressPos);
                            }
                        }
                    }
                    break;
                }
        }

        //if (interval > 0)
        //    return;

        //SkillBase skill = GetAvailableSkill();
        //if (skill == null)
        //    return;

        //switch (mode)
        //{
        //    case Mode.MOVE:
        //        {
        //            if (targetUnit != null)
        //            {
        //                i
        //            }


        //            성까지 거리 계산
        //            float distance = Vector3.Distance(transform.position, fortressPos);

        //            공격 우선 유닛
        //            if (mission == Mission.FIGHTER)
        //            {
        //                targetUnit = SearchTarget(UnitStats.sightRange);
        //                if (targetUnit != null)
        //                {
        //                    mode = Mode.COMBAT;
        //                    return;
        //                }
        //            }

        //            공격범위 내면 성 공격
        //            if (distance <= UnitStats.attackRange)
        //            {
        //                mode = Mode.ATTACKFORTRESS;
        //                return;
        //            }

        //            Vector3 start = transform.position;
        //            if (NavMesh.SamplePosition(start, out NavMeshHit hit, 5.0f, navAgent.areaMask))
        //            {
        //                start = hit.position;
        //            }
        //            bool pathState = NavMesh.CalculatePath(start, fortressPos, navAgent.areaMask, path);

        //            bool pathState = NavMesh.CalculatePath(transform.position, fortressPos, navAgent.areaMask, path);

        //            if (pathState)
        //            {
        //                navAgent.isStopped = false;
        //                navAgent.SetPath(path);
        //                if (path.status == NavMeshPathStatus.PathPartial) // 막혀 있음
        //                {
        //                    targetUnit = SearchTarget(UnitStats.sightRange);
        //                    if (targetUnit != null)
        //                        mode = Mode.COMBAT;
        //                    else
        //                        ForceMoveTo(fortressPos);
        //                }
        //            }
        //            else
        //            {
        //                targetUnit = SearchTarget(UnitStats.sightRange);
        //                if (targetUnit != null)
        //                    mode = Mode.COMBAT;
        //            }
        //        }
        //        break;
        //    case Mode.COMBAT:
        //        {
        //            if (targetUnit.HpPercent > 0f || targetUnit.gameObject.activeInHierarchy)
        //            {

        //                if (IsTargetInAttackRange(targetUnit, UnitStats.attackRange)) // 공격 사거리 내 -> 공격
        //                {
        //                    if (navAgent.enabled && !navAgent.isStopped)
        //                    {
        //                        navAgent.isStopped = true;
        //                        modelAnimator.SetBool("isRunning", false);
        //                    }


        //                    if (interval <= 0)
        //                    {

        //                        if (skill != null)
        //                        {
        //                            ActivateSkill(skill, targetUnit);
        //                        }


        //                    }

        //                }
        //                else if (IsTargetInRange(targetUnit, UnitStats.sightRange)) // 시야 거리 내 -> 이동
        //                {
        //                    MoveTo(path);


        //                    if (path.status != NavMeshPathStatus.PathComplete)
        //                    {
        //                        Debug.Log(111);

        //                        targetUnit = SearchTarget(UnitStats.sightRange);
        //                        if (targetUnit == null)
        //                        {
        //                            mode = Mode.MOVE;
        //                            MoveTo(fortressPos);
        //                        }
        //                    }
        //                }
        //                else // 시야 사거리 밖
        //                {
        //                    targetUnit = null;
        //                    hasTargetPos = false;
        //                    mode = Mode.MOVE;
        //                    MoveTo(fortressPos);
        //                }
        //            }
        //            else
        //            {
        //                targetUnit = null;
        //                mode = Mode.MOVE;
        //                MoveTo(fortressPos);
        //            }
        //        }
        //        break;
        //    case Mode.ATTACKFORTRESS:
        //        {
        //            switch (mission)
        //            {
        //                case Mission.FIGHTER:
        //                    {
        //                        SearchReachableTarget(unitStats.sightRange, enemyLayer);
        //                        if (targets.Count > 0)
        //                        {
        //                            if (interval <= 0f)
        //                            {
        //                                targetUnit = targetUnit = SearchTarget(skill.Data.Range, enemyLayer, skill);    // 스킬 범위내 대상 먼저 검색
        //                                if (targetUnit != null)
        //                                {
        //                                    mode = Mode.COMBAT;
        //                                }
        //                                else
        //                                {
        //                                    targetUnit = SearchTargetInTargets(skill); // 시야 내로 다시 검사
        //                                    if (targetUnit != null)
        //                                    {
        //                                        mode = Mode.MOVE;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            SkillBase GeneralSkill = GetGeneralSkill();

        //                            if (GeneralSkill != null)
        //                                ActivateSkill(fortress, data);
        //                        }

        //                        break;
        //                    }
        //                case Mission.SIEGE:
        //                    {
        //                        SkillBase GeneralSkill = GetGeneralSkill();

        //                        if (GeneralSkill != null)
        //                            ActivateSkill(fortress, data);

        //                        break;
        //                    }
        //            }
        //        }
        //        break;
        //}
    }

    private  void UpdateSkillState(SkillBase skill, Unit target)
    {
        if (skill == GeneralSkill)
        {
            state = State.GENERALSKILL;
            PlayAnimation("GeneralSkill");
            //modelAnimator.SetTrigger("GeneralSkill");
        }
        else if(skill == SpecialSkill)
        {
            state = State.SPECIALSKILL;
            PlayAnimation("SpecialSkill");
            //modelAnimator.SetTrigger("SpecialSkill");
        }

        isSkillActive = true;

        if (target != this)
            transform.LookAt(target.transform);

        navAgent.isStopped = true;

        //base.ActivateSkill(skill, target);
        interval = intervalCheck;
        currentSkill = null;
    }

    protected void ActivateSkill(Fortress fortress, UnitData data)  // 성 공격 상태
    {

        state = State.FORTRESSSKILL;
        PlayAnimation("GeneralSkill");
        //modelAnimator.SetTrigger("GeneralSkill");

        transform.LookAt(fortress.transform.position);
        isSkillActive = true;

        if (navAgent.enabled)
        {
            navAgent.isStopped = true;
        }
    }

    private void ActivateFortressSkil()
    {
        base.GeneralSkill.Activate(this, fortress);
        isSkillActive = false;
    }

    public override void TakeDamage(float Damage)
    {
        base.TakeDamage(Damage);

        //if (HpPercent * 100f <= angerTriggerPercent && !isDead)
        //{
        //    if (aiStance == AIStance.AGGRESSIVE && behaviorPriority != BehaviorPriority.Combat)
        //    {
        //        behaviorPriority = BehaviorPriority.Combat;
        //        //modelAnimator.SetTrigger("Rage");
        //        AddVFX(WarCryVFX.GetComponent<ParticleSystem>());
        //    }
        //}

    }

    //public override void GetProvoked(Unit ProvokedTarget)
    //{
    //    Debug.Log(gameObject.name + " Has Provoked to " + ProvokedTarget.name);
    //    mode = Mode.COMBAT;
    //    targetUnit = ProvokedTarget;
    //}

    public override void RemoveProvoked()
    {
        mode = Mode.MOVE;
        targetUnit = null;
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
            state = State.IDLE;
            navAgent.isStopped = false;
            //if (mode == Mode.MOVE)
            //{
            //    modelAnimator.SetBool("isRunning", true);
            //    
            //}


        }
        //    state = State.IDLE;

        //if (navAgent.enabled)
        //    navAgent.isStopped = false;
    }


    public override void Die()
    {
        if (isDead) return;

        isDead = true;

        if (state == State.STUN)
        {
            base.RemoveStun();
        }

        if(hasExecutedMark)
        {
            hasExecutedMark = false;
            executionEffect.OnTargetDead();
            executionEffect = null;
        }

        isFortressPathBlocked = false;
        hasTargetPos = false;
        state = State.DEAD;

        base.Die();



        SoundManager.Instance.PlaySFX(this.transform.position, enemyDeadSFX);

        // 상태를 변경하고 에니메이션을 변경

        AddVFX(UnitDeathVFX.GetComponent<ParticleSystem>());
        AddVFX(CoinDropVFX.GetComponent<ParticleSystem>());

        enemySpawner.OnEnemyDead(data, this);

    }

    public override void MoveToTargetUnit(Unit target)
    {
        if(target is AllyUnit)
        {
            AllyUnit allyUnit = target as AllyUnit;

            if (allyUnit.ModeType == AllyUnit.Mode.SEIGE)
            {
                if (hasTargetPos)
                {
                    return;
                }
                else
                {
                    Vector3 direction = (transform.position - target.transform.position).normalized;
                    targetPos = target.transform.GetNearPosition(direction, NearbyDistance);
                    hasTargetPos = true;

                    if (navAgent.isStopped)
                        navAgent.isStopped = false;


                    navAgent.SetDestination(target.transform.position);
                }
            }
            else if (allyUnit.ModeType == AllyUnit.Mode.FREE)
            {
                base.MoveToTargetUnit(target);
            }
        }
        else if(target is EnemyUnit)
        {
            base.MoveToTargetUnit(target);
        }


    }


    public  Unit SearchNearestTarget(float range)
    {

        Unit result = null;
        float nearest = float.MaxValue;

        NavMeshPath bestPath = null;

        int count = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);

        for (int i = 0; i < count; i++)
        {
            Unit target = collidersInRange[i].GetComponent<Unit>();
            if (target.IsDead || !target.gameObject.activeInHierarchy)
                continue;

            Vector3 pos = target.transform.position;

            bool onNav = NavMesh.SamplePosition(pos, out NavMeshHit hit, 0.1f, navAgent.areaMask);

            if (onNav)
            {
                NavMeshPath temp = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, hit.position, navAgent.areaMask, temp);

                if (temp.status == NavMeshPathStatus.PathComplete)
                {
                    float len = CalculatePathLength(temp);
                    if (len < nearest)
                    {
                        nearest = len;
                        result = target;
                        bestPath = temp;  
                    }
                }
            }
            else
            {
                Vector3 direction = (transform.position - pos).normalized;

                for (int j = 0; j < 6; j++)
                {
                    Vector3 dir = Quaternion.AngleAxis(60f * j, Vector3.up) * direction;
                    Vector3 cand = target.transform.GetNearPosition(dir, NearbyDistance);

                    if (!NavMesh.SamplePosition(cand, out NavMeshHit hit2, 0.5f, navAgent.areaMask))
                        continue;

                    NavMeshPath temp = new NavMeshPath();
                    NavMesh.CalculatePath(transform.position, hit2.position, navAgent.areaMask, temp);
                    //Debug.Log(hit2.position);

                    if (temp.status != NavMeshPathStatus.PathComplete)
                        continue;

                    float len = CalculatePathLength(temp);
                    if (len < nearest)
                    {
                        nearest = len;
                        result = target;
                        bestPath = temp;
                        //Debug.Log(11111111);
                    }
                }
            }
        }

        if (bestPath != null && bestPath.status == NavMeshPathStatus.PathComplete)
        {
            path = bestPath;
        }

        return result;
    }
}
