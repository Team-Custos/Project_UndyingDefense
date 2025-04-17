using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

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
        DEAD
    }

    private EnemyUnitData data;
    private ObjectPoolWithList<EnemyUnit> pool;
    private EnemyUnitSpawner enemySpawner;

    private Mode mode;
    private State state;

    private Fortress fortress;
    private Vector3 fortressPos;

    public override UnitData Data => data;

    private float attackCool;

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
    }

    protected override void Update()
    {
        base.Update();

        switch(state)
        {
            case State.GENERALSKILL:
            case State.SPECIALSKILL:
            case State.STUN:
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
        switch (mode)
        {
            case Mode.MOVE:
                {
                    if (navAgent.pathStatus != NavMeshPathStatus.PathComplete) // 길이 막혀있는 경우
                    {
                        float distance = Vector3.Distance(transform.position, fortressPos);

                        if(distance <= data.AttackRange)
                        {
                            ActivateSkill(fortress, data);
                        }
                        else
                        {
                            if (navAgent.enabled)
                            {
                                targetUnit = SearchNearestReachableTarget(data.SightRange);
                            }
                            else
                            {
                                navObstacle.enabled = false;
                                navAgent.enabled = true;

                                targetUnit = SearchNearestReachableTarget(data.SightRange);

                                navAgent.enabled = false;
                                navObstacle.enabled = true;
                            }
                        }

                        //if (navAgent.enabled)
                        //{
                        //    targetUnit = SearchNearestReachableTarget(data.SightRange);
                        //}
                        //else
                        //{
                        //    navObstacle.enabled = false;
                        //    navAgent.enabled = true;

                        //    targetUnit = SearchNearestReachableTarget(data.SightRange);

                        //    navAgent.enabled = false;
                        //    navObstacle.enabled = true;
                        //}
                        
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
                                targetUnit = SearchNearestReachableTarget(data.SightRange);
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
                    ActivateSkill(fortress, data);
                }
                break;
        }
    }

    protected override void ActivateSkill(SkillBase skill, Unit target) // 적 공격 스킬
    {
        if (skill == generalSkill)
        {
            state = State.GENERALSKILL;
            modelAnimator.SetTrigger("GeneralSkill");
        }
        else if(skill == specialSkill)
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

        attackCool -= Time.deltaTime;
        if(attackCool <= 0f)
        {
            modelAnimator.SetTrigger("GeneralSkill");
            fortress.TakeDamage(data.Tier);
            attackCool = base.generalSkill.Data.CoolTime;
        }


        //if (skill == generalSkill)
        //{
        //    state = State.GENERALSKILL;
        //    modelAnimator.SetTrigger("GeneralSkill");
        //}
        //else if (skill == specialSkill)
        //{
        //    state = State.SPECIALSKILL;
        //    modelAnimator.SetTrigger("SpecialSkill");
        //}
        //transform.LookAt(fortress.transform.position);
        //skill.Activate(this, fortress);
    }


    public override void Die()
    {
        if (!isDead)
        {
            navAgent.enabled = false;
            navObstacle.enabled = false;
            collider.enabled = false;

            // 상태를 변경하고 에니메이션을 변경
            state = State.DEAD;
            modelAnimator.SetTrigger("Die");

            enemySpawner.OnEnemyDead(data);

            isDead = true;
        }

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
