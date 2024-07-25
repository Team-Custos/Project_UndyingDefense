using MonsterLove.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Attack,
    Search,
    Move
}


public class UD_Ingame_EnemyState : MonoBehaviour
{
    public StateMachine<EnemyState, StateDriverUnity> fsm;

    UD_Ingame_EnemyCtrl EnemyCtrl;
    NavMeshAgent navAgent;

    private void Start()
    {
        EnemyCtrl = this.GetComponent<UD_Ingame_EnemyCtrl>();
        navAgent = this.GetComponent<NavMeshAgent>();

        fsm = new StateMachine<EnemyState, StateDriverUnity>(this);
        fsm.ChangeState(EnemyState.Move);
    }

    private void Update()
    {
        fsm.Driver.Update.Invoke();
    }

    #region Idle State
    void Idle_Enter()
    {
        Debug.Log("Enemy Idle Enter");
        
    }
    #endregion

    #region Attack State
    void Attack_Enter()
    {
        Debug.Log("Enemy Attack_Enter");
    }

    void Attack_Update()
    {
        EnemyCtrl.Enemy_Attack();
    }

    void Attack_Exit()
    {
        EnemyCtrl.targetUnit = null;
        EnemyCtrl.isUnitInRange = false;
    }
    #endregion

    #region Move State
    void Move_Enter()
    {
        Debug.Log("Enemy Move_Enter");
        EnemyCtrl.isUnitInRange = false;
    }

    void Move_Update()
    {
        navAgent.SetDestination(EnemyCtrl.moveTargetPos);

        //transform.LookAt(EnemyCtrl.moveTargetPos);
        //transform.Translate(Vector3.forward * EnemyCtrl.testSpeed * Time.deltaTime, Space.Self);

        if (EnemyCtrl.targetUnit != null)
        {
            float targetUnitDistance_Cur = Vector3.Distance(transform.position, EnemyCtrl.targetUnit.transform.position);
            EnemyCtrl.moveTargetPos = EnemyCtrl.targetUnit.transform.position;

            if (targetUnitDistance_Cur <= EnemyCtrl.unitAttackDistance)
            {
                //EnemyCtrl.moveTargetPos = transform.position;
                EnemyCtrl.isUnitInRange = true;
                navAgent.SetDestination(transform.position);
                return;
            }
        }
        else
        {
            //float targetMoveDistance_Cur = Vector3.Distance(transform.position, EnemyCtrl.moveTargetPos);

            if (EnemyCtrl.isBaseInRange)
            {
                navAgent.SetDestination(transform.position);
                navAgent.isStopped = true;
                return;
            }
        }

    
    }

    void Move_Exit()
    {

    }
    #endregion


    void Search_Enter()
    {
        Debug.Log("Enemy Search_Enter");

        UD_Ingame_RangeCtrl Range = EnemyCtrl.GetComponentInChildren<UD_Ingame_RangeCtrl>();

        if (Range == null)
        {
            Debug.LogError("Range Error in : " + EnemyCtrl.gameObject.name);
        }
        else
        {
            //GameObject TargetObj = Range.ObjectSearch(EnemyCtrl.unitSightDistance,EnemyCtrl.unitAttackDistance, true);

            //if (TargetObj != null)
            //{
            //    EnemyCtrl.moveTargetPos = TargetObj.transform.position;
            //}
            //else
            //{
            //    EnemyCtrl.moveTargetPos = EnemyCtrl.targetBase.transform.position;
            //}
        }

       
    }
}
