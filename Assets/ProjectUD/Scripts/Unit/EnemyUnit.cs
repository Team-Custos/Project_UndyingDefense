using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : Unit
{
    private enum Mode
    {
        MOVE,
        COMBAT,
        ATTACKFORTRESS,
        STUN
    }

    private enum State
    {
        IDLE,
        BATTLECRY,
        RUN,
        GENERALSKILL,
        SPECIALSKILL,
        STUN,
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

    private EnemyUnitData data;
    private ObjectPoolWithList<EnemyUnit> pool;
    private EnemyUnitSpawner enemySpawner;

    private Mode mode;
    private State state;
    private AIStance aiStance;

    private Fortress fortress;
    private Vector3 fortressPos;

    private bool hasExecutedMark = false;

    public bool HasExecuteMark => hasExecutedMark;

    public override UnitData Data => data;

    private float attackCool;

    protected static AudioClip[] enemyDeadSFX;

    protected static GameObject coinDropVFX;

    protected static AudioClip[] EnemyDeadSFX
    {
        get
        {
            if (enemyDeadSFX == null)
            {
                enemyDeadSFX = Resources.LoadAll<AudioClip>("Sound/SFX/효과음/캐릭터/DeathSFX/EnemyDeath");
            }
            return enemyDeadSFX;
        }
    }

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

    public void SetExecuted(bool Executed)
    {
        hasExecutedMark = Executed;
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
        state = State.BATTLECRY;
        isDead = false;
        aiStance = data.aiStance;
    }

    protected override void Update()
    {
        base.Update();

        switch(state)
        {
            case State.STUN:
                //{
                //    return;
                //}
            case State.GENERALSKILL:
            case State.SPECIALSKILL:
            case State.BATTLECRY:
            case State.DEAD:
                {
                    if (state != State.DEAD)
                    {
                        if (navAgent.enabled)
                        {
                            navAgent.enabled = false;
                            modelAnimator.SetBool("isRunning", false);
                        }

                        if (!navObstacle.enabled)
                            navObstacle.enabled = true;
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
                            gameObject.SetActive(false);

                        state = State.IDLE;

                        navObstacle.enabled = false;
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
                        if (navAgent.enabled)
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
    }

    private void UpdateMode()
    {
        if (state == State.STUN)
            return;

        switch (mode)
        {
            case Mode.STUN:
                break;
            case Mode.MOVE:
                {
                    float distance = Vector3.Distance(transform.position, fortressPos);

                    if(distance <= data.AttackRange)
                    {
                        mode = Mode.ATTACKFORTRESS;
                        return;
                    }

                    if (navAgent.pathStatus != NavMeshPathStatus.PathComplete)
                    {
                        if (distance <= data.AttackRange)
                        {
                            mode = Mode.ATTACKFORTRESS;
                        }
                        else
                        {
                            if (navAgent.enabled)
                            {
                                targetUnit = SearchTarget(data.SightRange);
                            }
                            else
                            {
                                navObstacle.enabled = false;
                                navAgent.enabled = true;

                                targetUnit = SearchTarget(data.SightRange);

                                navAgent.enabled = false;
                                navObstacle.enabled = true;
                            }
                        }

                        if (targetUnit != null)
                            mode = Mode.COMBAT;
                        else
                            ForceMoveTo(fortressPos);
                    }
                    else
                    {
                        int length = path.corners.Length;
                        if (length > 0)
                        {
                            if (Vector3.Distance(transform.position, path.corners[length - 1]) <= navAgent.stoppingDistance)
                            {
                                if (navAgent.enabled && !navAgent.isStopped)
                                {
                                    navAgent.isStopped = true;
                                    modelAnimator.SetBool("isRunning", false);
                                    transform.LookAt(fortress.transform.position);
                                    mode = Mode.ATTACKFORTRESS;
                                    return;
                                }
                            }
                        }  

                        if (!navAgent.enabled)
                            MoveTo(fortressPos);
                    }
                }
                break;
            case Mode.COMBAT:
                {
                    if (targetUnit.HpPercent > 0f || !targetUnit.gameObject.activeInHierarchy)
                    {
                        if (IsTargetInRange(targetUnit, Data.AttackRange)) // 공격 사거리 내
                        {
                            if (navAgent.enabled && !navAgent.isStopped)
                            {
                                navAgent.isStopped = true;
                                modelAnimator.SetBool("isRunning", false);
                            }

                            // 스킬 관련 처리
                            SkillBase skill = GetAvailableSkill();
                            if (skill != null)
                            {
                                ActivateSkill(skill, targetUnit);
                            }
                        }
                        else if (IsTargetInRange(targetUnit, Data.SightRange)) // 공격 사거리 < 대상 < 시야 사거리
                        {
                            MoveTo(targetUnit); 
                            if(path.status != NavMeshPathStatus.PathComplete)
                            {
                                targetUnit = SearchTarget(data.SightRange);
                                if (targetUnit == null)
                                {
                                    mode = Mode.MOVE;
                                    MoveTo(fortressPos);
                                }
                            }
                        }
                        else // 시야 사거리 밖
                        {
                            targetUnit = null;
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
                    if (aiStance == AIStance.AGGRESSIVE)
                    {
                        targetUnit = SearchTarget(data.SightRange);
                        if (targetUnit != null)
                        {
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

        base.ActivateSkill(skill, target);
    }

    protected void ActivateSkill(Fortress fortress, UnitData data)  // 성 공격 스킬
    {
        transform.LookAt(fortress.transform.position);

        if (navAgent.enabled)
        {
            navAgent.isStopped = true;
        }


        attackCool -= Time.deltaTime;
        if(attackCool <= 0f)
        {
            modelAnimator.SetTrigger("GeneralSkill");
            fortress.TakeDamage(data.Tier);
            attackCool = base.GeneralSkill.Data.CoolTime;
        }
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
        base.GetStun();
        state = State.STUN;
        mode = Mode.STUN;
    }

    public override void RemoveStun()
    {
        base.RemoveStun();
        state = State.IDLE;
        mode = Mode.MOVE;
        ForceMoveTo(fortressPos);
    }


    public override void Die()
    {
        if (!isDead)
        {
            //navAgent.enabled = false;
            //navObstacle.enabled = false;
            //collider.enabled = false;

            base.Die();

            if(EnemyDeadSFX.Length > 0)
            {
                AudioClip clip = EnemyDeadSFX[Random.Range(0, EnemyDeadSFX.Length)];
                SoundManager.Instance.PlaySFX(clip);
            }

            // 상태를 변경하고 에니메이션을 변경
            state = State.DEAD;
            //modelAnimator.SetTrigger("Die");
            AddVFX(UnitDeathVFX.GetComponent<ParticleSystem>());
            AddVFX(CoinDropVFX.GetComponent<ParticleSystem>());

            enemySpawner.OnEnemyDead(data);

            isDead = true;
        }

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
