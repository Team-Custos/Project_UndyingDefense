using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WSWhitehouse.TagSelector;

//이 스크립트는 공격 판정 트리거를 위한 스크립트입니다.
public enum AttackType //공격 속성
{
    Pierce = 0,
    Slash = 1,
    Crush = 2,
    Magic = 3,
    UnKnown = 99,
}

public enum AttackMethod //공격 방식
{
    Arrow,
    AttackTrigger_Circle,
    AttackTrigger_Fan,
    Granade,
    Trap,
}

public class AttackCtrl : MonoBehaviour
{
    [Header("====General====")]
    public Animator ModelAnimator;
    public AttackMethod MethodType;
    public ParticleSystem Particle;
    public AttackType Type;
    public UnitDebuff Debuff2Add;
    public float Damage = 1; //데미지
    public float moveSpeed = 1f;//(이동할 경우) 이동 속도
    public float Crit = 0;//치명타 확률

    [Header("====Trigger====")]
    [TagSelector] public string tagToSearch; //찾을 오브젝트의 태그 설정.
    public List<GameObject> detectedObjects = new List<GameObject>();//감지된 게임 오브젝트.
    public List<GameObject> ignoreList = new List<GameObject>();//무시할 게임 오브젝트 (자기 자신 등)
    [SerializeField]private float fanAngle = 90;


    // Start is called before the first frame update
    void Start()
    {
        if (MethodType == AttackMethod.Arrow)
        {
            Destroy(gameObject, 3f);
        }
        else if (MethodType == AttackMethod.Granade)
        {
            Destroy(gameObject, 1f);
        }
        else if (MethodType == AttackMethod.AttackTrigger_Circle
            || MethodType == AttackMethod.AttackTrigger_Fan)
        {
            Destroy(gameObject, Particle.main.duration);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (MethodType)
        {
            case AttackMethod.Arrow:
                transform.Translate(0.5f * moveSpeed * Vector3.forward);
                break;
            case AttackMethod.AttackTrigger_Circle:
                break;
            case AttackMethod.AttackTrigger_Fan:
                break;
            case AttackMethod.Granade:
                break;
            case AttackMethod.Trap:
                //CritPercentAdd = 5;
                break;
        }        
    }

    private void ListTargetDelete(GameObject latestTarget)//리스트 초기화.
    {
        if (detectedObjects.Contains(latestTarget))
        {
            detectedObjects.Remove(latestTarget);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.TryGetComponent(out Ingame_UnitCtrl unit))
        {
            if (other.CompareTag(tagToSearch.ToString()))//찾을 태그에 맞을 경우. (외부 스크립트 사용.)
            {
                if (detectedObjects.Contains(unit.gameObject) == false)
                {
                    detectedObjects.Add(unit.gameObject); //리스트에 추가.
                }
            }
            else return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(CONSTANT.TAG_ENEMY))//적군에게 닿았을 경우
        {
            Ingame_UnitCtrl EnemyCtrl = other.gameObject.GetComponent<Ingame_UnitCtrl>();

            if (this.MethodType == AttackMethod.Granade)
            {
                if (EnemyCtrl != null)
                {
                    Debug.Log("Granade Hit!");
                    EnemyCtrl.ReceivePhysicalDamage(Damage, Crit + 10, Type, Debuff2Add);
                }
            }
            else if(this.MethodType == AttackMethod.AttackTrigger_Circle)
            {
                if (EnemyCtrl != null)
                {
                    Debug.Log("CircleTrigger Hit!");
                    EnemyCtrl.ReceivePhysicalDamage(Damage, Crit + 10, Type, Debuff2Add);
                }
            }
            else if (this.MethodType == AttackMethod.AttackTrigger_Fan)
            {
                if (EnemyCtrl != null)
                {
                    Vector2 EnemyPosition = new Vector2(EnemyCtrl.transform.position.x, EnemyCtrl.transform.position.z);
                    Vector2 AttackPosition = new Vector2(this.transform.position.x, this.transform.position.z);

                    float Angle = Vector2.Angle(EnemyPosition - AttackPosition, Vector2.up);

                    if (Angle <= fanAngle)
                    {
                        Debug.Log("FanTrigger Hit!");
                        EnemyCtrl.ReceivePhysicalDamage(Damage, Crit + 10, Type, Debuff2Add);
                    }
                }
            }
            else if (this.MethodType == AttackMethod.Trap)
            {
                if (EnemyCtrl != null)
                {
                    Debug.Log("Trap Hit!");
                    ModelAnimator.SetTrigger("TrapTriggered");
                    EnemyCtrl.ReceivePhysicalDamage(Damage, Crit + 10, Type, Debuff2Add);
                    Destroy(this.gameObject, 5f);
                }
            }
        }
        else if (other.gameObject.CompareTag(CONSTANT.TAG_UNIT))//아군에게 닿았을 경우
        {
            Ingame_UnitCtrl AllyCtrl = other.gameObject.GetComponent<Ingame_UnitCtrl>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(CONSTANT.TAG_ENEMY))//적군이 범위를 벗어났을 경우
        {
            Ingame_UnitCtrl EnemyCtrl = other.gameObject.GetComponent<Ingame_UnitCtrl>();
            ListTargetDelete(EnemyCtrl.gameObject);
        }
        else if (other.gameObject.CompareTag(CONSTANT.TAG_UNIT))//아군이 범위를 벗어났을 경우
        {
            Ingame_UnitCtrl AllyCtrl = other.gameObject.GetComponent<Ingame_UnitCtrl>();
        }
    }
}
