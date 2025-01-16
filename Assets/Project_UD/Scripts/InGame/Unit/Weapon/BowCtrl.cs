using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCtrl : MonoBehaviour
{
    public GameObject Arrow;

    public void ArrowShoot(bool isEnemyAttack, float ArrowPosY = 0)
    {
        GameObject arrow_Obj = Instantiate(Arrow);
        if(arrow_Obj != null)
        {
            arrow_Obj.transform.SetPositionAndRotation(transform.position + Vector3.up * ArrowPosY, transform.rotation);
            AttackCtrl arrowCtrl = arrow_Obj.GetComponent<AttackCtrl>();
        }
    }

    public void DoubleShot()
    {
        ArrowShoot(false, 0.3f);
        ArrowShoot(false, -0.3f);
    }
}