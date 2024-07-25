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

    UD_Ingame_UnitCtrl EnemyCtrl;
    NavMeshAgent navAgent;

    private void Start()
    {
        EnemyCtrl = this.GetComponent<UD_Ingame_UnitCtrl>();
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
        EnemyCtrl.Unit_Attack();
    }

    void Attack_Exit()
    {
        EnemyCtrl.targetEnemy = null;
        EnemyCtrl.isEnemyInRange = false;
    }
    #endregion

    #region Move State
    void Move_Enter()
    {
        Debug.Log("Enemy Move_Enter");
        EnemyCtrl.isEnemyInRange = false;
    }

    void Move_Update()
    {
        navAgent.SetDestination(EnemyCtrl.moveTargetPos);
        EnemyCtrl.SearchEnemy();

        //transform.LookAt(EnemyCtrl.moveTargetPos);
        //transform.Translate(Vector3.forward * EnemyCtrl.testSpeed * Time.deltaTime, Space.Self);

        if (EnemyCtrl.targetEnemy != null)
        {
            float targetUnitDistance_Cur = Vector3.Distance(transform.position, EnemyCtrl.targetEnemy.transform.position);
            EnemyCtrl.moveTargetPos = EnemyCtrl.targetEnemy.transform.position;

            if (targetUnitDistance_Cur <= EnemyCtrl.attackDistance)
            {
                //EnemyCtrl.moveTargetPos = transform.position;
                EnemyCtrl.isEnemyInRange = true;
                navAgent.SetDestination(EnemyCtrl.transform.position);
                return;
            }
        }
        else
        {
            //float targetMoveDistance_Cur = Vector3.Distance(transform.position, EnemyCtrl.moveTargetPos);

            if (EnemyCtrl.enemy_isBaseInRange)
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
            EnemyCtrl.SearchEnemy();
        }

       
    }
}
