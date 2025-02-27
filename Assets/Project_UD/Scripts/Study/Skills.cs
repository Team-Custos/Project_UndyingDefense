using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skills : MonoBehaviour
{
    [Header("====General====")]
    private Unit target;
    protected Unit caster;
    [SerializeField] private float coolTime;
    private float curCoolTime;

    //각각의 스킬들의 추가 치명타율에 따른 유닛의 최종 치명타율을 계산하는 함수가 필요할 것 같음.

    public abstract void Activate(Unit caster, Unit target);

    public virtual void AddEffect(Unit target)
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
                Activate(caster, target);
                curCoolTime = 0;
            }
        }
    }
}

public class AttackSkill : Skills
{
    [Header("====Attack====")]
    [SerializeField] protected float skillDamage = 5;
    [SerializeField] protected float skillBounsCritical = 0;
    [SerializeField] protected AttackType attackType = AttackType.Slash;
    [SerializeField] protected GameObject EffectOnCrit;


    public override void Activate(Unit caster, Unit target)
    {
        float Damage = skillDamage;
        float crit = caster.curCrit + skillBounsCritical;

        if ((target.unitData.defenseType == DefenseType.완충갑 && attackType == AttackType.Crush)
            || (target.unitData.defenseType == DefenseType.방탄갑 && attackType == AttackType.Pierce)
            || (target.unitData.defenseType == DefenseType.철갑 && attackType == AttackType.Slash))
        {
            Damage *= 0.7f;
            crit *= 0.7f;
        }
    }

    protected float getDamage(float damage)
    {
        return damage;
    }

    protected bool isCrit(float crit)
    {
        return (Random.Range(0f, 1f) <= crit);
    }

    protected virtual void Attack(Unit target, float Damage, bool isCrit)
    {
        target.TakeDamage(Damage);
        if (isCrit)
        {
            AddEffect(target);
        }
    }
}

public class BuffSkill : Skills
{
    public override void Activate(Unit caster, Unit target)
    {
        // 버프를 준다.
        //target.AddBuff(buffType, buffValue);
    }
}