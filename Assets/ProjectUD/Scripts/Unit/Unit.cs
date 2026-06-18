using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AttackType = AttackData.AttackType;
using ActiveType = SpecialAbility.ActiveType;
using UltEvents;
using AYellowpaper.SerializedCollections;
using DG.Tweening;

public abstract class Unit : MonoBehaviour
{
    public enum ArmorType           //유닛의 방어속성.
    {
        PADDED,         // 완충갑
        ANTIPIERCING,   // 방탄갑
        STEELPLATED,    // 철갑
        NONE            // 무속성
    }

    // 특수 능력 발동 조건
    //public enum SpecialAbility //EventState
    //{
    //    NONE,           // 능력 없음
    //    TAKEDAMAGE,     // 피해 받았을 때 발동
    //    BASIC,         // 기본적으로 발동
    //    KILL           // 적 처치시 발동
    //}

    [Header("■ Components")]
    [SerializeField] protected Animator modelAnimator;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected new Collider collider;
    [SerializeField] protected Transform effectParent;
    [SerializeField] protected Transform VFXParent;
    [SerializeField] protected Transform heightPos;

    [Header("■ Skill")]
    [SerializeField] private SkillBase generalSkill;
    [SerializeField] private SkillBase specialSkill;
    [SerializeField] private SpecialAbility specialAbility;

    protected SkillBase currentSkill;     // 현재 보유한 스킬

    [Header("■ Enemy Layer")]
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected LayerMask allyLayer;

    [Header("■ Nearby Distance")]
    [SerializeField] private float nearbyDistance; // 캐릭터 '주변' 위치를 계산하기 위한 거리.

    [Header("■ State Events")]
    //[SerializeField] private UltEvent<Unit>[] stateEvents;
    //[SerializeField] private SpecialAbility specialAbility;

    protected float maxhp;
    protected float hp;
    protected float critPercent;
    protected float critVulnerability; // 치명타를 받을 확률.
    protected int mental; // 정신력
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
    private ArmorType armorType;

    protected Collider[] collidersInRange = new Collider[maxTargetCount];
    protected List<Unit> targets = new List<Unit>(); // 탐색 조건을 만족하는 대상들. (조건에 만족하는 대상이 여러 개일 경우 사용)
    protected DurationEffectPool durationEffectPool;
    protected InstantEffectPool instantEffectPool;
    protected VFXObjectPool hitVFXPool;
    protected VFXObjectPool skillVFXPool;
    protected EffectImagePool effectImagePool;

    //protected Unit skillTarget; // 공격 대상
    //protected Unit chaseTarget; // 추격 대상
    protected Unit targetUnit; // 스킬 사용 대상
    protected Unit executionUnit;   // 척살 명령 대상

    protected NavMeshPath path; // 경로 설정용
    protected NavMeshPath pathForSearch; // 경로 탐색용
    protected Vector3 targetPos;

    protected float stateDuration;
    protected float stateDurationCheck;

    private List<DurationEffect> effectList = new List<DurationEffect>();
    private EffectImage[] effectImages = new EffectImage[3];

    protected const int maxTargetCount = 100;

    protected bool isSelected;
    protected bool isSkillActive = false; // 스킬 사용중인지를 확인하는 변수

    protected const float moveThresholdOnStop = float.MaxValue;

    protected bool isDead;

    // 판단 유예 상태 관련 변수
    protected bool isDeferredState = false;
    protected const float deferredStateDuration = 0.5f;
    protected float deferredStateDurationCheck;
    protected GameObject deferredStateVFX;
    protected GameObject deferredStateObj;

    [SerializedDictionary("State", "Animation Clips")]
    public SerializedDictionary<string, AnimationClip[]> stateAnimDic; // = new Dictionary<string, AnimationClip[]>();

    public Transform EffectParent => effectParent;

