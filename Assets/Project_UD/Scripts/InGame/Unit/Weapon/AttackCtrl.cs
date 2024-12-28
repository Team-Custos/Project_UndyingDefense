using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    Granade,
    Trap,
}

public class AttackCtrl : MonoBehaviour
{
    public Animator ModelAnimator;

    public AttackMethod MethodType;
    public ParticleSystem Particle;

    public AttackType Type;
    public UnitDebuff Debuff2Add;
    public int Damage = 1; //데미지
    public float moveSpeed = 1f;//(이동할 경우) 이동 속도
    public float Crit = 0;//치명타 확률

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
        else if (MethodType == AttackMethod.AttackTrigger_Circle)
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
            case AttackMethod.Granade:
                break;
            case AttackMethod.Trap:
                //CritPercentAdd = 5;
                break;
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
}
