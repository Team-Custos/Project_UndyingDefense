using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    Seige,
    Free
}

//TODO : 유닛 상태 변수 추가

public class UD_Ingame_UnitCtrl : MonoBehaviour
{
    [HideInInspector] public UD_Ingame_UnitState Unit_State;
    MeshRenderer MeshRenderer;

    [Header("====Data====")]
    public int modelType;
    public int HP;
    public float speed;
    public UnitType unitType;

    [Header("====Status====")]
    public Mode Unit_Mode;

    public Vector2 UnitPos = Vector2.zero;
    public Color32 colorAlly = Color.blue;
    public GameObject Selected_Particle;
    public bool isSelected = false;
    public float weaponCooldown = 0;

    public float skillCooldown = 0;


    [Header("====AI====")]
    public Vector3 moveTargetPos = Vector3.zero; 
    public GameObject targetEnemy = null;
    public UD_Ingame_RangeCtrl sightRangeSensor;
    public float enemySightDistance = 0;
    public float enemyAttackDistance = 0;
    public bool isEnemyInSight = false;
    public bool isEnemyInRange = false;

    

    public GameObject findEnemyRange = null;
    public GameObject Bow = null;


    public float testSpeed = 1;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        Unit_State = GetComponent<UD_Ingame_UnitState>();
        moveTargetPos = transform.position;
    }

    

    // Update is called once per frame
    void Update()
    {
        MeshRenderer.material.color = colorAlly;
        Selected_Particle.SetActive(isSelected);
        //findEnemyRange.SetActive(isSelected);

        sightRangeSensor.radius = enemySightDistance;
        findEnemyRange.transform.localScale = new Vector3(enemyAttackDistance + 4, enemyAttackDistance + 4, 0);

        //if (Unit_State.fsm.State == UnitState.Idle)
        //{
        //    SearchEnemy();
        //}

        if (targetEnemy != null)
        {
            if (isEnemyInSight)
            {
                isEnemyInRange = (Vector3.Distance(transform.position, targetEnemy.transform.position) <= enemyAttackDistance);
                if (isEnemyInRange)
                {
                    Unit_State.fsm.ChangeState(UnitState.Attack);
                }
                else
                {
                    Unit_State.fsm.ChangeState(UnitState.Move);
                }
            }
        }

        if (Vector3.Distance(transform.position, moveTargetPos) > 0.1f)
        {
            Unit_State.fsm.ChangeState(UnitState.Move);
        }
        else
        {
            Unit_State.fsm.ChangeState(UnitState.Idle);
        }
    }

    public void SearchEnemy()
    {
        if (sightRangeSensor == null)
        {
            Debug.LogError("Range Error in : " + this.gameObject.name);
            return;
        }
        else
        {
            GameObject TargetObj = 
                sightRangeSensor.NearestObjectSearch(enemyAttackDistance, false);

            if (TargetObj != null)
            {
                isEnemyInSight = true;
                moveTargetPos = TargetObj.transform.position;
                targetEnemy = TargetObj;
            }
            else
            {
                moveTargetPos = transform.position;
                targetEnemy = null;
            }
        }
    }

    public void Unit_Attack()
    {
        if (targetEnemy == null)
        {
            Unit_State.fsm.ChangeState(UnitState.Search);
            return;
        }
        else
        {
            transform.LookAt(targetEnemy.transform.position);
            Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(weaponCooldown);
        }

        
    }

    public void Init(UnitSpawnData data)
    {
        modelType = data.modelType;
        HP = data.HP;
        speed = data.speed;
        unitType = data.unitType;

    }
}
