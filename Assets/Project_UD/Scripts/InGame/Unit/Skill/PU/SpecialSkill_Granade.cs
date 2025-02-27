using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkill_Granade : AttackSkill
{
    [Header("====Required components====")]
    Unit unitCtrl;
    public GameObject Granade;

    private void Awake()
    {
        unitCtrl = GetComponentInParent<Unit>();
    }

    public override void Activate(Unit caster, Unit target)
    {
        //유닛 스킬 애니메이션 실행
        //unitCtrl. unitAnimator.SetTrigger(CONSTANT.ANITRIGGER_SPECIAL);
        GameObject Granade_Obj = Instantiate(Granade);
        Granade_Obj.transform.position = unitCtrl.transform.position;
        GranadeCtrl granade = Granade_Obj.GetComponent<GranadeCtrl>();
        granade.targetPos = target.transform.position;
        granade.AttackTrigger.GetComponent<AttackCtrl>().Damage = skillDamage;
        granade.AttackTrigger.GetComponent<AttackCtrl>().Crit = unitCtrl.curCrit;
        granade.AttackTrigger.GetComponent<AttackCtrl>().Type = attackType;
        //granade.AttackTrigger.GetComponent<AttackCtrl>().Debuff2Add = debuff;
    }
}
