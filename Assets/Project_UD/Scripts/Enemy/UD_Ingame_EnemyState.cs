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
        Debug.Log("Idle Enter");
        
    }
    #endregion

    #region Attack State
    void Attack_Enter()
    {
        Debug.Log("Attack_Enter");
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
        Debug.Log("Move_Enter");
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
                EnemyCtrl.moveTargetPos = transform.position;
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
                EnemyCtrl.moveTargetPos = EnemyCtrl.targetBase.transform.position;
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

    }
}
