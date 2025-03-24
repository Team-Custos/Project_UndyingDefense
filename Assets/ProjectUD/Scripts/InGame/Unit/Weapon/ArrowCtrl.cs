using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 화살 오브젝트를 관리하기 위한 스크립트입니다.
public class ArrowCtrl : MonoBehaviour
{
    Unit targetUnit = null;
    float attackPower = 1f;
    [SerializeField] float speed = 1f;
    float timeCheck = 0f;
    float time = 0f;

    //public float alphaColor = 1f;

    //public Color baseColor;

    //private MaterialPropertyBlock block;
    private Rigidbody rb;

    //MeshRenderer meshRenderer;
    //Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        //meshRenderer = GetComponent<MeshRenderer>();
        //animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //block = new MaterialPropertyBlock();
        rb.velocity = Vector3.forward * speed * 0.5f;
        timeCheck = 0f;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetAttackPower(float power)
    {
        attackPower = power;
    }

    public void SetTarget(Unit target)
    {
        targetUnit = target;
    }

    public void CalculateTime(float distance)
    {
        time = distance / speed;
    }

    private void Update()
    {
        if (timeCheck < time)
        {
            timeCheck += Time.deltaTime;
        }
        else
        {
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(attackPower);
            }
            Destroy(gameObject);
        }
    }


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
