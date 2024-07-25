using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_GameManager : MonoBehaviour
{
    public static UD_Ingame_GameManager inst;
    public UD_Ingame_GridManager gridManager;

    public GameObject[] Unit;
    public GameObject[] Enemy;

    public GameObject Base;
   

    public bool AllyUnitSetMode = false;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Unit = GameObject.FindGameObjectsWithTag(UD_CONSTANT.TAG_UNIT);
        Enemy = GameObject.FindGameObjectsWithTag(UD_CONSTANT.TAG_ENEMY);

        //정렬보다 찾는 식으로 코드 다시 짜야됨.
        //UnitSearch();
        //EnemySearch();
    }

    void UnitSearch()
    {
        for (int UnitIdx = 0; UnitIdx < Unit.Length; UnitIdx++)
        {
            UD_Ingame_UnitCtrl UnitCtrl = Unit[UnitIdx].GetComponent<UD_Ingame_UnitCtrl>();

            for (int EnemyIdx = 0; EnemyIdx < Enemy.Length; EnemyIdx++)
            {
                float Distance = Vector3.Distance(Unit[UnitIdx].transform.position, Enemy[EnemyIdx].transform.position);//유닛과 적 간의 거리

                UnitCtrl.isEnemyInSight = (Distance <= UnitCtrl.enemySightDistance); //유닛 시야 범위에 적이 있을 경우
                UnitCtrl.isEnemyInRange = (Distance <= UnitCtrl.enemyAttackDistance); // 유닛 공격 범위에 적이 있을 경우

                if (UnitCtrl.Unit_State.fsm.State == UnitState.Idle)
                {
                    if (UnitCtrl.isEnemyInSight || UnitCtrl.isEnemyInRange)
                    {
                        UnitCtrl.targetEnemy = Enemy[EnemyIdx];
                    }
                }
            }
        }
    }

    void EnemySearch()
    {
        for (int EnemyIdx = 0; EnemyIdx < Enemy.Length; EnemyIdx++)
        {
            UD_Ingame_EnemyCtrl EnemyCtrl = Enemy[EnemyIdx].GetComponent<UD_Ingame_EnemyCtrl>();

            for (int UnitIdx = 0; UnitIdx < Unit.Length; UnitIdx++)
            {
                float Distance = Vector3.Distance(Unit[UnitIdx].transform.position, Enemy[EnemyIdx].transform.position);//유닛과 적 간의 거리

                EnemyCtrl.isUnitInSight = (Distance <= EnemyCtrl.unitSightDistance);
                EnemyCtrl.isUnitInRange = (Distance <= EnemyCtrl.unitAttackDistance);

                if (EnemyCtrl.Enemy_State.fsm.State == EnemyState.Idle || EnemyCtrl.Enemy_State.fsm.State == EnemyState.Move)
                {
                    if (EnemyCtrl.isUnitInSight || EnemyCtrl.isUnitInRange)
                    {
                        EnemyCtrl.targetUnit = Unit[UnitIdx];
                    }
                    else
                    {
                        EnemyCtrl.targetUnit = null;
                    }
                }
            }

            if (EnemyCtrl.targetBase != null)
            {
                float Distance = Vector3.Distance(EnemyCtrl.transform.position,EnemyCtrl.targetBase.transform.position);

                EnemyCtrl.isBaseInRange = (Distance <= EnemyCtrl.unitAttackDistance);
            }
        }

        
    }
}
