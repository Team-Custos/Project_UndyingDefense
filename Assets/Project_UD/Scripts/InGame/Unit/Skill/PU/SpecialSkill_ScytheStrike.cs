using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkill_ScytheStrike : AttackSkill
{
    public override void Activate(UnitCtrl_ReBuild target)
    {
        Debug.Log("TestSkill1 Activate");
        base.Activate(target);
    }
}
