using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkill_ShieldBash : AttackSkill
{ 
    public override void Activate(UnitCtrl_ReBuild target)
    {
        Debug.Log("TestSkill2 Activate");
        base.Activate(target);
    }
}
