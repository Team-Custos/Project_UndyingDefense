using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSkill_SwordSlash : AttackSkill
{
    public override void Activate(UnitCtrl_ReBuild target)
    {
        Debug.Log("TestSkill6 Activate");
        base.Activate(target);
    }

    public override void AddDebuff(UnitCtrl_ReBuild target)
    {
        GameObject Effect_Obj = Instantiate(EffectOnCrit);
        Effect_Obj.transform.parent = target.EffectParent.transform;
        UnitDebuff_Rebuild debuff = Effect_Obj.GetComponent<UnitDebuff_Rebuild>();
        debuff.SetTarget(target);
    }
}
