using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommandSkillManager;
using static UnitDataManager;

public abstract class Skill : MonoBehaviour
{
    [Header("====General====")]
    private UnitCtrl_ReBuild target;
    [SerializeField] private float coolTime;
    private float curCoolTime;

    //각각의 스킬들의 추가 치명타율에 따른 유닛의 최종 치명타율을 계산하는 함수가 필요할 것 같음.

    public abstract void Activate(UnitCtrl_ReBuild target);

    public virtual void AddDebuff(UnitCtrl_ReBuild target)
    { }

    public void UnitSkillCooldownInit()
    {
        curCoolTime = 0;
    }

    public void Update()
    {
        if (target != null)
        {
            if (curCoolTime > 0)
            {
                curCoolTime -= Time.deltaTime;
            }
            else
            {
                Activate(target);
                curCoolTime = 0;
            }
        }
    }
}

//public class PU_GeneralSkill_MaceSmash : TestSkill
//{
//    public override void Activate(UnitCtrl_ReBuild target)
//    {
//        Debug.Log("TestSkill4 Activate");
//        //Cast를 써서 가상의 범위를 만드면 될것 같음. 프리팹으로 처리해도 좋고.
//    }
//}

public class AttackSkill : Skill
{
    [Header("====Attack====")]
    [SerializeField] protected float skillDamage = 5;
    [SerializeField] protected int skillBounsCritical = 0;
    [SerializeField] protected AttackType attackType = AttackType.Slash;
    [SerializeField] protected UnitDebuff debuff = UnitDebuff.Bleed;

    public override void Activate(UnitCtrl_ReBuild target)
    {
        skillDamage = caculateDamage(skillDamage, attackType, target.unitData.defenseType);
        // 대미지를 준다.
        target.TakeDamage(skillDamage);
    }

    protected float caculateDamage(float Damage, AttackType attackType, DefenseType defenseType)
    {
        if (defenseType == DefenseType.cloth)
        {
            if (attackType == AttackType.Crush)
            {
                Damage -= Damage * 0.3f;
            }
        }
        else if (defenseType == DefenseType.leather)
        {
            if (attackType == AttackType.Pierce)
            {
                Damage -= Damage * 0.3f;
            }
        }
        else if (defenseType == DefenseType.metal)
        {
            if (attackType == AttackType.Slash)
            {
                Damage -= Damage * 0.3f;
            }
        }

        if (Damage <= 0)
        {
            Damage = 0;
        }

        return Damage;
    }
}

public class BuffSkill : Skill
{
    public override void Activate(UnitCtrl_ReBuild target)
    {
        // 버프를 준다.
        //target.AddBuff(buffType, buffValue);
    }
}