using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkill_DoubleShoot : AttackSkill
{
    [Header("====Required Components====")]
    public GameObject Bow;

    public override void Activate(Unit caster, Unit target)
    {
        Bow.transform.LookAt(target.transform.position);
        Bow.GetComponent<BowCtrl>().DoubleShot();
        base.Activate(caster, target);
    }

    public override void AddEffect(Unit target)
    {
        GameObject Effect_Obj = Instantiate(EffectOnCrit);
        Effect_Obj.transform.parent = target.EffectParent.transform;
        UnitEffect debuff = Effect_Obj.GetComponent<UnitEffect>();
        debuff.SetTarget(target);
    }
}
