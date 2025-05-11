using UnityEngine;
using static AttackSkill;
using static Unit;
using AttackTriggerType = CommandSkillAttackTrigger.AttackTriggerType;

public class ActiveCommandSkill : CommandSkill
{
    [Header("■ Data")]
    [SerializeField] private ActiveCommandSkillData data;

    [Header("■ Target")]
    [SerializeField] private LayerMask attackTargetLayer;

    [Header("■ AreaTriggerObject")]
    [SerializeField] protected GameObject areaTriggerObject;

    public override CommandSkillData Data => data;

    public void AreaAttack(Transform pivotTarget, float radius, float tickTime = 0.1f, float lifeTime = 0f) //원형 공격
    {
        CommandSkillAttackTrigger trigger = 
            Instantiate(areaTriggerObject).GetComponent<CommandSkillAttackTrigger>();
        trigger.transform.position = pivotTarget.position;
        trigger.transform.rotation = pivotTarget.rotation;
        if (lifeTime > 0)
        {
            Destroy(trigger.gameObject, lifeTime);
        }
        trigger.SetData(data);
        trigger.SetTargetLayer(attackTargetLayer);
        trigger.SetTriggerType(AttackTriggerType.Shpere);
        trigger.SetArea(radius);
    }

    public void AreaAttack(Transform pivotTarget, float AreaX, float AreaY, float AreaZ, float tickTime = 0.1f, float lifeTime = 0f)//사각형 공격
    {
        CommandSkillAttackTrigger trigger =
            Instantiate(areaTriggerObject).GetComponent<CommandSkillAttackTrigger>();
        trigger.transform.position = pivotTarget.position;
        trigger.transform.rotation = pivotTarget.rotation;
        if (lifeTime > 0)
        {
            Destroy(trigger.gameObject, lifeTime);
        }
        trigger.SetData(data);
        trigger.SetTargetLayer(attackTargetLayer);
        trigger.SetTriggerType(AttackTriggerType.Box);
        trigger.SetArea(AreaX, AreaY, AreaZ);

    }

    public void Attack(Unit target)
    {
        float calcDamage = data.Damage;
        float calcCrit = (target.CritVulnerability + data.BonusCrit) * 0.01f;
        if (IsBlocked(target.Data.ArmorType))
        {
            float calcBlockRate = 1f - (0.3f * target.BlockRate);
            calcDamage *= calcBlockRate;
        }

        calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;

        target.TakeDamage(calcDamage);
        if (Random.Range(0f, 1f) <= data.InduseEffectSuccessRate * 0.01f)
        {
            if (data.InduseEffct != null)
            {
                ApplyEffect(target, data.InduseEffct);
            }
        }
    }

    public void ApplyEffect(Unit target, GameObject effectObject)
    {
        Effect effect = effectObject.GetComponent<Effect>();
        target.AddEffect(target, effect);
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.AttackType == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackType == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackType == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }
}
