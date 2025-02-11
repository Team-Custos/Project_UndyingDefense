using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 화살 오브젝트를 관리하기 위한 스크립트입니다.
public class ArrowCtrl : MonoBehaviour
{
    public int Atk = 1;
    public float speed = 1f;
    public bool isEnemyAttack = false;

    public float alphaColor = 1f;
    
    public Color baseColor;

    private MaterialPropertyBlock block;
    private Rigidbody rb;

    MeshRenderer meshRenderer;
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        block = new MaterialPropertyBlock();
        rb.velocity = Vector3.forward * speed * 0.5f;
    }


    private void StickToTarget(Transform target)
    {
        transform.SetParent(target);
        transform.localPosition = Vector3.zero + Vector3.up * transform.localPosition.y;

        rb.velocity = Vector3.zero;

        animator.SetTrigger("FadeOut");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            Ingame_UnitCtrl unitCtrl = other.GetComponent<Ingame_UnitCtrl>();

            StickToTarget(unitCtrl.VisualModel.transform);
        }
    }

    private void ObjDestroy()
    {
        Destroy(gameObject);
    }

}
