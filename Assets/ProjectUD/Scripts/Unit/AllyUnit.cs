using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.AI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class AllyUnit : Unit
{
    public enum Mode
    {
        FREE,
        SEIGE,
        CHANGE
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

    private AllyUnitData data;
    private ObjectPoolWithList<AllyUnit> pool;
    [SerializeField] private Mode mode;
    
    private State state;

    private float changeDuration = 3.0f;
    private Mode previousMode;

    public override UnitData Data => data;

    [SerializeField] private GameObject siegeEffect;
    [SerializeField] private ParticleSystem siegeParticle;

    private bool isSiegeModeInitialized = false;

    [SerializeField] private GameObject chagneEffet;

    private AllyUnitSpawner spawner;

    public Transform destinaitonTransfrom;

    public bool isSelected = false;


    public override void Initialize()
    {
        base.Initialize();
        mode = Mode.SEIGE;
        //mode = Mode.FREE;
        previousMode = mode;
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
            case State.GENERALSKILL:
            case State.SPECIALSKILL:
            case State.STUN:
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
                    if(mode == Mode.CHANGE)
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

        
        //SkillBase skill = GetAvailableSkill();
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
    }

    private void UpdateMode()
    {
        switch (mode)
        {
            case Mode.SEIGE:
                {
                    if (!isSiegeModeInitialized)
                    {
                        siegeParticle.gameObject.SetActive(true);
                        siegeParticle.Play();
                        
                        isSiegeModeInitialized = true;
                    }

                    SkillBase skill = GetAvailableSkill();
                    if (skill != null) // 사용 가능한 스킬이 존재할 경우
                    {
                        SkillBase.TargetType skillTargetType = skill.GetTargetType(); // 스킬 대상 종류 확인
                        switch (skillTargetType)
                        {
                            case SkillBase.TargetType.ENEMY:
                                {
                                    if(targetUnit != null && targetUnit.HpPercent > 0f && targetUnit.gameObject.activeInHierarchy)
                                        if (IsTargetInRange(targetUnit, data.AttackRange))
                                        {
                                            if (navAgent.enabled && !navAgent.isStopped)
                                            {
                                                navAgent.isStopped = true;
                                                modelAnimator.SetBool("isRunning", false);
                                            }

                                            ActivateSkill(skill, targetUnit);
                                        }
                                        else
                                            targetUnit = null;
                                    else
                                        targetUnit = SearchTarget(data.SightRange);
                                }
                                break;
                            case SkillBase.TargetType.ALLY:
                                {
                                    ActivateSkill(skill, null);
                                }

                                break;
                        }
                    }
                    else
                    {
                        if (targetUnit == null || targetUnit.HpPercent <= 0f || !targetUnit.gameObject.activeInHierarchy)
                            targetUnit = SearchTarget(data.SightRange);
                        else
                            LookAt(targetUnit.transform.position);   
                    }

                    siegeEffect.SetActive(true);
                    chagneEffet.SetActive(false);
                }
                break;
            case Mode.FREE:
                {
                    if(isSelected && destinaitonTransfrom != null)
                    {
                        Debug.Log(destinaitonTransfrom.transform);

                        targetUnit = null;

                        navAgent.SetDestination(destinaitonTransfrom.position);
                        modelAnimator.SetBool("isRunning", true);

                        float distance = Vector3.Distance(transform.position, destinaitonTransfrom.position);

                        if (distance < 0.1f)
                        {
                            destinaitonTransfrom = null;
                            modelAnimator.SetBool("isRunning", false);
                        }
                    }

                    if (targetUnit != null && targetUnit.HpPercent > 0f && targetUnit.gameObject.activeInHierarchy)
                    {
                        SkillBase skill = GetAvailableSkill();
                        if(skill != null)
                        {
                            SkillBase.TargetType skillTargetType = skill.GetTargetType(); // 스킬 대상 종류 확인
                            switch (skillTargetType)
                            {
                                case SkillBase.TargetType.ENEMY:
                                    {
                                        if (IsTargetInRange(targetUnit, data.AttackRange))
                                        {
                                            ActivateSkill(skill, targetUnit);
                                            return;
                                        }
                                    }
                                    break;
                                case SkillBase.TargetType.ALLY:
                                    {
                                        ActivateSkill(skill, null);
                                    }
                                    return;
                            }
                        }

                        if(IsTargetInRange(targetUnit, data.SightRange))
                        {
                            MoveTo(targetUnit);
                            if(path.status != NavMeshPathStatus.PathComplete)
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

                        targetUnit = SearchReachableTarget(data.SightRange);

                        if (!navAgentEnabled) // navAgent가 비활성화 상태일 경우
                        {
                            navObstacle.enabled = false;
                            navAgent.enabled = true;
                        }
                    }
                }
                break;
            case Mode.CHANGE:
                {
                    if (isSiegeModeInitialized)
                    {
                        siegeParticle.Stop();
                        siegeParticle.gameObject.SetActive(false);
                        isSiegeModeInitialized = false;
                    }

                    siegeEffect.SetActive(false);
                    chagneEffet.SetActive(mode == Mode.CHANGE);

                    if (changeDuration >= 0)
                    {
                        changeDuration -= Time.deltaTime;
                        state = State.IDLE;
                    }
                    else
                    {
                        if(previousMode == Mode.FREE)
                        {
                            mode = Mode.SEIGE;
                        }
                        else if(previousMode == Mode.SEIGE)
                        {
                            mode = Mode.FREE;
                        }

                        changeDuration = 3.0f;
                        previousMode = mode;
                    }

                }
                break;
        }

        //if (previousMode != mode)
        //{
        //    if (mode == Mode.SEIGE)
        //    {
        //        siegeParticle.gameObject.SetActive(true);
        //        siegeParticle.Play();
        //    }
        //    previousMode = mode;
        //}
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

    protected override void ActivateSkill(SkillBase skill, Unit target)
    {
        if (skill == generalSkill)
        {
            state = State.GENERALSKILL;
            modelAnimator.SetTrigger("GeneralSkill");
        }
        else if (skill == specialSkill)
        {
            state = State.SPECIALSKILL;
            modelAnimator.SetTrigger("SpecialSkill");
        }

        if (target != this)
            transform.LookAt(target.transform);

        base.ActivateSkill(skill, target);
    }

    public void ChangeMode(Mode mode)
    {
        this.mode = mode;
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

            GameObject obj = upgradeUnitData.Prefab;
            spawner.CreatUpgradeUnit(obj, (AllyUnitData)upgradeUnitData, this.transform);

            pool.Pool.Release(this);
            gameObject.SetActive(false);    
        }

        // 새 유닛 데이터를 가져와 스포너 방식의 풀로
        // 프리팹 데이터를 가져옴, 기존 유닛 반환후 새 유닛생성, 생성된 유닛으로 풀 생성 -> 키 : 프리팹 , 벨류 allyunit 
    }


    public override void Die()
    {
        navAgent.enabled = false;
        navObstacle.enabled = false;
        collider.enabled = false;

        state = State.DEAD;
        modelAnimator.SetTrigger("Die");
    }
}