    public abstract UnitData Data { get; }
    public float Maxhp => maxhp;
    public float Hp => hp;
    public float HpPercent => hp / Maxhp;
    public int Mental => mental;
    public float CritPercent => critPercent;
    public float CritVulnerability => critVulnerability;
    public float BlockPercent => blockPercent;
    public float DamageTakenMult => damageTakenMult;
    public float AtkMult => atkMult;
    public LayerMask EnemyLayer => enemyLayer;
    public SkillBase GeneralSkill => generalSkill;
    public SkillBase SpecialSkill => specialSkill;
    public SkillBase CurrentSKill => currentSkill;
    public SpecialAbility SpecialAbility => specialAbility;
    public IReadOnlyList<DurationEffect> EffectList => effectList;
    public string UnitId => unitId;
    public UnitStats UnitStats => unitStats;
    public float NearbyDistance => nearbyDistance;
    public float Interval => interval;
    public bool IsDead => isDead;
    public Transform HeightPos => heightPos;
    public VFXObjectPool SkillVfxPool => skillVFXPool;
    public EffectImagePool EffectImagePool => effectImagePool;
    public ArmorType Armortype => armorType;

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
        

        moveSpeedMult = 1f;
        attackSpeedMult = 1f;
        atkMult = 1f;
        damageTakenMult = 1f;
        

        collider.enabled = true;
        deferredStateDurationCheck = deferredStateDuration;
        armorType = Data.ArmorType;

        // 스킬 쿨타임 초기화
        generalSkill.Initialize();

        if(specialSkill != null)
            specialSkill.Initialize();

        //InvokeEvent(SpecialAbility.BASIC);
        ActivateSpecialAbility(ActiveType.ALWAYS);

        //deferredStateVFX = Resources.Load<GameObject>("Prefabs/VFX/VFX_provoked/VFX_provoked_02");

        //if(deferredStateObj == null)
        //{
        //    deferredStateObj = Instantiate(deferredStateVFX, HeightPos);
        //}
        //deferredStateObj.SetActive(false);


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

    public void SetHitVFXPool(VFXObjectPool hitVFXPool, VFXObjectPool skillVFXPool)
    {
        this.hitVFXPool = hitVFXPool;
        this.skillVFXPool = skillVFXPool;
    }

    public void SetDurationEffectPool(DurationEffectPool durationEffectPool)
    {
        this.durationEffectPool = durationEffectPool;
    }

    public void SetInstantEffectPool(InstantEffectPool instantEffectPool)
    {
        this.instantEffectPool = instantEffectPool;
    }

