using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeneralSkill_ScytheSlash : AttackSkill
{
    public override void Activate(UnitCtrl_ReBuild target)
    {
        Debug.Log("TestSkill1 Activate");
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
