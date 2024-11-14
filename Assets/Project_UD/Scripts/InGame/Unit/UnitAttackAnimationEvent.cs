using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackAnimationEvent : MonoBehaviour
{
    public void AttackStart()
    {
        Ingame_UnitCtrl unitCtrl = this.GetComponentInParent<Ingame_UnitCtrl>();
        UnitSkillManager unitSkill = unitCtrl.UnitSkill;

        unitSkill.attackStop = true;
        unitSkill.weaponCooldown_Cur = unitSkill.weaponCooldown_Cur = unitCtrl.cur_attackSpeed;
    }

    public void AttackEnd()
    {
        UnitSkillManager unitSkill = this.GetComponentInParent<Ingame_UnitCtrl>().UnitSkill;

        unitSkill.attackStop = false;
        unitSkill.weaponCooldown_Cur = 0;
    }
}
