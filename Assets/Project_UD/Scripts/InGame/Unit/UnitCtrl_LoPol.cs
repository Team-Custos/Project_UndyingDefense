using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnitDataManager;

public enum newUnitState
{
    Idle,   // 기본 상태(대기)
    Attack, // 공격
    Move,   // 이동 (추적)
    Dead    // 사망
}

public class UnitCtrl_LoPol : MonoBehaviour
{
    public UnitData_LoPol unitData;   // 유닛의 기본 데이터, ScriptableObject 사용

    [Header("====Unit Stats====")]
    public float Hp;                       // 유닛의 현재 체력
    public float Damage;                 // 유닛의 현재 공격력
    public float CritChance;             // 유닛의 현재 치명타율
    public float MoveSpeed;              // 유닛의 현재 이동속도
    public float AttackCooldown;         // 유닛의 현재 공겨 쿨타임 -> 추후 공격속도로

    public GameObject targetObject;         // 유닛의 목표 적
    protected Transform targetPosition;        // 유닛의 이동 목표 지점

    public NavMeshAgent navMeshAgent;       // 유닛 이동 AI
    public RangeCtrl rangeCtrl;            // 유닛의 공격 범위 컴포넌트

    

    protected bool isTargetInRange; // 유닛의 공격 범위 내에 적이 있는지 확인
    protected bool isTargetInSight; // 유닛의 시야 범위 내에 적이 있는지 확인


    // Start is called before the first frame update
    void Start()
    {
        InitStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitStats()
    {
        // 유닛의 스텟을 초기화 하는 매서드(체력, 데메지, 치명타율, 이동속도 등)
        Hp = unitData.maxHp;
        Damage = unitData.damage;
        CritChance = unitData.critChance;
        MoveSpeed = unitData.moveSpeed;
        navMeshAgent.speed = MoveSpeed;
    }

    protected virtual Transform Move(Transform targetTransform)
    {
        // 유닛의 이동을 실행하는 매서드

        /* 
         * 1. 이동 명령이 있다면 이동 명령 위치로 이동
         * 2. 이동 명령이 없다면 다른 움직임 실행(공격, 추적 등)
         * 
         */

        if (targetTransform != null) // 이동 명령
        {
            navMeshAgent.SetDestination(targetTransform.position); // 이동
            targetPosition = targetTransform; // 이동 목표 설정
        }
        else
        {
            // 공격, 추적 실행
        }

        return targetPosition;
    }

    protected virtual void Attack()
    {
        // 유닛의 공격을 실행하는 매서드
        if(targetObject != null)
        {
            // 공격 애니메이션 재생

            // 목표 적에게 데미지 적용
            UnitCtrl_LoPol targetUnit = targetObject.GetComponent<UnitCtrl_LoPol>();
            if (targetUnit != null)
            {
                // 적에게 데미지 적용
                targetUnit.ReceiveDamage(Damage);
            }
        }
    }



    protected virtual void SearchEnmey()
    {
        // 공격할 적을 찾는 매서드 

        /*
         * 1. 시야 내에 적이 있는지 확인
         * 2. 적이 있다면 제일 가까운 적을 공격 목표로 설정 -> 공격
         */

        // 시야 범위 내에 적이 있는지 확인
        GameObject nearestTarget = rangeCtrl.NearestObjectSearch();

        if (nearestTarget != null)
        {
            // 가장 가까운 적을 공격 목표로 설정
            targetObject = nearestTarget;

            // 공격 수행
            Attack();
        }
    }

    private void ReceiveDamage(float damage)
    {   // 유닛이 데미지를 받는 매서드
        Hp -= damage;

        if(Hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

        // 유닛이 죽는 매서드
        Destroy(gameObject);
    }

    private void UnAbleToAct()
    { // 유닛이 행동불가 상태가 되는 매서드(기절 등)

        navMeshAgent.isStopped = false;
    }
}

public class AllyUnit : UnitCtrl_LoPol
{
    // 아군 유닛의 클래스,  UnitCtrl_LoPol을 상속받아 사용

    [HideInInspector] public AllyUnitState Ally_State;
    public AllyMode allyMode;  // 유닛의 현재 모드
    private AllyMode previousAllyMode; // 유닛의 이전 모드

    public NavMeshObstacle navMeshObstacle; // 시즈 모드시 유닛이 적 유닛의 길을 막게하는 컴포넌트

    private bool haveToMovePosition; // 유닛이 이동해야하는지 확인

    public int cost;           // 유닛을 소환하는데 필요한 비용

    public float unitStateChangeTime; // 유닛의 모드 변경시 걸리는 시간

    private void ChangeUnitMode(AllyMode newMode)
    { // 아군 유닛의 모드를 변경하는 매서드

        if (allyMode == newMode)
        {
            return;
        }

        allyMode = newMode;

        switch (allyMode)
        {
            case AllyMode.Siege:
                SiegeMode();
                break;
            case AllyMode.Free:
                FreeMode();
                break;
            case AllyMode.Change:
                ChangeMode();
                break;
            default:
                break;
        }
    }

    private void SiegeMode()
    {
        // 시즈모드 상태

        if (targetObject == null)//적군이 시야 범위 내에 없을경우.
        {
            Ally_State.fsm.ChangeState(UnitState.Idle);
            SearchEnmey();
        }
        else
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetObject.transform.position);
            isTargetInRange = (distanceToEnemy <= unitData.attackRange);//적군이 유닛 공격 범위 내에 있는가?

            if (isTargetInRange && Ally_State != null && Ally_State.fsm != null)
            {
                Ally_State.fsm.ChangeState(UnitState.Attack);
            }
        }
    }