    public void SetEffectImagePool(EffectImagePool effectImagePool)
    {
        this.effectImagePool = effectImagePool;
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
            intervalCheck = unitStats.interval;
            interval = 0;
            navAgent.speed = unitStats.moveSpeed;
            //navAgent.stoppingDistance = 1.0f;


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


    protected virtual void ActivateSkill(SkillBase skill, Unit target) 
    {
        if (target == null)
            return;
        skill.Activate(this, target);
        isSkillActive = false;

        //if(target.IsDead)
        //{
        //    ActivateSpecialAbility(ActiveType.KILL);
        //}

        //if (stateDurationCheck < skill.AnimationStateTime)
        //{
        //    stateDurationCheck += Time.deltaTime;
        //}
        //else
        //{

        //}   
    }



    protected virtual void ActivateFortressSkill(SkillBase skill, Fortress target)
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

    public virtual Unit SearchTarget(float range, LayerMask targetLayer, SkillBase skill)
    {
        switch(skill.GetTargetRule())
        {
            case SkillBase.TargetRule.NEAR:
                return SearchNearestTarget(range, targetLayer);
            case SkillBase.TargetRule.LOWHP:
                return SearchLowHPTarget(range, targetLayer);
            case SkillBase.TargetRule.RANDOM:
                return SearchRandomTarget(range, targetLayer);
            default:
                return null;
        }
    }

    protected Unit SearchTargetInTargets(SkillBase skill)
    {
        switch(skill.GetTargetRule())
        {
            case SkillBase.TargetRule.NEAR:
                return SearchNearestTarget();
            case SkillBase.TargetRule.LOWHP:
                return SearchLowHPTarget();
            case SkillBase.TargetRule.RANDOM:
                return SearchRandomTarget();
            default:
                return null;
        }
    }

    protected Unit SearchNearestTarget()
    {
        //if (targets.Count == 0)
        //    return null;

        //Unit result = null;
        //float minDst = float.MaxValue;


        //for (int i = 0; i < targets.Count; i++)
        //{
        //    float dist = (transform.position - targets[i].transform.position).sqrMagnitude;

        //    if (dist < minDst)
        //    {
        //        minDst = dist;
        //        result = targets[i];
        //    }
        //}

        //return result;

        if (targets.Count == 0)
            return null;

        Unit result = null;
        float nearest = float.MaxValue;

        NavMeshPath bestPath = new NavMeshPath();

        for (int i = 0; i < targets.Count; i++)
        {
            Unit target = targets[i];

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

    protected float CalculatePathLength(NavMeshPath path)
    {
        float length = 0f;
        if (path.corners.Length < 2)
            return 0f;

        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return length;
    }

    protected Unit SearchLowHPTarget()
    {
        if (targets.Count == 0)
            return null;

        Unit result = null;
        float minHp = float.MaxValue;

        for(int i = 0; i < targets.Count; i++)
        {
            float hp = targets[i].HpPercent;

            if(hp < minHp)
            {
                minHp = hp;
                result = targets[i];
            }
        }

        return result;
    }

    protected Unit SearchRandomTarget()
    {
        if (targets.Count == 0)
            return null;

        int randomIndex = Random.Range(0, targets.Count);
        return targets[randomIndex];
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

    protected bool IsTargetValid(Unit target, LayerMask targetLayer)
    {
        if (target != null && !target.isDead) //&& IsTargetInRange(target, range, targetLayer))
        {
            return true;
        }
        else
        {
            targetUnit = null;
            return false;
        }
    }

    protected bool IsTargetValid(Unit target,float range, LayerMask targetLayer)
    {
        if (target != null && !target.isDead) //&& IsTargetInRange(target, range, targetLayer))
        {
            return true;
        }
        else
        {
            targetUnit = null;
            return false;
        }
    }

    protected void SearchReachableTargets(float range, LayerMask targetLayer)   // 시야 범위내 이동 가능 유닛 확인
    {
        targets.Clear();

        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, targetLayer);

        if (enemyCount > 0)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();
                if (unit == null)
                    continue;

                if (unit.isDead || !unit.gameObject.activeInHierarchy)
                    continue;

                if(unit == this)
                {
                    if (targetLayer == allyLayer)
                    {
                        targets.Add(unit);
                        continue;
                    }
                    else
                        continue;
                }

                NavMesh.CalculatePath(transform.position, unit.transform.position, navAgent.areaMask, pathForSearch);

                if (pathForSearch.status == NavMeshPathStatus.PathComplete)
                {
                    targets.Add(unit);
                }
                else
                {
                    for (float j = 0f; j < 6f; j++)
                    {
                        Vector3 startDir = (transform.position - unit.transform.position).normalized;
                        Vector3 dir = Quaternion.AngleAxis(60f * j, Vector3.up) * startDir;
                        Vector3 targetPos = unit.transform.GetNearPosition(dir, unit.nearbyDistance);

                        //GameObject debugObj = GameObject.CreatePrimitive(PrimitiveType.Sphere); 
                        //debugObj.transform.position = targetPos;

                        NavMesh.CalculatePath(transform.position, targetPos, navAgent.areaMask, pathForSearch);

                        if (pathForSearch.status == NavMeshPathStatus.PathComplete)
                        {
                            targets.Add(unit);
                            break;
                        }
                        //else if(pathForSearch.status == NavMeshPathStatus.PathInvalid)
                        //{
                        //    if(navAgent.isOnNavMesh)
                        //    {
                        //        Debug.Log(11111111);
                        //    }
                        //    else
                        //    {
                        //        Debug.Log(222222222);
                        //    }
                        //}
                    }

                    
                }
            }

            if(targets.Count == 0)
            {
               Debug.Log("검색된 대상 없음");
            }
        }
    }


    protected bool IsPathBlocked(Unit target)
    {

        if (NavMesh.CalculatePath(transform.position, target.transform.position, navAgent.areaMask, pathForSearch) &&
            pathForSearch.status == NavMeshPathStatus.PathComplete)
            return false;

        if (HasReachablePosition(target))
            return false;

        return true;
    }

    protected bool HasReachablePosition(Unit unit)
    {
        Vector3 startDir = (transform.position - unit.transform.position).normalized;

        for (int i = 0; i < 6; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(60f * i, Vector3.up) * startDir;
            Vector3 targetPos = unit.transform.GetNearPosition(dir, unit.nearbyDistance);

            if (NavMesh.CalculatePath(transform.position, targetPos, navAgent.areaMask, pathForSearch) &&
                pathForSearch.status == NavMeshPathStatus.PathComplete)
            {
                return true;
            }
        }

        //for (int i = 0; i < 4; i++)
        //{
        //    Vector3 dir = Quaternion.AngleAxis(90f * i, Vector3.up) * startDir;
        //    Vector3 rawTargetPos = unit.transform.GetNearPosition(dir, unit.nearbyDistance);

        //    if (!NavMesh.SamplePosition(rawTargetPos, out NavMeshHit hit, 0.5f, navAgent.areaMask))
        //        continue;

        //    if (NavMesh.CalculatePath(transform.position, hit.position, navAgent.areaMask, pathForSearch) &&
        //        pathForSearch.status == NavMeshPathStatus.PathComplete)
        //    {
        //        return true;
        //    }
        //}

        return false;
    }

    

    protected Unit SearchNearestTarget(float range, LayerMask targetLayer)
    {
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, targetLayer);

        Unit result = null;
        float minDst = float.MaxValue;

        if (targetCount > 0)
        {
            for (int i = 0; i < targetCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();
                if (unit == null)
                    continue;

                if (unit.isDead || !unit.gameObject.activeInHierarchy || unit == this)
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

    //protected Unit SearchNearestTarget()
    //{
    //    Unit result = null;
    //    float minDst = float.MaxValue;

    //    int targetCount = targets.Count;

    //    if (targetCount > 0)
    //    {
    //        for (int i = 0; i < targetCount; i++)
    //        {
    //            Unit unit = targets[i];
    //            float dst = Vector3.Distance(transform.position, unit.transform.position);
    //            if (dst < minDst)
    //            {
    //                minDst = dst;
    //                result = unit;
    //            }
    //        }
    //    }

    //    return result;
    //}

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

    protected Unit SearchLowHPTarget(float range, LayerMask targetLayer)
    {
        Unit result = null;

        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, targetLayer);

        if(targetCount > 0)
        {
            float minHpPercent = 1f;

            for(int i = 0; i < targetCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();

                if (unit == null)
                    continue;

                if (unit.isDead || !unit.gameObject.activeInHierarchy)
                    continue;

                if (unit.HpPercent <= minHpPercent)
                {
                    minHpPercent = unit.HpPercent;
                    result = unit;
                }
            }
        }

        return result;
    }

    protected Unit SearchRandomTarget(float range, LayerMask targetLayer)
    {
        Unit result = null;

        targets.Clear();

        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, targetLayer);

        if (targetCount > 0)
        {
            for (int i = 0; i < targetCount; i++)
            {
                Unit unit = collidersInRange[i].GetComponent<Unit>();

                if (unit == null)
                    continue;

                if (unit.isDead || !unit.gameObject.activeInHierarchy)
                    continue;

                targets.Add(unit);
            }
        }

        if(targetCount <= 0 || targets.Count <= 0)
            return null;

        int randomIndex = Random.Range(0, targets.Count);

        result = targets[randomIndex];
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
        if (targetCount > 0)
        {
            for (int i = 0; i < targetCount; i++)
            {
                if (target.collider == collidersInRange[i])
                    return true;
            }
        }
        return false;

        //float dist = Vector3.Distance(transform.position, target.transform.position);

        //return dist <= range;
    }

    protected bool IsTargetInRange(Unit target, float range, LayerMask targetLayer)
    {
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, range, collidersInRange, targetLayer);
        if (targetCount > 0)
        {
            for (int i = 0; i < targetCount; i++)
            {
                if (target.collider == collidersInRange[i])
                    return true;
            }
        }
        return false;
    }

