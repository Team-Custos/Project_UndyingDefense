using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSkill_ClawSlash : AttackSkill
{
    public override void Activate(UnitCtrl_ReBuild target)
    {
        Debug.Log("TestSkill6 Activate");
        base.Activate(target);
    }
}
