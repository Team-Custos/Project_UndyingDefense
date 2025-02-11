using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum UnitState
{
    Idle,
    Attack,
    Move,
    Dead
}

public class Unit : MonoBehaviour
{
    public System.Action<GameObject> OnDisable;
    public System.Action<Unit> OnUnitDead;

    [SerializeField] protected Skill generalSkill;
    [SerializeField] private Skill specialSkill;

    protected NavMeshAgent navAgent;
    protected NavMeshObstacle navObstacle;
    public UnitData_ReBuild unitData;
    public GameObject VisualModel;
    [SerializeField] protected float curHP;
    public float curCrit;
    protected float curMoveSpeed;
    protected float curAttackDamage;
    protected float curAttackCooldown;
    public Vector2 unitPos;

    public UnitState unitState;
    [SerializeField] protected GameObject targetEnemy;
    public Collider SightRangeCollider;
    public float AttackRangeDistance;

    protected Vector3 moveTargetPos;

    public UnitSound_Rebuild soundManager;

    public void InitStats()
    {
        //스탯 초기화
        curHP = unitData.maxHP;
        curMoveSpeed = unitData.baseMoveSpeed;
        curCrit = unitData.baseCritChanceRate;
        //curAttackCooldown = unitData.baseAttackCooldown;
    }

    public virtual void GeneralSkillAttack()
    {
        Debug.Log("시야범위 안에 들어온 적을 바라보며 공격 수행.");
        UnitCtrl_ReBuild EnemyCtrl = targetEnemy.GetComponent<UnitCtrl_ReBuild>();
        LookAtTarget(targetEnemy.transform.position);
        int HitSoundRandomNum = Random.Range(0, 2);
        //AudioClip SFX2Play = unitData.attackSound[HitSoundRandomNum];
        //soundManager.PlaySFX(SFX2Play);

        //유닛 스킬 실행
        generalSkill.Activate(EnemyCtrl);

        bool isCritical = Random.Range(0f, 1f) <= curCrit * 0.01f;
        if (isCritical)
        {
            generalSkill.AddDebuff(EnemyCtrl);
        }
    }

    public virtual void SpecialSkillAttack()
    {
        if (specialSkill != null)
        {
            Debug.Log("특수 스킬 공격 수행.");
            UnitCtrl_ReBuild EnemyCtrl = targetEnemy.GetComponent<UnitCtrl_ReBuild>();
            LookAtTarget(targetEnemy.transform.position);

            specialSkill.Activate(EnemyCtrl);

            bool isCritical = Random.Range(0f, 1f) <= curCrit * 0.01f;
            if (isCritical)
            {
                specialSkill.AddDebuff(EnemyCtrl);
            }
        }
    }

    public void Debuff(UnitDebuff unitDebuff = UnitDebuff.None)
    {
        Debug.Log("적에게 디버프 적용.");

    }

    public virtual void Move(Vector3 TargetPos)
    {
        //NavAgent의 목표지정.
        Debug.Log("특정 지점을 향해 이동.");

        navAgent.SetDestination(TargetPos);
    }

    public void LookAtTarget(Vector3 targetPos)
    {
        Debug.Log("적을 바라보는 방향으로 회전.");
        if (targetEnemy != null)
        {
            Vector3 dir = targetEnemy.transform.position - this.transform.position;
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * 2f);
        }
    }

    public void SearchEnemy()
    {
        Debug.Log("검색 조건에 따른 적 검색.");
        if (SightRangeCollider == null)
        {
            Debug.LogError("Range NullError in : " + this.gameObject.name);
            return;
        }
        else
        {
            GameObject TargetObj = SightRangeCollider.GetComponent<RangeCtrl>().FinalTarget;
        }
    }

    public virtual void TakeDamage(float Damage)
    {
        Debug.Log("적의 공격을 받아 체력 감소.");
        this.curHP -= Damage;

        if (curHP <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log("체력이 0이 되어 죽음.");
        //죽음 사운드 재생

        //if (this.gameObject == GameOrderSystem.instance.selectedUnit)
        //{
        //    Ingame_UIManager.instance.unitInfoPanel.SetActive(false);
        //}

        unitState = UnitState.Dead;

        // OnUnitDead?.Invoke(this);

        return;
    }

    public void GetUnactable()
    {
        Debug.Log("유닛이 행동 불가 상태로 전환.");
    }

    public void ChangeMoveSpeed(float Speed)
    {
        curMoveSpeed = Speed;
    }

    //public virtual void ChangeAttackCooldown(float Cooldown)
    //{
    //    curAttackCooldown = Cooldown;
    //}

    public void ChangeAttackDamage(int Damage)
    {
        curAttackDamage = Damage;
    }
}

