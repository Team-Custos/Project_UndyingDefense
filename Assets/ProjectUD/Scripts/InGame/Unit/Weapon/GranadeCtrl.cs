using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeCtrl : ProjectileCtrl
{
    [SerializeField] private GameObject AttackTrigger;
    [SerializeField] private AudioClip bombAudio;
    private LayerMask attackTargetLayer;
    private AttackSkillData attackSkillData;

    private Vector3 targetPos;
    private float duration;
    private float durationCheck = 0f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float horizontalSpeed = 8f;


    [SerializeField] private float radius = 0f; // 원형 공격 범위
    //private Vector3 areaSize = Vector3.zero; // 사각형 공격 범위

    //private bool reachedTarget = false;
    //public void SetRadius(float radius)
    //{
    //    this.radius = radius;
    //}

    //public void SetArea(Vector3 areaSize)
    //{
    //    this.areaSize = areaSize;
    //}

    private void Update()
    {
        if (durationCheck >= duration)
        {
            durationCheck = 0f;

            SkillAttackTrigger attackTrigger =
                Instantiate(AttackTrigger).GetComponent<SkillAttackTrigger>();
            attackTrigger.transform.position = transform.position;
            attackTrigger.SetData(attackSkillData);
            attackTrigger.SetTargetLayer(attackTargetLayer);
            attackTrigger.AreaAttack(transform, radius);
            SoundManager.Instance.PlaySFX(bombAudio, transform.position);
            Destroy(gameObject);
        }
        else
        {
            durationCheck += Time.deltaTime;
        }
    }

    public void SetTargetLayer(LayerMask targetLayer)
    {
        attackTargetLayer = targetLayer;
    }

    public void SetTargetPos(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }

    public void JumpTowards()
    {
        Vector3 startPos = rb.position;

        Vector3 displacementXZ = new Vector3(
            targetPos.x - startPos.x,
            0f,
            targetPos.z - startPos.z
        );

        float distanceXZ = displacementXZ.magnitude;

        // 거리가 멀수록 duration 증가
        duration = distanceXZ / horizontalSpeed;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float deltaY = targetPos.y - startPos.y;

        // duration 시간 후 정확히 targetPos.y에 도착하도록 Y 초기속도 계산
        float verticalSpeed =
            (deltaY + 0.5f * gravity * duration * duration)
            / duration;

        // 수평 속도
        Vector3 velocityXZ = displacementXZ / duration;

        rb.velocity = velocityXZ + Vector3.up * verticalSpeed;

        durationCheck = 0f;
    }


    // 특정 좌표로 점프하는 함수
    //public void JumpTowards(Vector3 targetPos, float duration = 1f)
    //{
    //    Vector3 startPos = transform.position;

    //    float g = Mathf.Abs(Physics.gravity.y);

    //    float timeToPeak = duration / 2f;
    //    float vy = g * timeToPeak;

    //    Vector3 displacementXZ = new Vector3(
    //        targetPos.x - startPos.x,
    //        0,
    //        targetPos.z - startPos.z
    //    );

    //    Vector3 velocityXZ = displacementXZ / duration;
    //    Vector3 velocity = new Vector3(velocityXZ.x, vy, velocityXZ.z);

    //    rb.velocity = velocity;
    //}

    public void SetData(AttackSkillData data)
    {
        attackSkillData = data;
    }




    //private void Update()
    //{
    //    if (timeCheck < time)
    //    {
    //        timeCheck += Time.deltaTime;
    //    }
    //    else
    //    {
    //        SkillAttackTrigger attackTrigger =
    //                Instantiate(AttackTrigger).GetComponent<SkillAttackTrigger>();
    //        attackTrigger.transform.position = transform.position;
    //        attackTrigger.SetData(attackSkillData);
    //        attackTrigger.SetTargetLayer(attackTargetLayer);
    //        attackTrigger.AreaAttack(transform, radius);
    //        SoundManager.Instance.PlaySFX(bombAudio, transform.position);
    //        Destroy(gameObject);

    //        //if (!reachedTarget)
    //        //{
    //        //    //reachedTarget = true;
    //        //    SkillAttackTrigger attackTrigger =
    //        //        Instantiate(AttackTrigger).GetComponent<SkillAttackTrigger>();
    //        //    attackTrigger.transform.position = transform.position;
    //        //    attackTrigger.SetData(attackSkillData);
    //        //    attackTrigger.SetTargetLayer(attackTargetLayer);
    //        //    attackTrigger.AreaAttack(transform, radius);
    //        //    SoundManager.Instance.PlaySFX(bombAudio, transform.position);
    //        //    Destroy(gameObject);
    //        //}
    //    }
    //}

}
