using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Unit : MonoBehaviour
{
    public UnitData_ReBuild unitData;
    [SerializeField] protected float curHP;
    protected float curAttackDamage;
    protected float curCrit;
    protected float curMoveSpeed;
    //protected float curAttackCooldown;
    protected GameObject targetEnemy;
    public Collider SightRangeCollider;
    public float AttackRangeDistance;

    private Vector3 moveTargetPos;

    public void InitStats()
    {
        //스탯 초기화
    }


    public virtual void Attack()
    {
        Debug.Log("시야범위 안에 들어온 적을 바라보며 공격 수행.");
        UnitCtrl_ReBuild EnemyCtrl = targetEnemy.GetComponent<UnitCtrl_ReBuild>();
        
        bool isCritical = Random.Range(0f, 1f) <= curCrit * 0.01f;
        if (isCritical)
        {
            //디버프 적용
        }

    }

    public virtual void Move(Vector3 TargetPos)
    {
        moveTargetPos = TargetPos;
        //NavAgent의 목표지정.
        Debug.Log("특정 지점을 향해 이동.");
    }

    public virtual void SearchEnemy()
    {
        Debug.Log("검색 조건에 따른 적 검색.");
        //unitData.targetSelectType
    }

    public virtual void TakeDamage(int Damage)
    {
        Debug.Log("적의 공격을 받아 체력 감소.");
    }

    public virtual void Die()
    {
        Debug.Log("체력이 0이 되어 죽음.");
    }

    public virtual void GetUnactable()
    {
        Debug.Log("유닛이 행동 불가 상태로 전환.");
    }

    public virtual void ChangeMoveSpeed(float Speed)
    {
        curMoveSpeed = Speed;
    }

    //public virtual void ChangeAttackCooldown(float Cooldown)
    //{
    //    curAttackCooldown = Cooldown;
    //}

    public virtual void ChangeAttackDamage(int Damage)
    {
        curAttackDamage = Damage;
    }
}

public class AllyUnit : Unit
{
    protected enum UnitMode
    {
        Free,
        Seige
    }

    protected int cost;
    protected UnitMode unitMode;

    protected void ChangeMode(UnitMode mode)
    {
        Debug.Log("모드 변경.");
    }

    protected void Upgrade()
    {
        Debug.Log("유닛 업그레이드.");
    }
}

public class EnemyUnit : Unit
{
    protected int gold; //처치시 골드 획득량

    protected void SearchPath()
    {
        Debug.Log("성까지의 경로를 탐색.");
    }

}




public class UnitCtrl_ReBuild : Unit
{

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            EnemyUnit enemyUnit = new EnemyUnit();
            enemyUnit.InitStats();
        }
        else if (this.gameObject.CompareTag(CONSTANT.TAG_UNIT))
        {
            AllyUnit allyUnit = new AllyUnit();
            allyUnit.InitStats();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
