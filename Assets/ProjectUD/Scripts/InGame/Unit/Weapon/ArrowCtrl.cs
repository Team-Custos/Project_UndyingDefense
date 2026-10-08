using UnityEngine;
using UnityEngine.Events;

//이 스크립트는 화살 오브젝트를 관리하기 위한 스크립트입니다.
public class ArrowCtrl : ProjectileCtrl
{
    private UnityEvent onAttack = new UnityEvent();

    public void SetEvent(UnityAction onAttack)
    {
        this.onAttack.AddListener(onAttack);
    }

    //public void CalculateTime(float distance)
    //{
    //    time = distance / speed;
    //}

    private void FixedUpdate()
    {
        Vector3 targetPos;      // 목표 대상 위치 유닛 or 성

        if (targetUnit != null)
            targetPos = targetUnit.transform.position;
        else if (fortress != null)
            targetPos = fortress.transform.position;
        else
        {
            Debug.Log("목표 없음");
            return;
        }

        Vector3 pos = transform.position;

        targetPos.y = pos.y;

        //transform.position = Vector3.MoveTowards(pos, targetPos, speed * Time.fixedDeltaTime);

        float distance = Vector3.Distance(pos, targetPos);
        float distancePerFrame = speed * Time.fixedDeltaTime;
        if (distance > hitDistance + distancePerFrame)
        {
            Vector3 direction = (targetPos - pos).normalized;

            rb.velocity = direction * speed;
            rb.MoveRotation(Quaternion.LookRotation(direction));
        }
        else
        {
            onAttack.Invoke();
            Destroy(gameObject);
        }




        //if (targetUnit != null)
        //{
        //    Vector3 pos = transform.position;
        //    Vector3 targetPos = targetUnit.transform.position;
        //    targetPos.y = pos.y;

        //    //transform.position = Vector3.MoveTowards(pos, targetPos, speed * Time.fixedDeltaTime);

        //    float distance = Vector3.Distance(pos, targetPos);
        //    float distancePerFrame = speed * Time.fixedDeltaTime;
        //    if (distance > hitDistance + distancePerFrame)
        //    {
        //        Vector3 direction = (targetPos - pos).normalized;

        //        rb.velocity = direction * speed;
        //        rb.MoveRotation(Quaternion.LookRotation(direction));
        //    }
        //    else
        //    {
        //        onAttack.Invoke();
        //        Destroy(gameObject);
        //    }
        //}
        //else if (fortress != null)
        //{
        //    float distance = Vector3.Distance(transform.position, fortress.transform.position);

        //    if (distance > hitDistance)
        //    {
        //        Vector3 direction = (fortress.transform.position - transform.position).normalized;

        //        rb.velocity = direction * speed;
        //    }
        //    else
        //    {
        //        onAttack.Invoke();
        //        Destroy(gameObject);
        //    }
        //}
    }

    //private void Update()
    //{
    //    if (timeCheck < time)   // 날아가는 중
    //    {
    //        timeCheck += Time.deltaTime;
    //    }
    //    else    // 목표에 도달 -> 효과 적용 및 제거
    //    {
    //        if (targetUnit != null)// && !reachedTarget)
    //        {
    //           // reachedTarget = true;
    //            onAttack.Invoke();
    //        }

    //        //if (fortress != null && !reachedTarget)
    //        //{
    //        //    reachedTarget = true;
    //        //    onAttack.Invoke();
    //        //}

    //        Destroy(gameObject);
    //    }
    //}

    //public void Shoot(Vector3 dir)
    //{
    //    rb.velocity = dir * speed;
    //    timeCheck = 0f;
    //}



    //private void StickToTarget(Transform target)
    //{
    //    transform.SetParent(target);
    //    transform.localPosition = Vector3.zero + Vector3.up * transform.localPosition.y;

    //    rb.velocity = Vector3.zero;

    //    animator.SetTrigger("FadeOut");
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
    //    {
    //        Ingame_UnitCtrl unitCtrl = other.GetComponent<Ingame_UnitCtrl>();

    //        StickToTarget(unitCtrl.VisualModel.transform);
    //    }
    //}

    //private void ObjDestroy()
    //{
    //    Destroy(gameObject);
    //}
}
