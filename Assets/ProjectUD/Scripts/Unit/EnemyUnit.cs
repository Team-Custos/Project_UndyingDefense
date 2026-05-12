using System.Collections.Generic;
using System.Data;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    //private Vector3 targetPos;
    //[SerializeField] private  bool hasTargetPos = false;

    private const float angerTriggerPercent = 100f; // 분노 발동 기준 퍼센트

    private ExecutionEffect executionEffect;
    private bool hasExecutionMark = false;
    public bool HasExecutionMark => hasExecutionMark;

    public override UnitData Data => data;


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

    public void SetExecution(ExecutionEffect executionEffect, bool executed, GameObject effect)
    {
        hasExecutionMark = executed;

        if(!executed)   // 제거
        {
            effect.SetActive(false);
            this.executionEffect = null;
            hasExecutionMark = false;
        }
        else
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, 20f, collidersInRange, enemyLayer);
            if(count > 0)
            {
                Debug.Log($"척살 명령 적용 수 : {count}");
                for (int i = 0; i < count; i++)
                {
                    AllyUnit target = collidersInRange[i].GetComponent<AllyUnit>();
                    if (target.IsDead || !target.gameObject.activeInHierarchy)
                        continue;

                    //target.SetTargetUnit(this);
                    target.SetExecutionUnit(this);
                }
            }


            this.executionEffect = executionEffect;
            effect.transform.SetParent(effectParent);
            effect.transform.position = heightPos.position + Vector3.up * 1.5f;
            effect.SetActive(true);
            executionEffect.ActivateExecution();
            hasExecutionMark = true;
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
        //hasTargetPos = false;

    }



    protected override void Update()
    {
        //base.Update();

        if (isStop)
            return;
            

        interval -= Time.deltaTime;

        if (isDeferredState)
        {
            deferredStateDurationCheck -= Time.deltaTime;
            if (deferredStateDurationCheck <= 0f)
            {
                targetUnit = null;
                isDeferredState = false;
                deferredStateDurationCheck = deferredStateDuration;
                deferredStateObj.SetActive(false);
            }
        }

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
                                if (skillTargetType == SkillBase.TargetType.ALLY ||
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
        if (isDeferredState)
            return;


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

                    NavMesh.CalculatePath(transform.position, fortressPos, navAgent.areaMask, path);
                    if (path.status != NavMeshPathStatus.PathComplete)
                    {   // 길막힘

                        SearchReachableTargets(unitStats.sightRange, enemyLayer);
                        if (targets.Count > 0)
                        {
                            if (interval <= 0f && currentSkill == null)     // 인터벌 중이 아니고, 보유 스킬이 없는 경우 스킬 선택
                            {
                                currentSkill = GetAvailableSkill();
                            }

                             if (currentSkill != null)
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
                                    case SkillBase.TargetType.ALLY:     // 탐색 -> 스킬 발동 or 이동
                                         {
                                             if (targetUnit is AllyUnit)
                                                 targetUnit = null;  // 공격 스킬 대상 초기화

                                             if (IsTargetValid(targetUnit, allyLayer)) // 기존 대상 유효
                                             {
                                                 if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
                                                 {
                                                     UpdateSkillState(currentSkill, targetUnit);
                                                 }
                                                 else
                                                 {
                                                     if (!IsPathBlocked(targetUnit))
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
                                                SearchReachableTargets(unitStats.sightRange, allyLayer);
                                                targetUnit = SearchTargetInTargets(currentSkill);
                                                //hasTargetPos = false;

                                                if (targetUnit != null)
                                                 {
                                                     if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
                                                     {
                                                         UpdateSkillState(currentSkill, targetUnit);
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
                                             if (IsTargetValid(targetUnit, enemyLayer))
                                             {
                                                 if (!IsTargetInRange(targetUnit, unitStats.sightRange))
                                                 {
                                                     // 범위 밖에 있음 -> 판단 유예 상태로
                                                     SetDeferredState();
                                                    Debug.Log("시야 범위 밖");
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
                                                        Debug.Log("이동 불가");
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
                                                //hasTargetPos = false;
                                                //SearchReachableTargets(unitStats.sightRange, enemyLayer); // 이동 가능한 대상 탐색
                                                targetUnit = SearchTargetInTargets(currentSkill);
                                                if (targetUnit != null)
                                                {
                                                    if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
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
                                                    ForceMoveTo(fortressPos);
                                                    //hasTargetPos = false;
                                                }

                                             }

                                             break;
                                         }
                                 }
                             }
                            
                        }
                        else
                        {
                            ForceMoveTo(fortressPos);
                        }
                    }
                    else
                    {   // 길 뚫림
                        ForceMoveTo(fortressPos);
                        targetUnit = null;
                    }

                    
                    break;
                }
            case Mission.FIGHTER:
                {
                    SearchReachableTargets(unitStats.sightRange, enemyLayer);
                    if (targets.Count > 0)
                    {
                        if (interval <= 0f && currentSkill == null)     // 인터벌 중이 아니고, 보유 스킬이 없는 경우 스킬 선택
                        {
                            currentSkill = GetAvailableSkill();
                        }

                         if (currentSkill != null)
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
                                case SkillBase.TargetType.ALLY:     // 탐색 -> 스킬 발동 or 이동
                                     {
                                         if (targetUnit is AllyUnit)
                                             targetUnit = null;  // 공격 스킬 대상 초기화

                                         if (IsTargetValid(targetUnit, allyLayer)) // 기존 대상 유효
                                         {
                                             if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
                                             {
                                                 UpdateSkillState(currentSkill, targetUnit);
                                             }
                                             else
                                             {
                                                 if (!IsPathBlocked(targetUnit))
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
                                             SearchReachableTargets(unitStats.sightRange, allyLayer);
                                             targetUnit = SearchTargetInTargets(currentSkill);
                                             //hasTargetPos = false;

                                            if (targetUnit != null)
                                             {
                                                 if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
                                                 {
                                                     UpdateSkillState(currentSkill, targetUnit);
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
                                         if (IsTargetValid(targetUnit, enemyLayer))
                                         {
                                             if (!IsTargetInRange(targetUnit, unitStats.sightRange))
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
                                            //hasTargetPos = false;
                                            SearchReachableTargets(unitStats.sightRange, enemyLayer); // 이동 가능한 대상 탐색
                                            targetUnit = SearchTargetInTargets(currentSkill);
                                             if (targetUnit != null)
                                             {
                                                 if (IsTargetInAttackRange(targetUnit, currentSkill.Data.Range))
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
                                                 ForceMoveTo(fortressPos);
                                             }

                                         }

                                         break;
                                     }
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


                    break;
                }
        }

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
        //hasTargetPos = false;
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

        if(hasExecutionMark)
        {
            hasExecutionMark = false;
            executionEffect.OnTargetDead();
            executionEffect = null;
        }

        //hasTargetPos = false;
        state = State.DEAD;

        base.Die();



        SoundManager.Instance.PlaySFX(this.transform.position, enemyDeadSFX);

        // 상태를 변경하고 에니메이션을 변경

        AddVFX(UnitDeathVFX.GetComponent<ParticleSystem>());
        AddVFX(CoinDropVFX.GetComponent<ParticleSystem>());

        enemySpawner.OnEnemyDead(data, this);

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

    public override void SetDeferredState()
    {
        base.SetDeferredState();
        state = State.IDLE;
        //hasTargetPos = false;
    }
}
