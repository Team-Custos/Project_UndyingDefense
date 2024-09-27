using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStatus : MonoBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        GameObject OBJ = other.gameObject;
        //피격 판정
        //if (OBJ.CompareTag(UD_CONSTANT.TAG_ATTACK))
        //{
        //    UD_Ingame_AttackCtrl Attack = OBJ.GetComponent<UD_Ingame_AttackCtrl>();

        //    if (Attack.isEnemyAttack)
        //    {
        //        //Debug.Log(this.gameObject.name + " attack hit!");
        //        this.BaseHPCur -= Attack.Atk;
        //        if (Attack.MethodType == AttackMethod.Arrow)
        //        {
        //            Destroy(OBJ);
        //        }
        //    }
        //}
    }

    public void ReceiveDamage(int Damage)
    {
        IEnumerator HitEffect()
        {
            this.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 255);

            yield return new WaitForSeconds(0.1f);

            this.GetComponent<MeshRenderer>().material.color = new Color32(255, 255, 255, 255);
        }

        StartCoroutine(HitEffect());

        BaseHPCur -= Damage;

       
    }

   

}
