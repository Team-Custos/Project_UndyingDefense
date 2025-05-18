#region 250213_기존 코드
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.AI;

//public enum UnitState
//{
//    Idle,
//    Attack,
//    Move,
//    Dead
//}

//public class Unit : MonoBehaviour
//{
//    public System.Action<GameObject> OnDisable;
//    public System.Action<Unit> OnUnitDead;

//    [SerializeField] protected Skill generalSkill;
//    [SerializeField] private Skill specialSkill;

//    protected NavMeshAgent navAgent;
//    protected NavMeshObstacle navObstacle;
//    public UnitData_ReBuild unitData;
//    public GameObject VisualModel;
//    [SerializeField] protected float curHP;
//    public float curCrit;
//    protected float curMoveSpeed;
//    protected float curAttackDamage;
//    protected float curAttackCooldown;
//    public Vector2 unitPos;

//    public UnitState unitState;
//    [SerializeField] protected GameObject targetEnemy;
//    public Collider SightRangeCollider;
//    public float AttackRangeDistance;

//    protected Vector3 moveTargetPos;

//    public UnitSound_Rebuild soundManager;

//    public void InitStats()
//    {
//        //스탯 초기화
//        curHP = unitData.maxHP;
//        curMoveSpeed = unitData.baseMoveSpeed;
//        curCrit = unitData.baseCritChanceRate;
//        //curAttackCooldown = unitData.baseAttackCooldown;
//    }

//    public virtual void GeneralSkillAttack()
//    {
//        Debug.Log("시야범위 안에 들어온 적을 바라보며 공격 수행.");
//        UnitCtrl_ReBuild EnemyCtrl = targetEnemy.GetComponent<UnitCtrl_ReBuild>();
//        LookAtTarget(targetEnemy.transform.position);
//        int HitSoundRandomNum = Random.Range(0, 2);
//        //AudioClip SFX2Play = unitData.attackSound[HitSoundRandomNum];
//        //soundManager.PlaySFX(SFX2Play);

//        //유닛 스킬 실행
//        generalSkill.Activate(EnemyCtrl);

//        bool isCritical = Random.Range(0f, 1f) <= curCrit * 0.01f;
//        if (isCritical)
//        {
//            generalSkill.AddDebuff(EnemyCtrl);
//        }
//    }

//    public virtual void SpecialSkillAttack()
//    {
//        if (specialSkill != null)
//        {
//            Debug.Log("특수 스킬 공격 수행.");
//            UnitCtrl_ReBuild EnemyCtrl = targetEnemy.GetComponent<UnitCtrl_ReBuild>();
//            LookAtTarget(targetEnemy.transform.position);

//            specialSkill.Activate(EnemyCtrl);

//            bool isCritical = Random.Range(0f, 1f) <= curCrit * 0.01f;
//            if (isCritical)
//            {
//                specialSkill.AddDebuff(EnemyCtrl);
//            }
//        }
//    }

//    public void Debuff(UnitDebuff unitDebuff = UnitDebuff.None)
//    {
//        Debug.Log("적에게 디버프 적용.");

//    }

//    public virtual void Move(Vector3 TargetPos)
//    {
//        //NavAgent의 목표지정.
//        Debug.Log("특정 지점을 향해 이동.");

//        navAgent.SetDestination(TargetPos);
//    }

//    public void LookAtTarget(Vector3 targetPos)
//    {
//        Debug.Log("적을 바라보는 방향으로 회전.");
//        if (targetEnemy != null)
//        {
//            Vector3 dir = targetEnemy.transform.position - this.transform.position;
//            dir.y = 0;
//            Quaternion rot = Quaternion.LookRotation(dir);
//            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * 2f);
//        }
//    }

//    public void SearchEnemy()
//    {
//        Debug.Log("검색 조건에 따른 적 검색.");
//        if (SightRangeCollider == null)
//        {
//            Debug.LogError("Range NullError in : " + this.gameObject.name);
//            return;
//        }
//        else
//        {
//            GameObject TargetObj = SightRangeCollider.GetComponent<RangeCtrl>().FinalTarget;
//        }
//    }

//    public virtual void TakeDamage(float Damage)
//    {
//        Debug.Log("적의 공격을 받아 체력 감소.");
//        this.curHP -= Damage;

//        if (curHP <= 0)
//        {
//            Die();
//        }
//    }

