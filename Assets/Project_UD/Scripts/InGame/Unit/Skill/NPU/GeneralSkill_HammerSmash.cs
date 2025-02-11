using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSkill_HammerSmash : AttackSkill
{
    public override void Activate(UnitCtrl_ReBuild target)
    {
        Debug.Log("TestSkill7 Activate");
        base.Activate(target);
    }
}
