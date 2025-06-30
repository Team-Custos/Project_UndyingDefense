using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeCtrl : ProjectileCtrl
{
    [SerializeField] private GameObject AttackTrigger;
    private LayerMask attackTargetLayer;
    private AttackSkillData attackSkillData;
    private float time = 1f;

    [SerializeField] private float radius = 0f; // 원형 공격 범위
    private Vector3 areaSize = Vector3.zero; // 사각형 공격 범위

    private bool reachedTarget = false;
    public void SetRadius(float radius)
    {
        this.radius = radius;
    }

    public void SetArea(Vector3 areaSize)
    {
        this.areaSize = areaSize;
    }

    public void SetTargetLayer(LayerMask targetLayer)
    {
        attackTargetLayer = targetLayer;
    }

    // 특정 좌표로 점프하는 함수
    public void JumpTowards(Vector3 targetPos, float duration = 1f)
    {
        Vector3 startPos = transform.position;
        
        float g = Mathf.Abs(Physics.gravity.y);

        float timeToPeak = duration / 2f;
        float vy = g * timeToPeak;

        Vector3 displacementXZ = new Vector3(
            targetPos.x - startPos.x,
            0,
            targetPos.z - startPos.z
        );

        Vector3 velocityXZ = displacementXZ / duration;
        Vector3 velocity = new Vector3(velocityXZ.x, vy, velocityXZ.z);

        rb.velocity = velocity;
    }

    public void SetData(AttackSkillData data)
    {
        attackSkillData = data;
    }


    private float timeCheck = 0f;
    private void Update()
    {
        if (timeCheck < time)
        {
            timeCheck += Time.deltaTime;
        }
        else
        {
            if (!reachedTarget)
            {
                reachedTarget = true;
                SkillAttackTrigger attackTrigger = 
                    Instantiate(AttackTrigger).GetComponent<SkillAttackTrigger>();
                attackTrigger.transform.position = transform.position;
                attackTrigger.SetData(attackSkillData);
                attackTrigger.SetTargetLayer(attackTargetLayer);
                attackTrigger.AreaAttack(transform, radius);
                Destroy(gameObject);
            }
        }
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == CONSTANT.TAG_TILE)
    //    {
    //        Debug.Log("Granade Hit Ground");
    //        Destroy(gameObject);
    //        Instantiate(AttackTrigger, transform.position, Quaternion.identity);
    //    }

    //}
}