//    public virtual void Die()
//    {
//        Debug.Log("체력이 0이 되어 죽음.");
//        //죽음 사운드 재생

//        //if (this.gameObject == GameOrderSystem.instance.selectedUnit)
//        //{
//        //    Ingame_UIManager.instance.unitInfoPanel.SetActive(false);
//        //}

//        unitState = UnitState.Dead;

//        // OnUnitDead?.Invoke(this);

//        return;
//    }

//    public void GetUnactable()
//    {
//        Debug.Log("유닛이 행동 불가 상태로 전환.");
//    }

//    public void ChangeMoveSpeed(float Speed)
//    {
//        curMoveSpeed = Speed;
//    }

//    //public virtual void ChangeAttackCooldown(float Cooldown)
//    //{
//    //    curAttackCooldown = Cooldown;
//    //}

//    public void ChangeAttackDamage(int Damage)
//    {
//        curAttackDamage = Damage;
//    }
//}

//public class PlayerUnit : Unit
//{
//    protected enum UnitMode
//    {
//        Free,
//        Seige
//    }
//    public AllyUnitState Ally_State;
//    protected int cost;
//    protected UnitMode unitMode;

//    protected void ChangeMode(UnitMode mode)
//    {
//        Debug.Log("모드 변경.");
//    }

//    public override void Move(Vector3 TargetPos)
//    {
//        if (unitMode == UnitMode.Free)
//        {
//            base.Move(TargetPos);
//        }
//    }

//    protected void Upgrade()
//    {
//        Debug.Log("유닛 업그레이드.");
//    }

//    public override void Die()
//    {
//        Debug.Log("아군이 죽음.");
//        Ingame_UIManager.instance.DestroyUnitStateChangeBox();
//        Ingame_UIManager.instance.DestroyUnitUpgradeMenu();
//        Ingame_UIManager.instance.DestorypgradeMenuConfirmBox();

//        base.Die();
//    }
//}

//public class NonPlayerUnit : Unit
//{
//    protected int gold; //처치시 골드 획득량

//    protected void SearchPath()
//    {
//        Debug.Log("성까지의 경로를 탐색.");
//        if (navAgent.enabled == false)
//        {
//            navAgent.enabled = true;
//        }

//        NavMeshPath calcaulatedPath = new NavMeshPath();
//        if (NavMesh.CalculatePath(transform.position, moveTargetPos, NavMesh.AllAreas, calcaulatedPath))
//        {
//            if (calcaulatedPath.status != NavMeshPathStatus.PathComplete)
//            {
//                Debug.Log("Can not Find Path");
//                //UnitCtrl.enemy_isPathBlocked = true;
//            }
//            else
//            {
//                Debug.Log("Find Path Success");
//                //UnitCtrl.enemy_isPathBlocked = false;
//            }
//        }
//        else
//        {
//            Debug.Log("Can not Find Path");
//            //UnitCtrl.enemy_isPathBlocked = true;
//        }
//    }

//    public override void Die()
//    {
//        Debug.Log("적이 죽음.");
//        //게임 매니저에게 골드 획득 알림
//        Ingame_ParticleManager.Instance.EnemyDeathEffect(this.transform);
//        EnemySpawner.inst.OnMonsterDead(this.gameObject);
//        InGameManager.inst.gold += gold;
//        Ingame_UIManager.instance.goldTxt.text = InGameManager.inst.gold.ToString();
//        base.Die();
//    }

//}

//public class UnitCtrl_ReBuild : Unit
//{
//    public UnitCtrl_ReBuild GetUnitCtrl()
//    {
//        return this;
//    }

//    private void Awake()
//    {
//        navAgent = GetComponent<NavMeshAgent>();
//        navObstacle = GetComponent<NavMeshObstacle>();
//    }


//    // Start is called before the first frame update
//    void Start()
//    {
//        InitStats();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.B))
//        {
//            generalSkill.AddDebuff(this);
//        }


//        if (unitState == UnitState.Idle)
//        {
//            SearchEnemy();
//            if (targetEnemy != null)
//            {
//                unitState = UnitState.Move;
//            }
//        }
//        else if (unitState == UnitState.Move)
//        {
//            if (targetEnemy != null)
//            {
//                //moveTargetPos = targetEnemy.transform.position;
//                Move(targetEnemy.transform.position);

