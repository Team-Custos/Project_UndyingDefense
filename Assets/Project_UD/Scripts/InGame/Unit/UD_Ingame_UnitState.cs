using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MonsterLove.StateMachine;
using UnityEngine.UI;

public enum UnitState
{
    Idle,
    Attack,
    Search,
    Chase,
    Move
}



public class UD_Ingame_UnitState : MonoBehaviour
{
    [HideInInspector]public UnitState State;
    public StateMachine<UnitState, StateDriverUnity> fsm;
    
    UD_Ingame_UnitCtrl UnitCtrl;
    UD_Ingame_UIManager UnitUIManager;
    NavMeshAgent navAgent;


    private void Start()
    {
        fsm = new StateMachine<UnitState, StateDriverUnity>(this);
        fsm.ChangeState(UnitState.Idle);

        UnitCtrl = this.GetComponent<UD_Ingame_UnitCtrl>();
        navAgent = this.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        fsm.Driver.Update.Invoke();
    }

    #region Idle State
    void Idle_Enter()
    {
        //Debug.Log("Idle Enter");
    }

    void Idle_Update()
    {
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
        UnitCtrl.targetEnemy = null;
        UnitCtrl.isEnemyInRange = false;
    }
    #endregion

    #region Move State
    void Move_Enter()
    {
       // Debug.Log("Move_Enter");
        navAgent.isStopped = false;
        //UnitCtrl.isEnemyInRange = false;
        //UnitCtrl.isEnemyInSight = false;
        UD_Ingame_UIManager.instance.ShowMoveUI(this.gameObject, true);
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
        UD_Ingame_UIManager.instance.ShowMoveUI(this.gameObject, false);
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

        if (UnitCtrl.targetEnemy != null)
        {
            float targetEnemyDistance_Cur = Vector3.Distance(transform.position, UnitCtrl.targetEnemy.transform.position);

            navAgent.SetDestination(UnitCtrl.targetEnemy.transform.position);

            if (targetEnemyDistance_Cur <= UnitCtrl.attackRange)
            {
                UnitCtrl.isEnemyInRange = true;
                navAgent.SetDestination(UnitCtrl.transform.position);
            }
        }
    }

    void Chase_Exit()
    {
        Debug.Log("Chase_Exit");
        if (UnitCtrl.Ally_Mode == AllyMode.Free)
        {
            navAgent.SetDestination(transform.position);
            navAgent.isStopped = true;
        }
    }

    void Search_Enter()
    {
        Debug.Log("Search_Enter");
        UnitCtrl.haveToMovePosition = false;
        UnitCtrl.SearchEnemy();
    }

    



}
