using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AttackType = AttackData.AttackType;
using UltEvents;

public abstract class Unit : MonoBehaviour
{
    public enum ArmorType           //유닛의 방어속성.
    {
        PADDED,         // 완충갑
        ANTIPIERCING,   // 방탄갑
        STEELPLATED     // 철갑
    }

    public enum EventState
    {
        TAKEDAMAGE
    }

    [Header("■ Components")]
    [SerializeField] protected Animator modelAnimator;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected new Collider collider;
    [SerializeField] protected Transform effectParent;
    [SerializeField] protected Transform VFXParent;

    [Header("■ Skill")]
    [SerializeField] private SkillBase generalSkill;
    [SerializeField] private SkillBase specialSkill;
    [SerializeField] private SkillBase passiveSkill;

    [Header("■ Enemy Layer")]
    [SerializeField] protected LayerMask enemyLayer;

    [Header("■ Nearby Distance")]
    [SerializeField] private float nearbyDistance; // 캐릭터 '주변' 위치를 계산하기 위한 거리.

    [Header("■ State Events")]
    [SerializeField] private UltEvent<Unit>[] stateEvents;

    protected float maxhp;
    protected float hp;
    protected float critPercent;
    protected float critVulnerability; // 치명타를 받을 확률.
    protected float mental; // 정신력
    // protected float moveSpeed;
    protected float attackSpeedMult;
    protected float moveSpeedMult;
    protected float damageTakenMult; // 피해량 비율
    protected float atkMult; // 공격력 비율
    protected float blockPercent; // 방어 계수(방어 상성으로 감소하는 수치의 비율)
    protected float interval; // 유닛의 공격 간격(속도) interval 마다 스킬 사용 가능
    protected float intervalCheck; // interval 체크용
    protected float intervalMultiplier = 1f;
    protected bool isStop = false;

    protected UnitStats unitStats;
    [SerializeField] private string unitId;

    protected Collider[] collidersInRange = new Collider[maxTargetCount];
    protected List<Unit> targets = new List<Unit>(); // 탐색 조건을 만족하는 대상들. (조건에 만족하는 대상이 여러 개일 경우 사용)
    protected DurationEffectPool durationEffectPool;
    protected VFXObjectPool hitVFXPool;

    //protected Unit skillTarget; // 공격 대상
    //protected Unit chaseTarget; // 추격 대상
    protected Unit targetUnit;


    protected NavMeshPath path; // 경로 설정용
    protected NavMeshPath pathForSearch; // 경로 탐색용

    protected float stateDuration;
    protected float stateDurationCheck;

    private List<DurationEffect> effectList = new List<DurationEffect>();

    protected const int maxTargetCount = 100;

    protected bool isSelected;

    protected const float moveThresholdOnStop = float.MaxValue;

    protected bool isDead;

    public Transform EffectParent => effectParent;

    public abstract UnitData Data { get; }
    public float Maxhp => maxhp;
    public float Hp => hp;
    public float HpPercent => hp / Maxhp;
    public float Mental => mental;
    public float CritPercent => critPercent;
    public float CritVulnerability => critVulnerability;
    public float BlockPercent => blockPercent;
    public float DamageTakenMult => damageTakenMult;
    public float AtkMult => atkMult;
    public LayerMask EnemyLayer => enemyLayer;
    public SkillBase GeneralSkill => generalSkill;
    public SkillBase SpecialSkill => specialSkill;
    public SkillBase PassiveSkill => passiveSkill;
    public IReadOnlyList<DurationEffect> EffectList => effectList;
    public string UnitId => unitId;
    public UnitStats UnitStats => unitStats;
    public float NearbyDistance => nearbyDistance;
    public float Interval => interval;
    public bool IsDead => isDead;

    public bool IsSelected
    {
        get => isSelected;
        set => isSelected = value;
    }

    protected SelectedUnitUI selectedUnitUI;
    protected SelectedUnitManager selectedUnitManager;

    protected static AudioClip[] slashHitSFX;
    protected static AudioClip slashCritSFX;
    protected static AudioClip[] pierceHitSFX;
    protected static AudioClip pierceCritSFX;
    protected static AudioClip[] crushHitSFX;
    protected static AudioClip crushCritSFX;