//                if (navAgent.remainingDistance < AttackRangeDistance)
//                {
//                    navAgent.ResetPath();
//                    unitState = UnitState.Attack;
//                }
//            }
//            else if (Vector3.Distance(this.transform.position, moveTargetPos) <= 0.1f)
//            {
//                navAgent.ResetPath();
//                this.transform.position = moveTargetPos;
//                unitState = UnitState.Idle;
//            }
//        }
//        else if (unitState == UnitState.Attack)
//        {
//            if (targetEnemy != null)
//            {
//                LookAtTarget(targetEnemy.transform.position);
//                GeneralSkillAttack();
//            }
//            else
//            {
//                unitState = UnitState.Idle;
//            }
//        }
//    }
//}
#endregion

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AttackType = AttackSkill.AttackType;

public abstract class Unit : MonoBehaviour
{
    public enum ArmorType           //유닛의 방어속성.
    {
        PADDED,         // 완충갑
        ANTIPIERCING,   // 방탄갑
        STEELPLATED     // 철갑
    }

    [Header("■ Components")]
    [SerializeField] protected Animator modelAnimator;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected NavMeshObstacle navObstacle;
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

    protected float maxhp;
    protected float hp;
    protected float critChance;
    protected float critVulnerability; // 치명타를 받을 확률.
    protected float attackSpeed;
    protected float mental; // 정신력
    // protected float moveSpeed;
    protected float attackSpeedMultiplier;
    protected float moveSpeedMultiplier;
    protected float blockRate; // 방어 계수(방어 상성으로 감소하는 수치의 비율)
    protected float damageReductionMultiplier; // 피해량 감소 비율
    protected float attackDamageMultiplier; // 공격력 증가 비율

    protected Collider[] collidersInRange = new Collider[maxTargetCount];
    protected List<Unit> targets = new List<Unit>(); // 탐색 조건을 만족하는 대상들. (조건에 만족하는 대상이 여러 개일 경우 사용)

    //protected Unit skillTarget; // 공격 대상
    //protected Unit chaseTarget; // 추격 대상
    protected Unit targetUnit;

    private float lastMoveTime;

    protected NavMeshPath path; // 경로 설정용
    protected NavMeshPath pathForSearch; // 경로 탐색용

    protected float stateDuration;
    protected float stateDurationCheck;

    private List<Effect> effectList = new List<Effect>();



    protected const int maxTargetCount = 10;

    protected bool isSelected;

    protected const float moveThresholdOnStop = float.MaxValue;

    protected bool isDead;

    public Transform EffectParent => effectParent;

    public abstract UnitData Data { get; }
    public float Maxhp => maxhp;
    public float Hp => hp;
    public float HpPercent => hp / Data.MaxHp;// * 100f;
    public float Mental => mental;
    public float CritChance => critChance;
    public float CritVulnerability => critVulnerability;
    public float BlockRate => blockRate;
    public float DamageReductionMultiplier => damageReductionMultiplier;
    public float AttackDamageMultiplier => attackDamageMultiplier;
    public LayerMask EnemyLayer => enemyLayer;
    public SkillBase GeneralSkill => generalSkill;
    public SkillBase SpecialSkill => specialSkill;

    public List<Effect> EffectList => effectList;
    public bool IsSelected
    {
        get => isSelected;
        set => isSelected = value;
    }

    protected SelectedUnitUI selectedUnitUI;

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

        hp = Data.MaxHp;
        critChance = Data.CritChance;
        critVulnerability = 0f;
        blockRate = 1f;

        // 이동 속도
        moveSpeedMultiplier = 1f;
        navAgent.speed = Data.MoveSpeed * moveSpeedMultiplier;

        attackSpeedMultiplier = 1f;
        attackSpeed = Data.AttackSpeed;

        navObstacle.carvingMoveThreshold = moveThresholdOnStop;

        navAgent.enabled = false;
        navObstacle.enabled = true;
        collider.enabled = true;


