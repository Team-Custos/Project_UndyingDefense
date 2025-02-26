using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSkill_GunShoot : AttackSkill
{
    public override void Activate(Unit caster, Unit target)
    {
        Debug.Log("TestSkill5 Activate");
        base.Activate(caster,target);
    }
}