    protected static GameObject unitDeathVFX;



    protected static AudioClip[] SlashHitSFX
    {
        get
        {
            if (slashHitSFX == null)
                slashHitSFX = Resources.LoadAll<AudioClip>("Sound/SFX/효과음/캐릭터/HitSFX/Slash/Hit");
            return slashHitSFX;
        }
    }

    protected static AudioClip[] PierceHitSFX
    {
        get
        {
            if (pierceHitSFX == null)
                pierceHitSFX = Resources.LoadAll<AudioClip>("Sound/SFX/효과음/캐릭터/HitSFX/Pierce/Hit");
            return pierceHitSFX;
        }
    }

    protected static AudioClip[] CrushHitSFX
    {
        get
        {
            if (crushHitSFX == null)
                crushHitSFX = Resources.LoadAll<AudioClip>("Sound/SFX/효과음/캐릭터/HitSFX/Crush/Hit");
            return crushHitSFX;
        }
    }

    protected static AudioClip SlashCritSFX
    {
        get
        {
            if (slashCritSFX == null)
                slashCritSFX = Resources.Load<AudioClip>("Sound/SFX/효과음/캐릭터/HitSFX/Slash/Crit/sfx_slashCrit01");
            return slashCritSFX;
        }
    }

    protected static AudioClip PierceCritSFX
    {
        get
        {
            if (pierceCritSFX == null)
                pierceCritSFX = Resources.Load<AudioClip>("Sound/SFX/효과음/캐릭터/HitSFX/Pierce/sfx_pierceCrit01");
            return pierceCritSFX;
        }
    }

    protected static AudioClip CrushCritSFX
    {
        get
        {
            if (crushCritSFX == null)
                crushCritSFX = Resources.Load<AudioClip>("Sound/SFX/효과음/캐릭터/HitSFX/Crush/sfx_crushCrit01");
            return crushCritSFX;
        }
    }

    protected static GameObject UnitDeathVFX
    {
        get
        {
            if (unitDeathVFX == null)
                unitDeathVFX = Resources.Load<GameObject>("Prefabs/VFX/UnitDeath/vfx_characterDeath");
            return unitDeathVFX;
        }
    }

    public virtual void Initialize()
    {
        if (path == null)
            path = new NavMeshPath();

        if (pathForSearch == null)
            pathForSearch = new NavMeshPath();


        hp = maxhp;
        //hp = Data.MaxHp;
        //critChance = Data.CritChance;
        critVulnerability = 0f;
        blockPercent = 1f;
        //mental = Data.Mental;

        //SetUnitStats();
        

        // 이동 속도
        moveSpeedMult = 1f;
        attackSpeedMult = 1f;
        atkMult = 1f;
        damageTakenMult = 1f;


        collider.enabled = true;


        effectParent.gameObject.SetActive(true);
        for (int idx = 0; idx < effectParent.childCount; idx++)
        {
            effectParent.GetChild(idx).gameObject.SetActive(false);
        }

        UpdateState();

        for(int i = 0; i < VFXParent.childCount; i++)
        {
            VFXParent.GetChild(i).gameObject.SetActive(false);
        }

    }

    public void SetHitVFXPool(VFXObjectPool hitVFXPool)
    {
        this.hitVFXPool = hitVFXPool;
    }

    public void SetDurationEffectPool(DurationEffectPool durationEffectPool)
    {
        this.durationEffectPool = durationEffectPool;
    }

    public void SetUnitStats(UnitStats unitStats)
    {
        this.unitStats = unitStats;

        if (unitStats != null)
        {
            maxhp = unitStats.maxHp;
            hp = unitStats.maxHp;
            critPercent = unitStats.critChance;
            mental = unitStats.mental;
            interval = unitStats.interval;

            intervalCheck = interval;
            interval = 0;
            navAgent.speed = unitStats.moveSpeed;
            navAgent.stoppingDistance = 1.0f;


        }
        else
            Debug.Log("데이터 없음");

    }

