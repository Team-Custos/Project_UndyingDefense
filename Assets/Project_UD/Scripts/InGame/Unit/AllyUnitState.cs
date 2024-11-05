using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MonsterLove.StateMachine;

//이 스크립트는 아군 병사들의 행동패턴을 관리하기 위한 함수입니다. (FSM 시스템 외부 스크립트 사용.)

public enum UnitState //유닛의 상태
{
    Idle,
    Attack,
    Search,
    Chase,
    Move
}

public class AllyUnitState : MonoBehaviour
{
    [HideInInspector]public UnitState State;
    public StateMachine<UnitState, StateDriverUnity> fsm;
    
    Ingame_UnitCtrl UnitCtrl; //이 병사
    Ingame_UIManager UnitUIManager;//이 병사의 UI
    NavMeshAgent navAgent;//병사의 길찾기 및 이동을 위한 NavMeshAgent.
    Animator allyAnimator;//병사 모델의 애니메이션을 관리하기 위한 애니메이터.


    private void Start()
    {
        fsm = new StateMachine<UnitState, StateDriverUnity>(this);
        fsm.ChangeState(UnitState.Idle);

        UnitCtrl = this.GetComponent<Ingame_UnitCtrl>();
        navAgent = this.GetComponent<NavMeshAgent>();
        
    }

    private void Update()
    {
        allyAnimator = UnitCtrl.GetComponent<UnitAnimationParaCtrl>().animator;
        fsm.Driver.Update.Invoke();
    }

    #region Idle State (병사가 아무것도 안할 때)
    void Idle_Enter()
    {
        Debug.Log("Idle Enter");
    }

    void Idle_Update()
    {
        allyAnimator.SetBool(CONSTANT.ANIBOOL_RUN, false);

        if (!UnitCtrl.isEnemyInSight || UnitCtrl.targetEnemy == null) // 프리모드에서는 회전이 안되게 해야하나??
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, UnitCtrl.defaultTargetRotation, Time.deltaTime * 2f);
            UnitCtrl.VisualModel.transform.rotation = Quaternion.Slerp(UnitCtrl.VisualModel.transform.rotation, UnitCtrl.defaultTargetRotation, Time.deltaTime * 2f);
        }
    }

    void Idle_Exit()
    {
        //Debug.Log("Idle Exit");
    }
    #endregion

    #region Attack State (병사가 공격할 때)
    void Attack_Enter()
    {
        Debug.Log("Attack_Enter");
    }

    void Attack_Update()
    {
        if (UnitCtrl.sightRangeSensor.Obj_Nearest != null) //유닛의 시야 범위에 적군이 있는가?
        {
            if (UnitCtrl.targetEnemy != null)//타겟 적군이 있는가?
            {
                if (UnitCtrl.haveToMovePosition)//적군을 무시하고 이동해야 하는가?
                {
                    //유닛의 적군과 관련된 모든 변수를 초기화.
                    UnitCtrl.targetEnemy = null;
                    UnitCtrl.isEnemyInRange = false;
                    UnitCtrl.isEnemyInSight = false;

                    //이동 상태로 변경.
                    fsm.ChangeState(UnitState.Move);

                    //변수 초기화.
                    UnitCtrl.haveToMovePosition = false;
                }
                else //타겟 적군이 있다면
                {
                    //타겟 적군 공격.
                    UnitCtrl.Unit_Attack();
                }
            }
        }
        else //시야범위에 적군이 없거나 있었던 적군이 없어질 경우
        {
            UnitCtrl.sightRangeSensor.ListTargetDelete(UnitCtrl.targetEnemy);//시야 범위 센서의 리스트를 새로 고침.
            fsm.ChangeState(UnitState.Search);//탐색 상태로 변경.
        }
    }

    void Attack_Exit()
    {
        Debug.Log("Attack_Exit");

        //UnitCtrl.targetEnemy = null;
        //UnitCtrl.isEnemyInRange = false;
    }
    #endregion

    #region Move State (병사가 이동할 때)
    void Move_Enter()
    {
        //Debug.Log("Move_Enter");
        
        //이동하기 위해 변수를 초기화.
        navAgent.isStopped = false;
        UnitCtrl.isEnemyInRange = false;
        UnitCtrl.isEnemyInSight = false;

        Ingame_UIManager.instance.ShowUnitStateUI(this.gameObject, true, false);
    }

    void Move_Update()
    {
        if (UnitCtrl.Ally_Mode == AllyMode.Free)//프리 모드일 때
        {
            navAgent.speed = UnitCtrl.cur_moveSpeed;//현재 설정된 속도로 이동.

            navAgent.SetDestination(UnitCtrl.moveTargetPos); //병사의 이동 목적지 설정
            navAgent.stoppingDistance = 0;

            float targetMoveDistance_Cur = Vector3.Distance(transform.position, UnitCtrl.moveTargetPos);//현재 목적지와 현재 병사의 위치간 거리.

            if (targetMoveDistance_Cur <= 0.1f)//목적지에 도착했을 때
            {
                UnitCtrl.moveTargetPos = transform.position;
                fsm.ChangeState(UnitState.Search);//탐색 상태로 변경.
                return;
            }
        }
    }

    void Move_Exit()
    {
        navAgent.SetDestination(transform.position); //NavmeshAgent를 정지시키기 위한 목적지 설정.
        navAgent.isStopped = true; //멈춤 상태로 변경.
        Ingame_UIManager.instance.ShowUnitStateUI(this.gameObject, false, false); 
    }
    #endregion

    void Chase_Enter()
    {
        Debug.Log("Chase_Enter");
        navAgent.isStopped = false;
    }

    void Chase_Update()
    {
        //Debug.Log("Chase_Update");

        float targetEnemyDistance_Cur = Vector3.Distance(transform.position, UnitCtrl.targetEnemy.transform.position); //타겟 적군과의 거리.

        if (targetEnemyDistance_Cur <= UnitCtrl.unitData.attackRange) //공격 범위 안으로 들어왔을 경우.
        {
            UnitCtrl.isEnemyInRange = true;
            navAgent.SetDestination(UnitCtrl.transform.position); //정지 시키기 위한 목적지 설정.
        }
        else //타겟 적군이 공격 범위 밖에 있을 경우.
        {
            navAgent.SetDestination(UnitCtrl.targetEnemy.transform.position); //적군의 위치로 목적지 설정.
        }

        if (UnitCtrl.targetEnemy != null && UnitCtrl.Ally_Mode == AllyMode.Free)//?? 왜 썼더라... 아마도 삭제할 예정.
        {
            
        }
    }

    void Chase_Exit()
    {
        Debug.Log("Chase_Exit");
        if (UnitCtrl.Ally_Mode == AllyMode.Free)
        {
            navAgent.SetDestination(transform.position); //정지 시키기 위한 목적지 설정.
        }
    }

    void Search_Enter()
    {
        Debug.Log("Search_Enter");
        UnitCtrl.haveToMovePosition = false;
        
    }

    void Search_Update()
    {
        if (!UnitCtrl.isEnemyInSight || UnitCtrl.targetEnemy == null) // 프리모드에서는 회전이 안되게 해야하나??
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, UnitCtrl.defaultTargetRotation, Time.deltaTime * 2f);
            UnitCtrl.VisualModel.transform.rotation = Quaternion.Slerp(UnitCtrl.VisualModel.transform.rotation, UnitCtrl.defaultTargetRotation, Time.deltaTime * 2f);
        }

        UnitCtrl.SearchEnemy();//적군 탐색.
    }

    


}
