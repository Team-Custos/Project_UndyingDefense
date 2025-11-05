using System.Collections.Generic;
using System.Resources;
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

    private enum BehaviorPriority
    {
        Move,
        Combat,
    }

    private EnemyUnitData data;
    private ObjectPoolWithList<EnemyUnit> pool;
    private EnemyUnitSpawner enemySpawner;
    private WaveManager waveManager;

    private Mode mode;
    private State state;
    private AIStance aiStance;
    [SerializeField] private BehaviorPriority behaviorPriority;

    private Fortress fortress;
    private Vector3 fortressPos;

    private const float angerTriggerPercent = 100f; // 분노 발동 기준 퍼센트

    private bool hasExecutedMark = false;
    private ExecutionEffect executionEffect;

    public bool HasExecuteMark => hasExecutedMark;

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
        }
        else
        {
            this.executionEffect = executionEffect;
            effect.transform.SetParent(effectParent);
            effect.transform.position = heightPos.position;
            effect.SetActive(true);
            executionEffect.ActivateExecution();
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
        aiStance = data.aiStance;
        mode = Mode.MOVE;
        //behaviorPriority = BehaviorPriority.Move;
        navAgent.avoidancePriority = 1;

        fortressAttackCool = GeneralSkill.Data.CoolTime;
    }

    public void SetWaveManager(WaveManager waveManager)
    {
        this.waveManager = waveManager;
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

                    if(state == State.FORTRESSSKILL)
                    {
                        SkillBase skill = GetGeneralSkill();

                        if (skill != null)
                        {
                            if (stateDurationCheck >= skill.AnimationStateTime)
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
        //if (state == State.STUN)
        //    return;

        switch (mode)
        {
            case Mode.MOVE:
                {
                    // 성까지 거리 계산
                    float distance = Vector3.Distance(transform.position, fortressPos);

                    // 공격 우선 유닛
                    if (behaviorPriority == BehaviorPriority.Combat)
                    {
                        targetUnit = SearchTarget(UnitStats.sightRange);
                        if (targetUnit != null)
                        {
                            mode = Mode.COMBAT;
                            return;
                        }
                    }

                    // 공격범위 내면 성 공격
                    if (distance <= UnitStats.attackRange)
                    {
                        mode = Mode.ATTACKFORTRESS;
                        return;
                    }

                    //Vector3 start = transform.position;
                    //if (NavMesh.SamplePosition(start, out NavMeshHit hit, 5.0f, navAgent.areaMask))
                    //{
                    //    start = hit.position;
                    //}
                    //bool pathState = NavMesh.CalculatePath(start, fortressPos, navAgent.areaMask, path);

                    bool pathState = NavMesh.CalculatePath(transform.position, fortressPos, navAgent.areaMask, path);

                    if (pathState)
                    {
                        navAgent.isStopped = false;
                        navAgent.SetPath(path);
                        if (path.status == NavMeshPathStatus.PathPartial) // 막혀 있음
                        {
                            targetUnit = SearchTarget(UnitStats.sightRange);
                            if (targetUnit != null)
                                mode = Mode.COMBAT;
                            else
                                ForceMoveTo(fortressPos);
                        }
                    }
                    else
                    {
                        targetUnit = SearchTarget(UnitStats.sightRange);
                        if (targetUnit != null)
                            mode = Mode.COMBAT;
                    }
                }
                break;
            case Mode.COMBAT:
                {
                    if (targetUnit.HpPercent > 0f || targetUnit.gameObject.activeInHierarchy)
                    {
                        if (IsTargetInAttackRange(targetUnit, UnitStats.attackRange)) // 공격 사거리 내 -> 공격
                        {
                            if (navAgent.enabled && !navAgent.isStopped)
                            {
                                navAgent.isStopped = true;
                                modelAnimator.SetBool("isRunning", false);
                            }


                            if (interval <= 0)
                            {
                                // 스킬 관련 처리
                                SkillBase skill = GetAvailableSkill();

                                if (skill != null)
                                {
                                    ActivateSkill(skill, targetUnit);
                                }

                                
                            }
                            
                        }
                        else if (IsTargetInRange(targetUnit, UnitStats.sightRange)) // 시야 거리 내 -> 이동
                        {
                            MoveTo(targetUnit);

                            //if (path.status != NavMeshPathStatus.PathComplete)
                            //{
                            //    Debug.Log(111);

                            //    targetUnit = SearchTarget(UnitStats.sightRange);
                            //    if (targetUnit == null)
                            //    {
                            //        mode = Mode.MOVE;
                            //        MoveTo(fortressPos);
                            //    }
                            //}
                        }
                        else // 시야 사거리 밖
                        {
                            targetUnit = null;
                            hasTargetPos = false;
                            mode = Mode.MOVE;
                            MoveTo(fortressPos);
                        }
                    }
                    else
                    {
                        targetUnit = null;
                        mode = Mode.MOVE;
                        MoveTo(fortressPos);
                    }
                }
                break;
            case Mode.ATTACKFORTRESS:
                {
                    if (behaviorPriority == BehaviorPriority.Combat)
                    {
                        targetUnit = SearchTarget(UnitStats.sightRange);
                        if (targetUnit != null)
                        {
                            state = State.IDLE;
                            mode = Mode.COMBAT;
                            MoveTo(targetUnit);
                        }
                        else
                        {
                            ActivateSkill(fortress, data);
                        }
                    }
                    else
                    {
                        ActivateSkill(fortress, data);
                    }
                }
                break;
        }
    }

    protected override void ActivateSkill(SkillBase skill, Unit target) // 적 공격 스킬
    {
        if (skill == GeneralSkill)
        {
            state = State.GENERALSKILL;
            modelAnimator.SetTrigger("GeneralSkill");
        }
        else if(skill == SpecialSkill)
        {
            state = State.SPECIALSKILL;
            modelAnimator.SetTrigger("SpecialSkill");
        }

        if (target != this)
            transform.LookAt(target.transform);

        //base.ActivateSkill(skill, target);
        interval = intervalCheck;
    }

    protected void ActivateSkill(Fortress fortress, UnitData data)  // 성 공격 스킬
    {
        state = State.FORTRESSSKILL;
        modelAnimator.SetTrigger("GeneralSkill");

        transform.LookAt(fortress.transform.position);

        if (navAgent.enabled)
        {
            navAgent.isStopped = true;
        }
    }

    private void ActivateFortressSkil()
    {
        base.GeneralSkill.Activate(this, fortress);
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

    public override void GetProvoked(Unit ProvokedTarget)
    {
        Debug.Log(gameObject.name + " Has Provoked to " + ProvokedTarget.name);
        mode = Mode.COMBAT;
        targetUnit = ProvokedTarget;
    }

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

            //if (mode == Mode.MOVE)
            //{
            //    modelAnimator.SetBool("isRunning", true);
            //    navAgent.isStopped = false;
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

        state = State.DEAD;

        base.Die();



        SoundManager.Instance.PlaySFX(this.transform.position, enemyDeadSFX);

        // 상태를 변경하고 에니메이션을 변경

        AddVFX(UnitDeathVFX.GetComponent<ParticleSystem>());
        AddVFX(CoinDropVFX.GetComponent<ParticleSystem>());

        enemySpawner.OnEnemyDead(data, this);

    }

    private Unit SearchTarget(float range)
    {
        Unit result = null;
        switch (data.targetingType)
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

    //private void Update()
    //{
    //    //if (pathUpdateTimeCheck < pathUpdateTime)
    //    //{
    //    //    pathUpdateTimeCheck += Time.deltaTime;
    //    //}
    //    //else
    //    //{
    //    //    pathUpdateTimeCheck -= pathUpdateTime;

    //    //    if (navAgent.CalculatePath(fortress.Position, path))
    //    //    {
    //    //        navAgent.SetPath(path);
    //    //    }
    //    //    else
    //    //    {
    //    //        Debug.Log(path.status);
    //    //        Debug.Log("경로가 막힘");
    //    //    }
    //    //}

    //    ////text1.text = $"현재 속도 : {navAgent.velocity}";
    //    ////text2.text = $"가려고 하는 속도 : {navAgent.desiredVelocity}";
    //    //float speedDiff = navAgent.desiredVelocity.magnitude - navAgent.velocity.magnitude;
    //    //text1.text = $"차이 : {navAgent.desiredVelocity.magnitude - navAgent.velocity.magnitude}";

    //    //if (navAgent.velocity.magnitude < 0.01f && navAgent.velocity.magnitude > 0f && speedDiff >= 0.01f)
    //    //{
    //    //    Debug.Log("막힘");
    //    //    text2.text = "막힘";
    //    //}
    //    //else
    //    //{
    //    //    text2.text = "뚫림";
    //    //}
    //}
}