        lastMoveTime = Time.time;
    }

    public void SetUnitUI(SelectedUnitUI selectedUnitUI)
    {
        this.selectedUnitUI = selectedUnitUI;
    }

    protected virtual void Update()
    {
        PassiveSkillCheck();

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
            passiveSkill.Activate(this, targetUnit);
        }
    }

    protected virtual void ActivateSkill(SkillBase skill, Unit target)
    {
        skill.Activate(this, target);
    }

    protected bool IsReachable(Vector3 pos)
    {
        navAgent.CalculatePath(pos, pathForSearch);
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
        if (specialSkill != null && specialSkill.IsCoolDown)
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

    public virtual void MoveTo(Vector3 pos)
    {
        bool navAgentEnabled = navAgent.enabled;
        if (!navAgentEnabled)
        {
            navObstacle.enabled = false;
            navAgent.enabled = true;
        }

        navAgent.CalculatePath(pos, path); // 경로 계산
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            if (navAgent.isStopped)
                navAgent.isStopped = false;

            navObstacle.carvingMoveThreshold = Data.MoveSpeed * 0.1f;
            navAgent.SetPath(path);
            lastMoveTime = Time.time;
            return;
        }

        // 경로 계산 이전에 navAgent가 비활성화 상태였을 경우
        if (!navAgentEnabled)
        {
            // 다시 비활성화 상태로 원상복구.
            navAgent.enabled = false;
            navObstacle.enabled = true;
        }
    }

    public virtual void ForceMoveTo(Vector3 pos)
    {
        bool navAgentEnabled = navAgent.enabled;
        if (!navAgentEnabled)
        {
            navObstacle.enabled = false;
            navAgent.enabled = true;
        }

        if (navAgent.CalculatePath(pos, path))
        {
            if (navAgent.isStopped)
                navAgent.isStopped = false;

            navObstacle.carvingMoveThreshold = Data.MoveSpeed * 0.1f;
            navAgent.SetPath(path);
            lastMoveTime = Time.time;
            return;
        }

        // 경로 계산 이전에 navAgent가 비활성화 상태였을 경우
        if (!navAgentEnabled)
        {
            // 다시 비활성화 상태로 원상복구.
            navAgent.enabled = false;
            navObstacle.enabled = true;
        }
    }

    public virtual void MoveTo(Unit target)
    {
        bool navAgentEnabled = navAgent.enabled;
        if (!navAgentEnabled) // navAgent가 비활성화 상태일 경우
        {
            navObstacle.enabled = false;
            navAgent.enabled = true;
        }

        Vector3 startDir = (transform.position - target.transform.position).normalized;
        for (float i = 0f; i < 6f; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(60f * i, Vector3.up) * startDir;
            Vector3 targetPos = target.transform.GetNearPosition(dir, target.nearbyDistance);
            navAgent.CalculatePath(targetPos, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                if (navAgent.isStopped)
                    navAgent.isStopped = false;

                navObstacle.carvingMoveThreshold = Data.MoveSpeed * 0.1f;
                navAgent.SetPath(path);
                lastMoveTime = Time.time;
                return;
            }
        }

        // 경로 계산 이전에 navAgent가 비활성화 상태였을 경우
        if (!navAgentEnabled)
        {
            // 다시 비활성화 상태로 원상복구.
            navAgent.enabled = false;
            navObstacle.enabled = true;
        }
    }

    public void LookAt(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        dir.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
    }

    public void PlayHitSFX(AttackType attackType)
    {
        AudioClip[] hitSFX = null;
        switch (attackType)
        {
            case AttackType.SLASH:
                hitSFX = SlashHitSFX;
                break;
            case AttackType.PIERCE:
                hitSFX = PierceHitSFX;
                break;
            case AttackType.CRUSH:
                hitSFX = CrushHitSFX;
                break;
        }
        if (hitSFX != null)
        {
            int randomIndex = Random.Range(0, hitSFX.Length);
            SoundManager.Instance.PlaySFX(hitSFX[randomIndex]);
        }
    }

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
            hp = 0f;
            Die();

            if (selectedUnitUI != null)
            {
                // ui 제거
                selectedUnitUI.HideHp();
                selectedUnitUI.HideAllyUI();
                selectedUnitUI.HideUpgrdeUI();
                selectedUnitUI.HideUntInfo();
            }

        }

        if (selectedUnitUI != null)
        {
            selectedUnitUI.UpdateHPUI(this);
        }
    }

    public virtual void Die()
    {
        if (!isDead)
        {
            navAgent.enabled = false;
            navObstacle.enabled = false;
            collider.enabled = false;

            modelAnimator.SetTrigger("Die");

            isDead = true;
        }
    }

    public void SetStateDuration(float duration) => stateDuration = duration;

    public void AddMoveSpeedMultiplier(float value)
    {
        moveSpeedMultiplier += value;
        if (moveSpeedMultiplier < 0f)
            moveSpeedMultiplier = 0f;

        navAgent.speed = Data.MoveSpeed * moveSpeedMultiplier;
    }

    public void AddMental(float amount)
    {
        mental += amount;
    }

    public void AddAttackSpeedMultiplier(float value)
    {
        attackSpeedMultiplier += value;
        if (attackSpeedMultiplier < 0f)
            attackSpeedMultiplier = 0f;

        attackSpeed = Data.AttackSpeed * attackSpeedMultiplier;
    }

    public void AddCriticalVulnerability(float amount)
    {
        critVulnerability += amount;
    }
    public void AddBlockRate(float amount)
    {
        blockRate += amount;
    }

    public void AddAttackSpeed(float speed)
    {
        attackSpeed += speed;
    }

    public void AddAdditionalDamage(float percent)
    {
        // 추가 피해량
        attackDamageMultiplier += percent;
    }

    public void AddDamageReduction(float percent)
    {
        // 받는 피해량 감소
        damageReductionMultiplier += percent;
    }

    public abstract void GetProvoked(Unit ProvokedTarget);

    public virtual void RemoveProvoked()
    { }

    public virtual void GetStun()
    {
        modelAnimator.SetBool("isStun", true);
        navAgent.speed = 0f;
    }

    public virtual void RemoveStun()
    {
        modelAnimator.SetBool("isStun", false);
        navAgent.speed = Data.MoveSpeed * moveSpeedMultiplier;
    }



    public void AddEffect(Unit unit, Effect effect)
    {
        Effect prevEffect = effectList.Find(effect.IsSameEffect);
        Effect prevMaxStackEffect = effectList.Find(effect.IsMaxStackEffect);

        if (prevMaxStackEffect != null && prevMaxStackEffect.gameObject.activeInHierarchy)
            return;

        // 효과 목록 중에 오브젝트로서 이미 추가된 적이 있는 효과가 존재할 경우.
        if (prevEffect != null)
        {
            if (!prevEffect.gameObject.activeInHierarchy)
            {
                prevEffect.gameObject.SetActive(true);
                prevEffect.Initialize();
            }
            else
            {
                prevEffect.AddStack();
            }
            prevEffect.Activate();
        }
        else //맨 처음 효과 오브젝트가 추가될 때.
        {
            GameObject obj = Instantiate(effect.gameObject);
            obj.transform.SetParent(effectParent);
            effect = obj.GetComponent<Effect>();
            effect.Initialize(unit, this);
            effect.Activate();

            effectList.Add(effect);
        }

        UpdateState();

        //Effect prevEffect = effectsList.Find(item => item.Id == effect.Id);
        //if (prevEffect != null)
        //{
        //    if(!prevEffect.gameObject.activeInHierarchy)
        //    {
        //        prevEffect.gameObject.SetActive(true);
        //    }
        //    prevEffect.AddStack();
        //    prevEffect.Activate();

        //    //effectsList.Add(effect);
        //    UpdateState();
        //}
        //else
        //{
        //    Effect maxStackEffect = effectsList.Find(effect.HasMaxStackEffect);
        //    if (maxStackEffect == null)
        //    {
        //        GameObject obj = Instantiate(effect.gameObject);
        //        obj.transform.SetParent(effectParent);
        //        effect = obj.GetComponent<Effect>();
        //        effect.Initialize(unit, this);
        //        effect.Activate();

        //        if (effect is DurationEffect)
        //        {
        //            DurationEffect durationEffect = effect as DurationEffect;

        //            effectsList.Add(durationEffect);

        //            UpdateState();
        //        }
        //    }
        //}
    }

    public void AddVFX(ParticleSystem VFX)
    {
        GameObject VFXobj = Instantiate(VFX.gameObject);
        VFXobj.transform.SetParent(VFXParent);
        VFXobj.transform.localPosition = Vector3.zero + Vector3.up * VFXobj.transform.localPosition.y;
        VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Destroy(VFXobj, VFX.main.duration);
    }

    public void UpdateState()
    {
        if(selectedUnitUI != null)
        {
            selectedUnitUI.UpdateUnitStateUI();
        }
    }

}