    protected bool IsTargetPosInRange(Vector3 pos, float range)
    {

        float distance = Vector3.Distance(pos, transform.position);
        return distance <= range;
    }

    protected bool IsTargetInAttackRange(Unit target, float range)
    {
        if (target == null)
            return false;

        float dst = Vector3.Distance(transform.position, target.transform.position);
        return dst <= range;
    }

    protected SkillBase GetAvailableSkill()     // 쿨타임과 필요 멘탈치를 충족해야 스킬 반환
    {
        if (specialSkill.Data.ActiveMental > mental)
            Debug.Log("멘탈 수치 부족");

        if (specialSkill != null && specialSkill.IsCoolDown && specialSkill.Data.ActiveMental <= mental)
            return specialSkill;
        else if (generalSkill != null && generalSkill.IsCoolDown && generalSkill.Data.ActiveMental <= mental)
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

    protected  void MoveToTargetUnit(Unit target)
    {
        if (!navAgent.pathPending)  // 경로 계산 아닐 때
        {
            // 타겟과의 방향 및 NearbyDistance 거리만큼 떨어진 위치 계산
            Vector3 direction = (transform.position - target.transform.position).normalized;
            Vector3 nearbyPos = target.transform.GetNearPosition(direction, NearbyDistance);

            // 해당 위치가 NavMesh 위에 있는지 확인
            NavMeshHit hit;
            if (NavMesh.SamplePosition(nearbyPos, out hit, 0.1f, navAgent.areaMask))
            {
                //NavMesh위에 있음 
                targetPos = nearbyPos;
                float distance = Vector3.Distance(transform.position, target.transform.position);

                //Debug.Log(navAgent.stoppingDistance);
                //Debug.Log(distance);
            }
            else
            {
                // NavMesh 위에 있지 않으면 보정
                if (NavMesh.SamplePosition(nearbyPos, out hit, 2.0f, navAgent.areaMask))
                {
                    targetPos = hit.position;
                }
                else
                {
                    // 보정 실패 경우, Debug용
                    Debug.Log("보정 실패");
                }
            }

            if(navAgent.isStopped)
                navAgent.isStopped = false;

            navAgent.SetDestination(targetPos);
        }
    }

    public virtual void MoveTo(NavMeshPath path)
    {
        //if (path == null || path.corners == null)
        //{
        //    return;
        //}

        if (navAgent.isStopped)
            navAgent.isStopped = false;


        navAgent.SetPath(path);

        // 타겟이 멀어지면 다시 이동
        //if(!IsTargetInAttackRange(targetUnit, unitStats.attackRange))
        //{
        //    Debug.Log(111111111);
        //    navAgent.SetDestination(targetUnit.transform.position);
        //}




        //if (Vector3.Distance(transform.position, targetPos) <= navAgent.stoppingDistance + 0.2f)
        //{
        //    if (!IsTargetInAttackRange(target, UnitStats.attackRange))
        //    {
        //        targetUnit = SearchNearestTarget(UnitStats.sightRange);


            //    }
            //}


            //float nearestDistance = float.MaxValue;
            //bool hasAvailablePath = false;

            //Vector3 result = Vector3.zero;
            //Vector3 startDir = (transform.position - target.transform.position).normalized;

            //for (float i = 0f; i < 6f; i++)
            //{
            //    Vector3 dir = Quaternion.AngleAxis(60f * i, Vector3.up) * startDir;
            //    Vector3 targetPos = target.transform.GetNearPosition(dir, target.nearbyDistance);

            //    NavMesh.CalculatePath(transform.position, targetPos, navAgent.areaMask, path);

            //    if (path.status == NavMeshPathStatus.PathComplete)
            //    {
            //        float distance = Vector3.Distance(transform.position, targetPos);

            //        if (distance < nearestDistance)
            //        {
            //            if (!hasAvailablePath)
            //                hasAvailablePath = true;

            //            nearestDistance = distance;
            //            result = targetPos;
            //            //navAgent.SetPath(path);
            //        }
            //    }


            //    //if (path.status != NavMeshPathStatus.PathInvalid)
            //    //{
            //    //    float distance = Vector3.Distance(transform.position, targetPos);

            //    //    if (distance < nearestDistance)
            //    //    {
            //    //        if (!hasAvailablePath)
            //    //            hasAvailablePath = true;

            //    //        nearestDistance = distance;
            //    //        result = targetPos;
            //    //        navAgent.SetPath(path);
            //    //    }
            //    //}
            //}

            //if (hasAvailablePath)
            //{
            //    if (navAgent.isStopped)
            //        navAgent.isStopped = false;

            //    navAgent.SetDestination(result);
            //}
            //else
            //{
            //    Unit newTarget = SearchNearestTarget(unitStats.sightRange, targetUnit);

            //    if (newTarget != null)
            //    {
            //        targetUnit = newTarget;
            //        //MoveTo(targetUnit);
            //    }
            //    else
            //    {
            //        Debug.Log("1111111");
            //    }
            //}
            //Vector3 startDir = (transform.position - target.transform.position).normalized;
            //for (float i = 0f; i < 6f; i++)
            //{
            //    Vector3 dir = Quaternion.AngleAxis(60f * i, Vector3.up) * startDir;
            //    Vector3 targetPos = target.transform.GetNearPosition(dir, target.nearbyDistance);

            //    NavMesh.CalculatePath(transform.position, targetPos, navAgent.areaMask, path);

            //    if (path.status != NavMeshPathStatus.PathInvalid)
            //    {
            //        navAgent.SetPath(path);
            //    }
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
            SoundManager.Instance.PlaySFX(critSFX, this.transform.position);
        }
    }