    public void SetUnitStatsByUpgradeUI(UnitStats unitStats)
    {
        maxhp = unitStats.maxHp;
        hp = unitStats.maxHp;
        critPercent = unitStats.critChance;
        mental = unitStats.mental;
    }

    public void SetSelectedUnitUI(SelectedUnitUI selectedUnitUI)
    {
        this.selectedUnitUI = selectedUnitUI;
    }

    public void SetSelectedUnitManager(SelectedUnitManager selectedUnitManager)
    {
        this.selectedUnitManager = selectedUnitManager;
    }

    protected virtual void Update()
    {
        //PassiveSkillCheck();

        //if (navAgent.velocity.magnitude > navObstacle.carvingMoveThreshold)
        //    lastMoveTime = Time.time;

        //if (lastMoveTime + navObstacle.carvingTimeToStationary < Time.time)
        //{
        //    if (navAgent.enabled)
        //    {
        //        navAgent.enabled = false;
        //        navObstacle.enabled = true;
        //        navObstacle.carvingMoveThreshold = moveThresholdOnStop;
        //        modelAnimator.SetBool("isRunning", false);
        //    }
        //}
    }

    protected virtual void PassiveSkillCheck()
    {
        if (passiveSkill != null)
        {
            passiveSkill.Activate(this);
        }
    }

    protected virtual void ActivateSkill(SkillBase skill, Unit target) // 실제 스킬 사용 부분
    {
        skill.Activate(this, target);

        //if (stateDurationCheck < skill.AnimationStateTime)
        //{
        //    stateDurationCheck += Time.deltaTime;
        //}
        //else
        //{
            
        //}   
    }

    protected bool IsReachable(Vector3 pos)
    {
        navAgent.CalculatePath(pos, pathForSearch);
        //NavMesh.CalculatePath(transform.position, pos, navAgent.areaMask, pathForSearch);
        return pathForSearch.status == NavMeshPathStatus.PathComplete;
    }

    protected bool IsReachable(Unit target)
    {
        if (IsReachable(target.transform.position))
        {
            return true;
        }
        else
        {
            Vector3 startDir = (transform.position - target.transform.position).normalized;
            for (float i = 0f; i < 6f; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(60f * i, Vector3.up) * startDir;
                Vector3 targetPos = target.transform.GetNearPosition(dir, target.nearbyDistance);
                navAgent.CalculatePath(targetPos, pathForSearch);
                //NavMesh.CalculatePath(transform.position, targetPos, navAgent.areaMask, path);
                if (pathForSearch.status == NavMeshPathStatus.PathComplete)
                    return true;
            }
        }

        return false;
    }

    protected Unit SearchNearestTarget(float range)
    {
        Unit result = null;
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if(targetCount > 0)
        {
            float minDst = float.MaxValue;
            for (int i = 0; i < targetCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();
                
                if (unit.HpPercent <= 0f || !unit.gameObject.activeInHierarchy)
                    continue;

                float dst = Vector3.Distance(transform.position, unit.transform.position);

                if (dst < minDst)
                {
                    minDst = dst;
                    result = unit;
                }
            }    
        }

        return result;
    }

    protected Unit SearchNearestTarget(IReadOnlyList<Unit> targets)
    {
        Unit result = null;
        float minDst = float.MaxValue;
        for (int i = 0; i < targets.Count; i++)
        {
            Unit current = targets[i];
            float dst = Vector3.Distance(transform.position, current.transform.position);
            if (dst < minDst)
            {
                minDst = dst;
                result = current;
            }
        }

        return result;
    }

    protected Unit SearchNearestReachableTarget(float range)
    {
        Unit result = null;
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if (targetCount > 0)
        {
            float minDst = float.MaxValue;
            for (int i = 0; i < targetCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();

                if (unit.HpPercent <= 0f || !unit.gameObject.activeInHierarchy)
                    continue;
                float dst = Vector3.Distance(transform.position, unit.transform.position);
                if (dst < minDst)
                {
                    if(IsReachable(unit))
                    {
                        minDst = dst;
                        result = unit;
                    }
                }
            }
        }

        return result;
    }

