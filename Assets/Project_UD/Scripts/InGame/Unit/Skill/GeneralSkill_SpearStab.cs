using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSkill_SpearStab : AttackSkill
{
    public override void Activate(UnitCtrl_ReBuild target)
    {
        Debug.Log("TestSkill3 Activate");
        base.Activate(target);
    }
}
