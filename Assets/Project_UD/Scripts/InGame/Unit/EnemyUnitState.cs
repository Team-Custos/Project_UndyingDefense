using MonsterLove.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

//이 스크립트는 적군 병사들의 행동패턴을 관리하기 위한 함수입니다. (FSM 시스템 외부 스크립트 사용.)

public enum EnemyState//적군 유닛의 상태.
{
    Idle,
    Attack,
    Move,
    Dead
}


public class EnemyUnitState : MonoBehaviour
{
    public StateMachine<EnemyState, StateDriverUnity> fsm;

    Ingame_UnitCtrl UnitCtrl;//이 병사.
    NavMeshAgent navAgent;//병사의 길찾기 및 이동을 위한 NavMeshAgent.
    NavMeshObstacle navObstacle;

    Animator EnemyAnimator;//병사 모델의 애니메이션을 관리하기 위한 애니메이터.

    Vector3 previousNavDestination;//병사의 길찾기를 위한 가장 최근의 목적지 좌표.

    public List<Transform> destinations;

    private void Start()
    {
        fsm = new StateMachine<EnemyState, StateDriverUnity>(this);

        UnitCtrl = this.GetComponent<Ingame_UnitCtrl>();
        navAgent = this.GetComponent<NavMeshAgent>();
        navObstacle = this.GetComponent<NavMeshObstacle>();

        previousNavDestination = this.transform.position;
    }

    private void Update()
    {
        EnemyAnimator = UnitCtrl.GetComponent<UnitAnimationParaCtrl>().animator;
        fsm.Driver.Update.Invoke();
    }

    #region Idle State (병사가 아무것도 안할 때)
    void Idle_Enter()
    {
        Debug.Log("Enemy Idle Enter");
        
    }

    void Idle_Update()
    {
        EnemyAnimator.SetBool(CONSTANT.ANIBOOL_RUN, false);
        //EnemyAnimator.SetBool(CONSTANT.ANITRIGGER_ATTACK, false);
    }

    #endregion

    #region Attack State
    void Attack_Enter()
    {
        //Debug.Log("Enemy Attack_Enter");
    }

    void Attack_Update()
    {
        navAgent.enabled = false;
        navObstacle.enabled = true;

        //if (UnitCtrl.enemy_isPathBlocked)
        //{
        //    EnemyAnimator.SetBool(CONSTANT.ANIBOOL_RUN, false);
        //    //EnemyAnimator.SetBool(CONSTANT.ANITRIGGER_ATTACK, true);
        //    UnitCtrl.Unit_Attack(); //아군 병사 공격.
        //}
        //else
        //{
        //    fsm.ChangeState(EnemyState.Move);
        //}

        EnemyAnimator.SetBool(CONSTANT.ANIBOOL_RUN, false);
        //EnemyAnimator.SetBool(CONSTANT.ANITRIGGER_ATTACK, true);
        UnitCtrl.Unit_Attack(); //아군 병사 공격.

        SearchPath();
    }

    void Attack_Exit()
    {
        //변수 초기화.
        UnitCtrl.targetEnemy = null;
        UnitCtrl.isEnemyInRange = false;
        //EnemyAnimator.SetBool(CONSTANT.ANITRIGGER_ATTACK, false);
    }
    #endregion

    #region Move State
    void Move_Enter()
    {
        //변수 초기화.
        UnitCtrl.isEnemyInRange = false;

        UnitCtrl.moveTargetPos = UnitCtrl.moveTargetBasePos;
        UnitCtrl.enemy_isPathBlocked = false;
        
        SearchPath();

    }

