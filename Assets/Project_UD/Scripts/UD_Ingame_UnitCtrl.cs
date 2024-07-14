using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IState
{
    public void EnterState();

    public void UpdateState();
    public void ExitState();

}

//TODO : 유닛 상태 변수 추가

public class UD_Ingame_UnitCtrl : MonoBehaviour
{
    MeshRenderer MeshRenderer;

    [Header("====Status====")]
    public Vector2 UnitPos = Vector2.zero;
    public UnitType UnitType;
    public Color32 colorAlly = Color.blue;
    public GameObject Selected_Particle;
    public bool isSelected = false;
    

    [Header("====AI====")]
    public Vector3 moveTargetPos = Vector3.zero;
    public GameObject targetEnemy = null;
    public float targetEnemyDistance = 0;


    public float testSpeed = 1;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MeshRenderer.material.color = colorAlly;
        Selected_Particle.SetActive(isSelected);
        Unit_Move();
    }


    void Unit_Move()
    {
        if (transform.position != moveTargetPos)
        {
            transform.LookAt(moveTargetPos);
            transform.Translate(Vector3.forward * testSpeed * Time.deltaTime, Space.Self);

            if (targetEnemy != null)
            {
                float targetEnemyDistance_Cur = Vector3.Distance(transform.position, targetEnemy.transform.position);

                if (targetEnemyDistance_Cur <= targetEnemyDistance)
                {
                    moveTargetPos = transform.position;
                    return;
                }
            }

        }
    }

    void Init(UnitSpawnData data)
    {
        
    }
}
