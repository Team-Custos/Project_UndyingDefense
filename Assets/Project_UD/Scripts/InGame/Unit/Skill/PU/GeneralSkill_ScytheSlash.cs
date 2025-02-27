using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeneralSkill_ScytheSlash : AttackSkill
{
    public override void Activate(Unit caster, Unit target)
    {
        Debug.Log("TestSkill1 Activate");
        base.Activate(caster, target);
    }

    public override void AddEffect(Unit target)
    {
        UnitEffect debuff = EffectOnCrit.GetComponent<UnitEffect>();
        target.AddEffect(debuff);
    }
}