    protected Unit SearchLowHPTarget(float range)
    {
        Unit result = null;
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if (targetCount > 0)
        {
            float minHpPercent = 1f;
            for (int i = 0; i < targetCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();

                if (unit.HpPercent <= 0f || !unit.gameObject.activeInHierarchy)
                    continue;

                if (unit.HpPercent < minHpPercent)
                {
                    minHpPercent = unit.HpPercent;
                    targets.Clear();
                    targets.Add(unit);
                }
                else if (unit.HpPercent == minHpPercent)
                {
                    targets.Add(unit);
                }
            }
        }

        if (targets.Count > 1)
            result = SearchNearestTarget(targets);
        else if (targets.Count == 1)
            result = targets[0];

        targets.Clear();

        return result;
    }

    protected Unit SearchLowHPReachableTarget(float range)
    {
        Unit result = null;
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if (targetCount > 0)
        {
            float minHpPercent = 1f;
            for (int i = 0; i < targetCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();

                if (unit.HpPercent <= 0f || !unit.gameObject.activeInHierarchy)
                    continue;

                if (unit.HpPercent < minHpPercent)
                {
                    if(IsReachable(unit))
                    {
                        minHpPercent = unit.HpPercent;
                        targets.Clear();
                        targets.Add(unit);
                    }
                }
                else if (unit.HpPercent == minHpPercent)
                {
                    if (IsReachable(unit))
                    {
                        targets.Add(unit);
                    }
                }
            }
        }

        if (targets.Count > 1)
            result = SearchNearestTarget(targets);
        else if (targets.Count == 1)
            result = targets[0];

        targets.Clear();

        return result;
    }

    protected Unit SearchHighTierTarget(float range)
    {
        Unit result = null;
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if (targetCount > 0)
        {
            int maxTier = 0;
            for (int i = 0; i < targetCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();

                if (unit.HpPercent <= 0f || !unit.gameObject.activeInHierarchy)
                    continue;

                if (unit.Data.Tier > maxTier)
                {
                    maxTier = unit.Data.Tier;
                    targets.Clear();
                    targets.Add(unit);
                }
                else if (unit.Data.Tier == maxTier)
                {
                    targets.Add(unit);
                }
            }
        }

        if (targets.Count > 1)
            result = SearchNearestTarget(targets);
        else if (targets.Count == 1)
            result = targets[0];

        targets.Clear();

        return result;
    }

    protected Unit SearchHighTierReachableTarget(float range)
    {
        Unit result = null;
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if (targetCount > 0)
        {
            int maxTier = 0;
            for (int i = 0; i < targetCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();

                if (unit.HpPercent <= 0f || !unit.gameObject.activeInHierarchy)
                    continue;

                if (unit.Data.Tier > maxTier)
                {
                    if(IsReachable(unit))
                    {
                        maxTier = unit.Data.Tier;
                        targets.Clear();
                        targets.Add(unit);
                    }
                }
                else if (unit.Data.Tier == maxTier)
                {
                    if(IsReachable(unit))
                        targets.Add(unit);
                }
            }
        }

        if (targets.Count > 1)
            result = SearchNearestTarget(targets);
        else if (targets.Count == 1)
            result = targets[0];

        targets.Clear();

        return result;
    }

