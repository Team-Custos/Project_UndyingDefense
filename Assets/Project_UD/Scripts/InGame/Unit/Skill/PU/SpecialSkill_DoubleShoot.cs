using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkill_DoubleShoot : AttackSkill
{
    [Header("====Required Components====")]
    public GameObject Bow;

    public override void Activate(UnitCtrl_ReBuild target)
    {
        Bow.transform.LookAt(target.transform.position);
        Bow.GetComponent<BowCtrl>().DoubleShot();
        base.Activate(target);
    }
}
