using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSkill_SwordSlash : AttackSkill
{
    public override void Activate(Unit caster, Unit target)
    {
        Debug.Log("TestSkill6 Activate");
        base.Activate(caster, target);
    }

    public override void AddEffect(Unit target)
    {
        GameObject Effect_Obj = Instantiate(EffectOnCrit);
        Effect_Obj.transform.parent = target.EffectParent.transform;
        Effect_Obj.transform.position = Vector3.zero;
        UnitEffect debuff = Effect_Obj.GetComponent<UnitEffect>();
        debuff.SetTarget(target);
        target.AddEffect(debuff);
    }
}
