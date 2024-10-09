using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MonsterLove.StateMachine;

public enum UnitState
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
    
    Ingame_UnitCtrl UnitCtrl;
    Ingame_UIManager UnitUIManager;
    NavMeshAgent navAgent;
    Animator allyAnimator;


    private void Start()
    {
        fsm = new StateMachine<UnitState, StateDriverUnity>(this);
        fsm.ChangeState(UnitState.Idle);

        UnitCtrl = this.GetComponent<Ingame_UnitCtrl>();
        navAgent = this.GetComponent<NavMeshAgent>();
        
    }

    private void Update()
    {
        allyAnimator = UnitCtrl.CurVisualModelAnimator;
        fsm.Driver.Update.Invoke();
    }

    #region Idle State
    void Idle_Enter()
    {
        Debug.Log("Idle Enter");
    }

    void Idle_Update()
    {
        allyAnimator.SetBool(CONSTANT.ANIBOOL_RUN, false);
        allyAnimator.SetBool(CONSTANT.ANIBOOL_ATTACK, false);
        UnitCtrl.SearchEnemy();
    }

    void Idle_Exit()
    {
        //Debug.Log("Idle Exit");
    }
    #endregion

    #region Attack State
    void Attack_Enter()
    {
        Debug.Log("Attack_Enter");
        allyAnimator.SetBool(CONSTANT.ANIBOOL_ATTACK, true);
    }

    void Attack_Update()
    {
        if (UnitCtrl.sightRangeSensor.Obj_Nearest != null)
        {
            if (UnitCtrl.targetEnemy != null)
            {
                if (UnitCtrl.haveToMovePosition)
                {
                    UnitCtrl.targetEnemy = null;
                    UnitCtrl.isEnemyInRange = false;
                    UnitCtrl.isEnemyInSight = false;
                    fsm.ChangeState(UnitState.Move);
                    UnitCtrl.haveToMovePosition = false;
                }
                else
                {
                    UnitCtrl.Unit_Attack();
                }
            }
        }
        else
        {
            UnitCtrl.sightRangeSensor.ListRefresh();
            fsm.ChangeState(UnitState.Search);
        }
    }

    void Attack_Exit()
    {
        Debug.Log("Attack_Exit");
        //UnitCtrl.targetEnemy = null;
        //UnitCtrl.isEnemyInRange = false;
    }
    #endregion

    #region Move State
    void Move_Enter()
    {
        //Debug.Log("Move_Enter");
        
        navAgent.isStopped = false;
        UnitCtrl.isEnemyInRange = false;
        UnitCtrl.isEnemyInSight = false;

        //Ingame_UIManager.instance.ShowMoveUI(this.gameObject, true);
        allyAnimator.SetBool(CONSTANT.ANIBOOL_RUN, true);
    }

    void Move_Update()
    {
        if (UnitCtrl.Ally_Mode == AllyMode.Free)
        {
            navAgent.SetDestination(UnitCtrl.moveTargetPos);
            navAgent.stoppingDistance = 0;

            float targetMoveDistance_Cur = Vector3.Distance(transform.position, UnitCtrl.moveTargetPos);

            if (targetMoveDistance_Cur <= 0.1f)
            {
                UnitCtrl.moveTargetPos = transform.position;
                fsm.ChangeState(UnitState.Search);
                return;
            }
        }

    }

    void Move_Exit()
    {
        navAgent.SetDestination(transform.position);
        navAgent.isStopped = true;
        Ingame_UIManager.instance.ShowMoveUI(this.gameObject, false);

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

        float targetEnemyDistance_Cur = Vector3.Distance(transform.position, UnitCtrl.targetEnemy.transform.position);

        if (targetEnemyDistance_Cur <= UnitCtrl.unitData.attackRange)
        {
            UnitCtrl.isEnemyInRange = true;
            navAgent.SetDestination(UnitCtrl.transform.position);
        }
        else
        {
            navAgent.SetDestination(UnitCtrl.targetEnemy.transform.position);
        }

        if (UnitCtrl.targetEnemy != null && UnitCtrl.Ally_Mode == AllyMode.Free)
        {
            
        }
    }

    void Chase_Exit()
    {
        Debug.Log("Chase_Exit");
        if (UnitCtrl.Ally_Mode == AllyMode.Free)
        {
            navAgent.SetDestination(transform.position);
        }
    }

    void Search_Enter()
    {
        Debug.Log("Search_Enter");
        UnitCtrl.haveToMovePosition = false;
        UnitCtrl.SearchEnemy();
    }

    



}