    void Move_Update()
    {

        navObstacle.enabled = false;
        navAgent.enabled = true;

        EnemyAnimator.SetBool(CONSTANT.ANIBOOL_RUN, true);
        //EnemyAnimator.SetBool(CONSTANT.ANITRIGGER_ATTACK, false);
        navAgent.speed = UnitCtrl.cur_moveSpeed;//현재 설정된 속도로 이동.
        UnitCtrl.moveTargetPos = UnitCtrl.moveTargetBasePos;

        if (navAgent.enabled == true)
        {
            SearchPath();//길 찾기.
        }

        if (UnitCtrl.enemy_isBaseInRange)//공격 범위에 성이 있을 경우
        {
            fsm.ChangeState(EnemyState.Attack); //공격 상태로 변경.
            return; //후에 빠져나옴.
        }
        

        navAgent.SetDestination(UnitCtrl.moveTargetPos);//이동 목적지 설정.

        if (UnitCtrl.enemy_isPathBlocked)//성까지의 길이 막혀있는가?
        {
            UnitCtrl.SearchEnemy();//아군 병사를 탐색.

            if (UnitCtrl.targetEnemy != null)//타겟 아군이 있는가?
            {
                if (previousNavDestination.z < UnitCtrl.targetEnemy.transform.position.z)//목표 병사보다 현재 타겟 위치가 더 멈.
                {
                    //Debug.Log("목표 병사보다 현재 타겟 위치가 더 멈.");
                    UnitCtrl.enemy_isPathBlocked = false; //성까지의 길이 막혀있지 않음.
                    return;
                }

                float targetUnitDistance_Cur = Vector3.Distance(transform.position, UnitCtrl.targetEnemy.transform.position);//현재 아군 병사와 현재 적군의 위치 간 거리.
                UnitCtrl.moveTargetPos = UnitCtrl.targetEnemy.transform.position;//적군의 이동 목적 좌표를 아군 병사의 위치로 설정.

                if (targetUnitDistance_Cur <= UnitCtrl.unitData.attackRange)//목적지에 도착했을 때
                {
                    UnitCtrl.isEnemyInRange = true;
                    navAgent.SetDestination(UnitCtrl.transform.position);//정지.
                    return;
                }
            }
            else
            {
                SearchPath();
            }
        }
    }

    void Move_Exit()
    {
        navAgent.SetDestination(transform.position);//정지.
        EnemyAnimator.SetBool(CONSTANT.ANIBOOL_RUN, false);        
        //navAgent.isStopped = true;
    }

    void Dead_Enter()
    {

    }

    void Dead_Update()
    {

    }

    public void SearchPath()//길찾기
    {
        if (navAgent == false)
        {
            return;
        }

        NavMeshPath calcaulatedPath = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, UnitCtrl.moveTargetBasePos, NavMesh.AllAreas, calcaulatedPath))
        {
            if (calcaulatedPath.status != NavMeshPathStatus.PathComplete)
            {
                Debug.Log("Can not Find Path");
                UnitCtrl.enemy_isPathBlocked = true;
            }
            else
            {
                Debug.Log("Find Path Success");
                UnitCtrl.enemy_isPathBlocked = false;
            }
        }
        else
        {
            Debug.Log("Can not Find Path");
            UnitCtrl.enemy_isPathBlocked = true;
        }

        //StartCoroutine(DestinationValidCheck());
        //IEnumerator DestinationValidCheck()//현재 길이 막혀있는지 판단.
        //{
        //    yield return new WaitUntil(() => { return !navAgent.pathPending; });

        //    int cornerCount = navAgent.path.corners.Length;
        //    //Debug.Log("명령 받은 목표 지점 위치 : " + UnitCtrl.moveTargetBasePos);

        //    Vector3 navDestination = new Vector3(navAgent.path.corners[cornerCount - 1].x, 0, navAgent.path.corners[cornerCount - 1].z);

        //    //Debug.Log("계산 된 마지막 목표 지점 위치 : " + navDestination);
        //    //Debug.Log("거리 : " + Mathf.Abs(navDestination.x - UnitCtrl.moveTargetBasePos.x));

        //    if (Mathf.Abs(navDestination.x - UnitCtrl.moveTargetBasePos.x) <= UnitCtrl.unitData.attackRange)
        //    {
        //        //Debug.Log("목표 지점으로 이동할 수 있습니다.");
        //        UnitCtrl.enemy_isPathBlocked = false;
        //    }
        //    else
        //    {
        //        //Debug.Log("목표 지점으로 이동할수는 없지만 중간 지점까진 갈 수 있습니다.");
        //        UnitCtrl.enemy_isPathBlocked = true;
        //    }
        //    previousNavDestination = navDestination;
        //}
    }

    #endregion



}
