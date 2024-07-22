using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEditor;

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

    private void Start()
    {
        fsm = new StateMachine<UnitState, StateDriverUnity>(this);
        fsm.ChangeState(UnitState.Idle);

        UnitCtrl = this.GetComponent<UD_Ingame_UnitCtrl>();
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
        UnitCtrl.isEnemyInRange = false;
    }

    void Move_Update()
    {
        transform.LookAt(UnitCtrl.moveTargetPos);
        transform.Translate(Vector3.forward * UnitCtrl.testSpeed * Time.deltaTime, Space.Self);

        if (UnitCtrl.targetEnemy != null)
        {
            float targetEnemyDistance_Cur = Vector3.Distance(transform.position, UnitCtrl.targetEnemy.transform.position);
            UnitCtrl.moveTargetPos = UnitCtrl.targetEnemy.transform.position;

            if (targetEnemyDistance_Cur <= UnitCtrl.enemyAttackDistance)
            {
                UnitCtrl.moveTargetPos = transform.position;
                UnitCtrl.isEnemyInRange = true;
                return;
            }
        }
        else
        {
            float targetMoveDistance_Cur = Vector3.Distance(transform.position, UnitCtrl.moveTargetPos);

            if (targetMoveDistance_Cur <= 0.1f)
            {
                UnitCtrl.moveTargetPos = transform.position;
                fsm.ChangeState(UnitState.Idle);
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