public class PlayerUnit : Unit
{
    protected enum UnitMode
    {
        Free,
        Seige
    }
    public AllyUnitState Ally_State;
    protected int cost;
    protected UnitMode unitMode;

    protected void ChangeMode(UnitMode mode)
    {
        Debug.Log("모드 변경.");
    }

    public override void Move(Vector3 TargetPos)
    {
        if (unitMode == UnitMode.Free)
        {
            base.Move(TargetPos);
        }
    }

    protected void Upgrade()
    {
        Debug.Log("유닛 업그레이드.");
    }

    public override void Die()
    {
        Debug.Log("아군이 죽음.");
        Ingame_UIManager.instance.DestroyUnitStateChangeBox();
        Ingame_UIManager.instance.DestroyUnitUpgradeMenu();
        Ingame_UIManager.instance.DestorypgradeMenuConfirmBox();

        base.Die();
    }
}

public class NonPlayerUnit : Unit
{
    protected int gold; //처치시 골드 획득량

    protected void SearchPath()
    {
        Debug.Log("성까지의 경로를 탐색.");
        if (navAgent.enabled == false)
        {
            navAgent.enabled = true;
        }

        NavMeshPath calcaulatedPath = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, moveTargetPos, NavMesh.AllAreas, calcaulatedPath))
        {
            if (calcaulatedPath.status != NavMeshPathStatus.PathComplete)
            {
                Debug.Log("Can not Find Path");
                //UnitCtrl.enemy_isPathBlocked = true;
            }
            else
            {
                Debug.Log("Find Path Success");
                //UnitCtrl.enemy_isPathBlocked = false;
            }
        }
        else
        {
            Debug.Log("Can not Find Path");
            //UnitCtrl.enemy_isPathBlocked = true;
        }
    }

    public override void Die()
    {
        Debug.Log("적이 죽음.");
        //게임 매니저에게 골드 획득 알림
        Ingame_ParticleManager.Instance.EnemyDeathEffect(this.transform);
        EnemySpawner.inst.OnMonsterDead(this.gameObject);
        InGameManager.inst.gold += gold;
        Ingame_UIManager.instance.goldTxt.text = InGameManager.inst.gold.ToString();
        base.Die();
    }

}

public class UnitCtrl_ReBuild : Unit
{
    public UnitCtrl_ReBuild GetUnitCtrl()
    {
        return this;
    }

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navObstacle = GetComponent<NavMeshObstacle>();
    }


    // Start is called before the first frame update
    void Start()
    {
        InitStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            generalSkill.AddDebuff(this);
        }


        if (unitState == UnitState.Idle)
        {
            SearchEnemy();
            if (targetEnemy != null)
            {
                unitState = UnitState.Move;
            }
        }
        else if (unitState == UnitState.Move)
        {
            if (targetEnemy != null)
            {
                //moveTargetPos = targetEnemy.transform.position;
                Move(targetEnemy.transform.position);

                if(navAgent.remainingDistance < AttackRangeDistance)
                {
                    navAgent.ResetPath();
                    unitState = UnitState.Attack;
                }
            }
            else if(Vector3.Distance(this.transform.position, moveTargetPos) <= 0.1f)
            {
                navAgent.ResetPath();
                this.transform.position = moveTargetPos;
                unitState = UnitState.Idle;
            }
        }
        else if (unitState == UnitState.Attack)
        {
            if (targetEnemy != null)
            {
                LookAtTarget(targetEnemy.transform.position);
                GeneralSkillAttack();
            }
            else
            {
                unitState = UnitState.Idle;
            }
        }
    }
}
