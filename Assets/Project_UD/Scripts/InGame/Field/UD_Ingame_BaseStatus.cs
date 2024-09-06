using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_BaseStatus : MonoBehaviour
{
    public int BaseHPMax = 0;
    public int BaseHPCur = 0;


    // Start is called before the first frame update
    void Start()
    {
        BaseHPCur = BaseHPMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (BaseHPCur <= 0)
        {
            //패배처리
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject OBJ = collision.gameObject;
        //피격 판정
        if (OBJ.CompareTag(UD_CONSTANT.TAG_ATTACK))
        {
            UD_Ingame_AttackCtrl Attack = OBJ.GetComponent<UD_Ingame_AttackCtrl>();

            if (Attack.isEnemyAttack)
            {
                //Debug.Log(this.gameObject.name + " attack hit!");
                this.BaseHPCur -= Attack.Atk;
                if (Attack.MethodType == AttackMethod.Arrow)
                {
                    Destroy(OBJ);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject OBJ = other.gameObject;
        //피격 판정
        if (OBJ.CompareTag(UD_CONSTANT.TAG_ATTACK))
        {
            UD_Ingame_AttackCtrl Attack = OBJ.GetComponent<UD_Ingame_AttackCtrl>();

            if (Attack.isEnemyAttack)
            {
                //Debug.Log(this.gameObject.name + " attack hit!");
                this.BaseHPCur -= Attack.Atk;
                if (Attack.MethodType == AttackMethod.Arrow)
                {
                    Destroy(OBJ);
                }
            }
        }
    }
}
