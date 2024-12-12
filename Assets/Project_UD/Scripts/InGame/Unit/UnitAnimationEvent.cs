using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UnitAnimationEvent : MonoBehaviour
{
    public void Start()
    {
        Ingame_UnitCtrl unitCtrl = this.GetComponentInParent<Ingame_UnitCtrl>();
        UnitSkillManager unitSkill = unitCtrl.UnitSkill;
    }

    public void SpawnWarcryEnd()
    {
        Ingame_UnitCtrl unitCtrl = this.GetComponentInParent<Ingame_UnitCtrl>();
        unitCtrl.SpawnIdleEnd = true;
    }

    public void BowAttackStart()
    {
        Ingame_UnitCtrl unitCtrl = this.GetComponentInParent<Ingame_UnitCtrl>();
        UnitSkillManager unitSkill = unitCtrl.UnitSkill;

        unitSkill.attackStop = true;
        unitSkill.weaponCooldown_Cur = unitSkill.weaponCooldown_Cur = unitCtrl.cur_attackSpeed;
    }

    public void BowAttackEnd()
    {
        UnitSkillManager unitSkill = this.GetComponentInParent<Ingame_UnitCtrl>().UnitSkill;

        unitSkill.attackStop = false;
        unitSkill.weaponCooldown_Cur = 0;
    }

    public void DeadEnd()
    {
        Ingame_UnitCtrl unitCtrl = this.GetComponentInParent<Ingame_UnitCtrl>();
        Destroy(unitCtrl.gameObject, 2f);
        GridManager.inst.SetTilePlaceable(this.transform.position, true, true);
    }

}