    protected bool IsTargetInRange(Unit target, float range)
    {
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, enemyLayer);
        if(targetCount > 0)
        {
            for(int i = 0; i < targetCount; i++)
            {
                if (target.collider == collidersInRange[i])
                    return true;
            }
        }
        return false;
    }

    protected bool IsTargetInAttackRange(Unit target, float range)
    {
        if (target == null)
            return false;

        float dst = Vector3.Distance(transform.position, target.transform.position);
        return dst <= range;
    }

    protected SkillBase GetAvailableSkill()
    {
        if (specialSkill != null && specialSkill.IsCoolDown )//&& !generalSkill.IsCoolDown)
            return specialSkill;
        else if (generalSkill != null && generalSkill.IsCoolDown)
            return generalSkill;
        else
            return null;
    }

    protected SkillBase GetGeneralSkill()
    {
        if(generalSkill != null && generalSkill.IsCoolDown)
            return generalSkill;
        else
            return null;
    }

    protected SkillBase GetSpecialSkill()
    {
        if (specialSkill != null && specialSkill.IsCoolDown)
            return specialSkill;
        else
            return null;
    }

    public virtual void MoveTo(Vector3 pos)
    {
        //bool navAgentEnabled = navAgent.enabled;
        //if (!navAgentEnabled)
        //{
        //    navObstacle.enabled = false;
        //    navAgent.enabled = true;
        //}

        //navAgent.CalculatePath(pos, path); // 경로 계산

        NavMesh.CalculatePath(transform.position, pos, navAgent.areaMask, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            if (navAgent.isStopped)
                navAgent.isStopped = false;

            //navObstacle.carvingMoveThreshold = unitStats.moveSpeed * 0.1f;
            navAgent.SetPath(path);
        }

        // 경로 계산 이전에 navAgent가 비활성화 상태였을 경우
        //if (!navAgentEnabled)
        //{
        //    // 다시 비활성화 상태로 원상복구.
        //    navAgent.enabled = false;
        //    navObstacle.enabled = true;
        //}
    }

    public virtual void ForceMoveTo(Vector3 pos)
    {
        //bool navAgentEnabled = navAgent.enabled;
        //if (!navAgentEnabled)
        //{
        //    navObstacle.enabled = false;
        //    navAgent.enabled = true;
        //}

        if (navAgent.CalculatePath(pos, path))
        {
            if (navAgent.isStopped)
                navAgent.isStopped = false;

            //navObstacle.carvingMoveThreshold = unitStats.moveSpeed * 0.1f;
            navAgent.SetPath(path);
            return;
        }

        // 경로 계산 이전에 navAgent가 비활성화 상태였을 경우
        //if (!navAgentEnabled)
        //{
        //    // 다시 비활성화 상태로 원상복구.
        //    navAgent.enabled = false;
        //    navObstacle.enabled = true;
        //}
    }

    public virtual void MoveTo(Unit target)
    {
        //bool navAgentEnabled = navAgent.enabled;
        //if (!navAgentEnabled) // navAgent가 비활성화 상태일 경우
        //{
        //    navObstacle.enabled = false;
        //    navAgent.enabled = true;
        //}


        Vector3 startDir = (transform.position - target.transform.position).normalized;
        for (float i = 0f; i < 6f; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(60f * i, Vector3.up) * startDir;
            Vector3 targetPos = target.transform.GetNearPosition(dir, target.nearbyDistance);

            NavMesh.CalculatePath(transform.position, targetPos, navAgent.areaMask, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            { 
                if (navAgent.isStopped)
                    navAgent.isStopped = false;


                //navObstacle.carvingMoveThreshold = unitStats.moveSpeed * 0.1f;
                navAgent.SetPath(path);

                //float distance = Vector3.Distance(transform.position, targetPos);
                //Debug.Log($"1 :  {distance}");

            }
            else if(path.status == NavMeshPathStatus.PathPartial)
            {
                navAgent.SetPath(path);
            }
        }

        // 경로 계산 이전에 navAgent가 비활성화 상태였을 경우
        //if (!navAgentEnabled)
        //{
        //    // 다시 비활성화 상태로 원상복구.
        //    navAgent.enabled = false;
        //    navObstacle.enabled = true;
        //}
    }

    public void LookAt(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        dir.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
    }

    //public void PlayHitSFX(AttackType attackType)
    //{
    //    AudioClip[] hitSFX = null;
    //    switch (attackType)
    //    {
    //        case AttackType.SLASH:
    //            hitSFX = SlashHitSFX;
    //            break;
    //        case AttackType.PIERCE:
    //            hitSFX = PierceHitSFX;
    //            break;
    //        case AttackType.CRUSH:
    //            hitSFX = CrushHitSFX;
    //            break;
    //    }
    //    if (hitSFX != null)
    //    {
    //        int randomIndex = Random.Range(0, hitSFX.Length);
    //        SoundManager.Instance.PlaySFX(hitSFX[randomIndex]);
    //    }
    //}

    public void PlayCritSFX(AttackType attackType)
    {
        AudioClip critSFX = null;
        switch (attackType)
        {
            case AttackType.SLASH:
                critSFX = SlashCritSFX;
                break;
            case AttackType.PIERCE:
                critSFX = PierceCritSFX;
                break;
            case AttackType.CRUSH:
                critSFX = CrushCritSFX;
                break;
        }
        if (critSFX != null)
        {
            SoundManager.Instance.PlaySFX(critSFX);
        }
    }

    public virtual void TakeDamage(float Damage)
    {
        hp -= Damage;
        if (hp <= 0)
        {
            Die();
        }

        InvokeEvent(EventState.TAKEDAMAGE);

        if (selectedUnitUI != null)
        {
            selectedUnitUI.UpdateHPUI(this);
        }
    }

    public virtual void Die()
    {
        if (isDead) return;

        hp = 0f;

        isDead = true;

        navAgent.enabled = false;
        collider.enabled = false;

        modelAnimator.SetTrigger("Die");


        if (selectedUnitUI != null)
        {
            // ui 제거
            selectedUnitUI.HideHp();
            selectedUnitUI.HideAllyUI();
            selectedUnitUI.HideUpgrdeUI();
            selectedUnitUI.HideUntInfo();
        }

        if (selectedUnitManager != null && selectedUnitManager.SelectedUnit == this)
        {
            selectedUnitManager.DeSelecteUnit();
        }

        //if (!isDead)
        //{
            

        //    navAgent.enabled = false;
        //    collider.enabled = false;
        //    //effectParent.gameObject.SetActive(false);

        //    modelAnimator.SetTrigger("Die");

        //    if (selectedUnitUI != null)
        //    {
        //        // ui 제거
        //        selectedUnitUI.HideHp();
        //        selectedUnitUI.HideAllyUI();
        //        selectedUnitUI.HideUpgrdeUI();
        //        selectedUnitUI.HideUntInfo();
        //    }

        //    if (selectedUnitManager != null && selectedUnitManager.SelectedUnit == this)
        //    {
        //        selectedUnitManager.DeSelecteUnit();
        //    }

        //    isDead = true;
        //}
    }


    public void SetStateDuration(float duration) => stateDuration = duration;

    public void AddMoveSpeedMult(float percent)
    {
        moveSpeedMult += percent * 0.01f;
        navAgent.speed = unitStats.moveSpeed * Mathf.Max(0f, moveSpeedMult);
    }

    public void AddMental(float amount)
    {
        mental += amount;
    }

    public void AddAttackSpeedMult(float percent)
    {
        attackSpeedMult += percent * 0.01f;
        //attackSpeed = unitStats.attackSpeed * Mathf.Max(0f, attackSpeedMult);
    }

    public void AddCriticalVulnerability(float amount)
    {
        critVulnerability += amount;
    }
    public void AddBlockPercent(float percent)
    {
        blockPercent += percent * 0.01f;    // 단위 수정_AYO
    }

    

    public void AddAtkMult(float percent)
    {
        // 추가 피해량
        atkMult += percent * 0.01f;
    }

    public void AddDamageTakenMult(float percent)
    {
        // 받는 피해량
        damageTakenMult += percent * 0.01f;
    }

    public void ChangeInterval(float percent) // interval을 변화시키는 함수
    {
        intervalMultiplier -= percent * 0.01f; 
        interval = intervalCheck * intervalMultiplier;
        intervalCheck = interval;
    }

    public void RevertInterval(float percent)
    {
        intervalCheck = unitStats.interval;
        intervalMultiplier += percent * 0.01f;
        interval = intervalCheck * intervalMultiplier;
    }

    public abstract void GetProvoked(Unit ProvokedTarget);

    public virtual void RemoveProvoked()
    { }

    public virtual void GetStun()
    {
        modelAnimator.SetBool("isStun", true);
    }

    public virtual void RemoveStun()
    {
        modelAnimator.SetBool("isStun", false);
    }

    public void AddImmediateEffect(GameObject effectPrefab)
    {
        DurationEffect effect = durationEffectPool.GetDurationEffect(effectPrefab);
        effect.transform.SetParent(effectParent);
        effect.transform.localPosition = Vector3.zero;
        effect.Initialize(this);
        //effect.Activate();
        effect.gameObject.SetActive(true);
    }

    public void AddEffect(GameObject effectPrefab ,Unit unit)
    {
        if (unit.IsDead)
            return;

        DurationEffect prevEffect = effectList.Find(effect => effect.IsSameType(effectPrefab));

        // 효과 목록 중에 추가된 효과가 존재할 경우.
        if (prevEffect != null)
        {
            if (prevEffect.Prefab == effectPrefab) // 기존 효과와 동일한 경우
            {
                prevEffect.Reapply(effectPrefab);
            }
            else
            {
                return;
            }
               
        }
        else //맨 처음 효과 오브젝트가 추가될 때.
        {
            DurationEffect effect = durationEffectPool.GetDurationEffect(effectPrefab);
            effectList.Add(effect);

            effect.transform.SetParent(effectParent);
            effect.transform.localPosition = Vector3.zero;
            effect.Initialize(this);
            effect.Activate();
            effect.gameObject.SetActive(true);
        }

        UpdateState();
    }
    public void RemoveEffect(DurationEffect effect)
    {
        effectList.Remove(effect);
        UpdateState();
    }

    public void RemoveAllEffect()
    {

        //if (effectList.Count > 0)
        //{
        //    Debug.Log(effectList.Count);

        //    for(int i = 0; i <= effectList.Count; i++)
        //    {
        //        effectList.Remove(effectList[i]);
        //    }

        //    UpdateState();
        //}
    }

    public void AddVFX(GameObject vfx, Transform rot) // hit & Crit VFX (오브젝트풀링 사용)
    {
        GameObject VFXobj = hitVFXPool.GetVFX(vfx, this);
        if (VFXobj == null)
            return;

        VFXobj.transform.SetParent(VFXParent);
        VFXobj.SetActive(true);
        VFXobj.transform.localPosition = Vector3.up * VFXobj.transform.localPosition.y;
        VFXobj.transform.localRotation = rot.localRotation; //Quaternion.Euler(0f, 0f, 0f);
    }

    public void AddVFX(ParticleSystem VFX)
    {
        GameObject VFXobj = Instantiate(VFX.gameObject);
        VFXobj.transform.SetParent(VFXParent);
        VFXobj.transform.localPosition = Vector3.up * VFXobj.transform.localPosition.y;
        VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Destroy(VFXobj, VFX.main.duration);
    }

    public void AddVFX(ParticleSystem VFX, Vector3 LookPos)
    {
        GameObject VFXobj = Instantiate(VFX.gameObject);
        VFXobj.transform.SetParent(VFXParent);
        VFXobj.transform.localPosition =  Vector3.up * VFXobj.transform.localPosition.y;
        if (LookPos != Vector3.zero)
        {
            VFXobj.transform.LookAt(LookPos);
            //float yRotation = VFXobj.transform.localRotation.eulerAngles.y;
            //VFXobj.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
            //VFXobj.transform.LookAt(new Vector3(LookPos.position.x, 1f, LookPos.position.z));
        }

        Destroy(VFXobj, VFX.main.duration);
    }

    public void InvokeEvent(EventState state)
    {
        if(stateEvents == null || stateEvents.Length <= (int)state)
            return;

        UltEvent<Unit> stateEvent = stateEvents[(int)state];
        if (stateEvent != null)
            stateEvent.Invoke(this);
    }

    public void UpdateState()
    {
        if(selectedUnitUI != null)
        {
            selectedUnitUI.UpdateUnitStateUI();
        }
    }


    public void Setpriority(int priority)
    {
        navAgent.avoidancePriority = priority;
    }

    public void StopUnit()
    {
        isStop = true;

        if (navAgent.enabled)
            navAgent.isStopped = true;
        modelAnimator.SetBool("isRunning", false);
    }
}