    public virtual void TakeDamage(float Damage)
    {
        hp -= Damage;
        hp = Mathf.Clamp(hp, 0f, Maxhp);

        if (hp <= 0)
        {
            Die();
        }

        ActivateSpecialAbility(ActiveType.HP);

        if (selectedUnitUI != null)
        {
            selectedUnitUI.UpdateHPUI(this);
        }
    }

    public virtual void Die()
    {
        //if (isDead) return;
        hp = 0f;

        RemoveAllEffect();

        stateDurationCheck = 0f;

        interval = intervalCheck; //interval 초기화

        isDeferredState = false;
        deferredStateDurationCheck = deferredStateDuration;

        navAgent.enabled = false;
        collider.enabled = false;

        PlayAnimation("Die");
        //modelAnimator.SetTrigger("Die");


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

        
    }


    public void SetStateDuration(float duration)
    {
        stateDuration = duration;
    }

    public void AddMoveSpeedMult(float percent)
    {
        moveSpeedMult += percent * 0.01f;
        navAgent.speed = unitStats.moveSpeed * Mathf.Max(0f, moveSpeedMult);
    }

    public void AddMental(int amount)
    {
        mental += amount;
        if (mental <= 0)
            mental = 0;
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
        Debug.Log("피해량 : " + damageTakenMult);
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


    public virtual void RemoveProvoked()
    { }

    public virtual void GetStun()
    {
        modelAnimator.SetBool("isStun", true);
    }

    public virtual void RemoveStun()
    {
        modelAnimator.SetBool("isStun", false);
        if (navAgent.enabled)
            navAgent.isStopped = false;
    }

    public void AddInstantEffect(GameObject effectPrefab)
    {
        InstantEffect effect = instantEffectPool.GetInstantEffect(effectPrefab);
        effect.transform.SetParent(effectParent);
        effect.transform.localPosition = Vector3.zero;
        //effect.Initialize(this);
        //effect.Activate();
        effect.gameObject.SetActive(true);
    }

    public void AddEffect(GameObject effectPrefab ,Unit unit, Vector3 pos)
    {
        if (unit.IsDead)
            return;


        DurationEffect prevEffect = effectList.Find(effect => effect.IsSameType(effectPrefab));


        // 효과 목록 중에 추가된 효과가 존재할 경우.
        if (prevEffect != null)
        {
            //if(prevEffect.Type == EffectType.CURSE)
            //{
            //    float finalProbability = 0.5f + CalculateCurseEffectProbability(this.Mental, unit.Mental);
            //    Debug.Log("확률 : " + finalProbability);

            //    if (Random.Range(0f, 1f) > finalProbability)
            //        return;
            //}

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

            //if(effect.Type == EffectType.CURSE)
            //{
            //    float finalProbability = 0.5f + CalculateCurseEffectProbability(Mental, unit.Mental);
            //    Debug.Log("확률 : " + finalProbability);
            //    if (Random.Range(0f, 1f) > finalProbability)
            //        return;
            //}

            effectList.Add(effect);

            effect.transform.SetParent(effectParent);
            effect.transform.position = pos;
            effect.Initialize(this);
            effect.Activate();
            effect.gameObject.SetActive(true);
        }

        UpdateState();
    }

    public void AddMaxStackEffect(GameObject effectPrefab, Unit unit, Vector3 pos)
    {
        DurationEffect prevEffect = effectList.Find(effect => effect.IsSameType(effectPrefab));

        if (prevEffect != null)// && prevEffect.Prefab == effectPrefab)
        {
            if (prevEffect is StackEffect stackEffect)
            {
                stackEffect.ActivateMaxStack();
            }
        }
        else
        {
            AddEffect(effectPrefab, unit, pos);
        }
    }


    public void RemoveEffect(DurationEffect effect)
    {
        effectList.Remove(effect);
        UpdateState();
    }

    protected void RemoveAllEffect()
    {
        for (int i = effectList.Count - 1; i >= 0; i--)
        {
            effectList[i].RemoveEffect();
            effectList.RemoveAt(i);
        }
    }


    public void AddVFX(GameObject vfx, Transform rot) // hit & Crit VFX (오브젝트풀링 사용)
    {
        GameObject VFXobj = hitVFXPool.GetVFX(vfx, this);
        if (VFXobj == null)
            return;

        VFXobj.transform.SetParent(VFXParent);
        VFXobj.SetActive(true);
        VFXobj.transform.localPosition = Vector3.up * VFXobj.transform.localPosition.y;
        VFXobj.transform.localRotation = rot.localRotation * Quaternion.Euler(0f, 90f, 0f); //Quaternion.Euler(0f, 0f, 0f);
    }

    public void AddVFX(GameObject vfx, Vector3 dir) // hit & Crit VFX (오브젝트풀링 사용)
    {
        GameObject VFXobj = hitVFXPool.GetVFX(vfx, this);
        if (VFXobj == null)
            return;

        VFXobj.transform.SetParent(VFXParent);
        VFXobj.transform.forward = dir;
        VFXobj.transform.localPosition = Vector3.zero;
        VFXobj.SetActive(true);

    }

    public void AddVFX(GameObject vfx, Unit target) // hit & Crit VFX (오브젝트풀링 사용)
    {
        //GameObject VFXobj = hitVFXPool.GetVFX(vfx, this);
        //if (VFXobj == null)
        //    return;

        //Camera mainCamera = Camera.main;
        //Vector3 direction = (mainCamera.transform.position - target.transform.position).normalized;

        //float distance = 1f; // VFX를 타겟에서 얼마나 떨어뜨릴지 결정하는 거리

        //Vector3 spawnPosition = target.transform.position + direction * distance;
        //VFXobj.transform.position = spawnPosition;
        //VFXobj.transform.SetParent(VFXParent);
        ////VFXobj.transform.localPosition = Vector3.up * VFXobj.transform.localPosition.y;

        ////VFXobj.transform.forward = dir;
        ////VFXobj.transform.localPosition = Vector3.zero;
        //VFXobj.SetActive(true);

        GameObject VFXobj = hitVFXPool.GetVFX(vfx, this);
        if (VFXobj == null)
            return;

        Camera mainCamera = Camera.main;

        // 1. 카메라와 타겟의 3차원 방향을 구합니다.
        Vector3 direction = (mainCamera.transform.position - target.transform.position).normalized;

        // [핵심] Y축 변화를 제거하여 VFX가 공중에 뜨거나 땅에 파묻히지 않게 만듭니다.
        direction.y = 0f;
        direction.Normalize(); // Y를 0으로 만들었으니 벡터를 다시 정규화해줍니다.

        float distance = 1f; // 카메라 앞쪽으로 살짝 밀어줄 거리 (1m는 생각보다 멉니다)

        // 2. 평면 기준으로 카메라 방향 전진 + 타겟의 중심 높이(예: Vector3.up)를 고려하여 스폰 위치 계산
        // target.transform.position에 보통 발밑이 기준이라면 Vector3.up * 1f 등을 더해 가슴 높이로 맞추는 것이 좋습니다.
        Vector3 spawnPosition = target.transform.position + (direction * distance) + (Vector3.up * 1.5f);

        VFXobj.transform.position = spawnPosition;
        VFXobj.transform.SetParent(VFXParent);

        // 3. VFX가 카메라를 똑바로 바라보도록 회전 (선택 사항)
        // 2D 텍스처 형태의 이펙트라면 이 코드가 있어야 카메라에서 이펙트가 정면으로 보입니다.
        //VFXobj.transform.lookAt(mainCamera.transform);

        VFXobj.SetActive(true);

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


    public void ActivateSpecialAbility(ActiveType activeType)
    {
        if(specialAbility != null && activeType == specialAbility.ActiveCondition)
        {
            specialAbility.Activate(this);
        }
    }

    public void UpdateState()
    {
        if(selectedUnitUI != null)
        {
            selectedUnitUI.UpdateUnitStateUI();
        }
    }

    public void StopUnit()
    {
        isStop = true;

        if (navAgent.enabled)
            navAgent.isStopped = true;
        modelAnimator.SetBool("isRunning", false);
    }

    public float SlotIndexToPosOffset(int slot)
    {
        switch (slot)
        {
            case 0: return 0f;   // 중앙
            case 1: return -1f;  // 왼쪽
            case 2: return 1f;   // 오른쪽
            default: return 0f;
        }
    }

    public EffectImage ApplyEffectImage(Sprite icon, bool hasStack, int stack)
    {
        int slot = GetEmptyEffectImageSlot();
        if (slot == -1)
            return null; // 자리 없음

        EffectImage effectImage = effectImagePool.GetEffectImage();
        effectImage.Initialize(this);
        effectImage.SetIcon(icon);
        effectImage.SetStack(hasStack, stack);

        float posOffset = SlotIndexToPosOffset(slot);
        effectImage.SetXOffset(posOffset);

        effectImage.gameObject.SetActive(true);

        effectImages[slot] = effectImage;

        return effectImage;

    }

    public void RemoveEffectImage(EffectImage effectImage)
    {
        if (effectImage == null)
            return;

        // 배열에서 제거
        for (int i = 0; i < effectImages.Length; i++)
        {
            if (effectImages[i] == effectImage)
            {
                effectImages[i] = null;
                break;
            }
        }

        //effectImage.Disappear();
        effectImagePool.ReturnEffectImage(effectImage);
        //effectImage.gameObject.SetActive(false);

    }

    private int GetEmptyEffectImageSlot()
    {
        for (int i = 0; i < effectImages.Length; i++)
        {
            if (effectImages[i] == null)
                return i;
        }
        return -1; // 자리 없음
    }

    public void ReapplyEffectImage(EffectImage effectImage, bool hasStack, int stack)
    {
        effectImage.Initialize(this);
        effectImage.SetStack(hasStack, stack);
    }

    public void PlayAnimation(string stateName)
    {
        if(stateAnimDic.ContainsKey(stateName))
        {
            AnimationClip[] arr = stateAnimDic[stateName];
            AnimationClip clip = arr[Random.Range(0, arr.Length)];
            if (clip != null)
            {
                AnimatorOverrideController aoc = modelAnimator.runtimeAnimatorController as AnimatorOverrideController;
                aoc[stateName] = clip;
            }
        }

        modelAnimator.SetTrigger(stateName);
    }

    protected void Rotation(Transform targetTr)
    {
        Vector3 direction = targetTr.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
    }
    public virtual void SetDeferredState()
    {
        isDeferredState = true;
        //targetUnit = null;
        currentSkill = null;
        if(navAgent.enabled)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
        }
         
        modelAnimator.SetBool("isRunning", false);

        if(deferredStateVFX == null)
        {
            deferredStateVFX = Resources.Load<GameObject>("Prefabs/VFX/VFX_provoked/VFX_provoked_02");
            deferredStateObj = Instantiate(deferredStateVFX, HeightPos);
        }

        deferredStateObj.SetActive(true);
    }


    public void ChangeArmorType(ArmorType armorType)
    {
        this.armorType = armorType;
    }

    //public float CalculateCurseEffectProbability(int unitMental, int targetMental)
    //{
    //    int mentalDifference = unitMental - targetMental;

    //    switch (mentalDifference)
    //    {
    //        case 4: return 0.8f;
    //        case 3: return  0.6f; 
    //        case 2: return  0.4f; 
    //        case 1: return  0.2f; 
    //        case 0: return  0f;
    //        case -1: return -0.2f;
    //        case -2: return -0.4f; 
    //        case -3: return -0.6f; 
    //        case -4: return -0.8f;
    //        default:
    //            // 4보다 크면 80f, 그 외(-4보다 작으면) -80f 반환
    //            if (mentalDifference > 4) return 0.8f;
    //            else return -0.8f;
    //    }

    //}
}