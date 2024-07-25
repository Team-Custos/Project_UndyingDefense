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
    Move
}


public class UD_Ingame_UnitState : MonoBehaviour
{
    public StateMachine<UnitState, StateDriverUnity> fsm;

    UD_Ingame_UnitCtrl UnitCtrl;
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
        Debug.Log("Idle Enter");
    }

    void Idle_Exit()
    {
        Debug.Log("Idle Exit");
    }
    #endregion

    #region Attack State
    void Attack_Enter()
    {
        Debug.Log("Attack_Enter");
    }

    void Attack_Update()
    {
        UnitCtrl.Unit_Attack();
    }

    void Attack_Exit()
    {
        UnitCtrl.targetEnemy = null;
        UnitCtrl.isEnemyInRange = false;
    }
    #endregion

    #region Move State
    void Move_Enter()
    {
        Debug.Log("Move_Enter");
        //UnitCtrl.isEnemyInRange = false;
        //UnitCtrl.isEnemyInSight = false;
    }

    void Move_Update()
    {
        

        if (UnitCtrl.targetEnemy != null)
        {
            UnitCtrl.moveTargetPos = UnitCtrl.targetEnemy.transform.position;
            //navAgent.stoppingDistance = UnitCtrl.enemyAttackDistance;

            float targetEnemyDistance_Cur = Vector3.Distance(transform.position, UnitCtrl.targetEnemy.transform.position);

            if (targetEnemyDistance_Cur <= UnitCtrl.enemyAttackDistance)
            {
                UnitCtrl.isEnemyInRange = true;
                navAgent.ResetPath();
            }
            else
            {
                if (UnitCtrl.isEnemyInRange == false)
                {
                    navAgent.SetDestination(UnitCtrl.moveTargetPos);
                }
                
            }
        }
        else
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
        
    }
    #endregion


    void Search_Enter()
    {
        Debug.Log("Search_Enter");
        UnitCtrl.SearchEnemy();
        
    }

    



}
