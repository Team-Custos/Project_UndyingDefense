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
        Debuff_Bleed b = target.AddComponent<Debuff_Bleed>();
        b.SetTarget(target);
    }
}