    private void FreeMode()
    {
        // 프리모드 상태

        if (haveToMovePosition)//모든 행동을 무시하고 이동해야하는가?
        {
            targetObject = null;
            Vector2 CurPos = new Vector2(transform.position.x, transform.position.z);

            if (Vector2.Distance(CurPos, new Vector2(targetPosition.position.x, targetPosition.position.z)) >= 0.15f)//목적지에 도착할떄까지
            {
                Ally_State.fsm.ChangeState(UnitState.Move);//이동 상태 설정.
            }
            else
            {
                //초기화 후 상태 변경.
                haveToMovePosition = false;
                this.transform.position = targetPosition.position;
                Ally_State.fsm.ChangeState(UnitState.Idle);
            }
        }
        else
        {
            if (targetObject == null)
            {
                Ally_State.fsm.ChangeState(UnitState.Idle);
                SearchEnmey();
            }

            if (targetObject != null)
            {
                float distance = Vector3.Distance(transform.position, targetObject.transform.position);

                if (distance <= unitData.attackRange)
                {
                    isTargetInRange = true;
                    //isEnemyInRangeDelay_Cur = 0;
                }
                else if (isTargetInRange)
                {
                    Attack();
                }
            }
            else
            {
                isTargetInRange = false;
            }

            if (targetObject != null && !haveToMovePosition)
            {
                if (isTargetInSight)//시야 범위 내에 있는가?
                {
                    if (isTargetInRange)//공격 범위 내에 있는가?
                    {
                        targetPosition.position = this.transform.position;
                        Ally_State.fsm.ChangeState(UnitState.Attack);
                    }
                }
                else
                {
                    Ally_State.fsm.ChangeState(UnitState.Idle);
                }
            }
        }
    }

    private void ChangeMode()
    {
        // 모드 변경시 실행되는 매서드

        if (unitStateChangeTime > 0) // 모드 변경 시간(3초)
        {
            unitStateChangeTime -= Time.deltaTime;
            if (Ally_State != null && Ally_State.fsm != null)
            {
                Ally_State.fsm.ChangeState(UnitState.Idle); // 움직임을 멈추게 함
            }
        }
        else
        {
            //유닛의 모드 변경.
            if (previousAllyMode == AllyMode.Free)
            {
                allyMode = AllyMode.Siege;

                IEnumerator ModeChangeDelayCoroutine()
                {
                    navMeshAgent.enabled = false;
                    yield return new WaitForSeconds(0.5f);
                }
                StartCoroutine(ModeChangeDelayCoroutine());

                navMeshAgent.enabled = true;

                SearchEnmey();
            }
            else if (previousAllyMode == AllyMode.Siege)
            {
                //NavAgent.updatePosition = false;

                IEnumerator ModeChangeDelayCoroutine()
                {
                    navMeshAgent.enabled = false;
                    yield return new WaitForSeconds(0.5f);
                    navMeshAgent.enabled = true;
                }
                StartCoroutine(ModeChangeDelayCoroutine());

                allyMode = AllyMode.Free;
            }
            unitStateChangeTime = 3;
        }
    }

}

public class EnemyUnit : UnitCtrl_LoPol
{
    // 적군 유닛의 클래스,  UnitCtrl_LoPol을 상속받아 사용

    public EnemyUnitState enemyState; // 적군 유닛의 상태

    private bool isPathBlocked; // 길이 막혀있는지 확인
    private bool isBaseInRange; // 성이 공격 범위 내에 있는지 확인

    private int gold;     // 적 유닛 처치시 획득하는 골드

    private  void SearchPathToBase()
    {  // 적 유닛이 성으로 가는 길을 찾는 매서드

        /* 
         * 1. 성으로 가는 길을 찾음
         * 2. 성까지 가는 길이 안 막혀 있으면 성까지 이동
         * 3. 성까지 가는 길이 막혀 있으면 길을 막은 아군 유닛을 공격 -> 길을 막은 유닛 제거후 1, 2,3 반복
         */

        if (isPathBlocked)//길이 막혀있는가?
        {
            if (isPathBlocked != null && !isBaseInRange)//길이 막혀있고 성이 범위 내에 없을 시
            {
                isTargetInRange =
                    (Vector3.Distance(transform.position, targetObject.transform.position) <= unitData.attackRange);

                if (isTargetInSight)
                {
                    
                    if (isTargetInRange)
                    { // 적군 공격
                        Attack();
                        enemyState.fsm.ChangeState(EnemyState.Attack);
                    }
                    else
                    {
                        // 적에게 이동
                        Move(targetPosition);
                        enemyState.fsm.ChangeState(EnemyState.Move);
                    }
                }
            }
        }
        else // 성 공격
        {
            if (isBaseInRange)
            {
                Attack();
                enemyState.fsm.ChangeState(EnemyState.Attack);
            }
            else
            {
                Move(targetPosition);
                enemyState.fsm.ChangeState(EnemyState.Move);
            }

        }

        if (!isTargetInRange && !isBaseInRange)
        {
            enemyState.fsm.ChangeState(EnemyState.Move);
        }
    }
}
