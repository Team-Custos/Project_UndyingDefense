using UnityEngine;
using UnityEngine.Events;

//이 스크립트는 화살 오브젝트를 관리하기 위한 스크립트입니다.
public class ArrowCtrl : ProjectileCtrl
{
    private float timeCheck = 0f;
    private float time = 0f;

    private UnityEvent onAttack = new UnityEvent();

    private bool reachedTarget = false;

    //public float alphaColor = 1f;

    //public Color baseColor;

    //private MaterialPropertyBlock block;

    //MeshRenderer meshRenderer;
    //Animator animator;


    // Start is called before the first frame update
    //void Start()
    //{
    //    //meshRenderer = GetComponent<MeshRenderer>();
    //    //animator = GetComponent<Animator>();
    //    //block = new MaterialPropertyBlock();
    //    timeCheck = 0f;
    //}

    public void SetEvent(UnityAction onAttack)
    {
        this.onAttack.AddListener(onAttack);
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
            if (targetUnit != null && !reachedTarget)
            {
                reachedTarget = true;
                onAttack.Invoke();
            }     
            Destroy(gameObject);
        }
    }

    public void Shoot(Vector3 dir)
    {
        rb.velocity = dir * speed;
        timeCheck = 0f;
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
