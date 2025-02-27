using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSkill_BowShoot : AttackSkill
{
    [Header("Required Component")]
    Unit unitCtrl;
    public GameObject Bow;

    private void Awake()
    {
        unitCtrl = GetComponentInParent<Unit>();
    }

    public override void Activate(Unit caster, Unit target)
    {
        Vector3 TargetPos = target.transform.position;


        //화살을 쏘아 한 명의 적에게 5 데미지의 관통 공격을 가한다. 치명타 발동 시 출혈 효과
        if (Bow != null && Bow.GetComponent<BowCtrl>() != null)
        {
            Bow.transform.LookAt(TargetPos);
            Bow.GetComponent<BowCtrl>().ArrowShoot(unitCtrl.CompareTag(CONSTANT.TAG_ENEMY));
        }

        Debug.Log("TestSkill2 Activate");
        base.Activate(caster, target);
    }
}
