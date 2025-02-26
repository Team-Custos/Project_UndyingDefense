using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSkill_GroundSmash : AttackSkill
{
    [Header("====Required Components====")]
    private Unit UnitCtrl;
    public GameObject AttackTrigger;

    private void Awake()
    {
        UnitCtrl = GetComponent<Unit>();
    }

    public override void Activate(Unit caster, Unit target)
    {
        if (AttackTrigger != null)
        {
            GameObject AttackTriggerObj = Instantiate(AttackTrigger, UnitCtrl.transform);
            AttackCtrl attackCtrl = AttackTriggerObj.GetComponent<AttackCtrl>();
            attackCtrl.Damage = skillDamage;
            attackCtrl.Crit = UnitCtrl.curCrit;
            attackCtrl.Type = attackType;
            //attackCtrl.Debuff2Add = debuff;
        }
    }
}
