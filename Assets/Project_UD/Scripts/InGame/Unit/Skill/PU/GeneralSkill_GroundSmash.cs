using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSkill_GroundSmash : AttackSkill
{
    [Header("====Required Components====")]
    private UnitCtrl_ReBuild UnitCtrl;
    public GameObject AttackTrigger;

    private void Awake()
    {
        UnitCtrl = GetComponent<UnitCtrl_ReBuild>();
    }

    public override void Activate(UnitCtrl_ReBuild target)
    {
        if (AttackTrigger != null)
        {
            GameObject AttackTriggerObj = Instantiate(AttackTrigger, UnitCtrl.transform);
            AttackCtrl attackCtrl = AttackTriggerObj.GetComponent<AttackCtrl>();
            attackCtrl.Damage = skillDamage;
            attackCtrl.Crit = UnitCtrl.curCrit;
            attackCtrl.Type = attackType;
            attackCtrl.Debuff2Add = debuff;
        }
    }
}
