using UnityEngine;
using static Unit;
using AttackTriggerType = CommandSkillAttackTrigger.AttackTriggerType;
using AttackType = AttackData.AttackType;

public class ActiveCommandSkill : CommandSkill
{
    [Header("■ Data")]
    [SerializeField] private ActiveCommandSkillData data;

    [Header("■ Target")]
    [SerializeField] private LayerMask attackTargetLayer;
    private EnemyUnit prevMarkedTargetUnit;

    [Header("■ AreaTriggerObject")]
    [SerializeField] protected GameObject areaTriggerObject;
    [SerializeField] protected Vector3 incomingDirection = Vector3.zero;

    public override CommandSkillData Data => data;

    public LayerMask AttackTargetLayer => attackTargetLayer;

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
        if (tickTime > 0)
        {
            trigger.SetTickTime(tickTime);
        }

        trigger.SetData(data);
        trigger.SetTargetLayer(attackTargetLayer);
        trigger.SetTriggerType(AttackTriggerType.Shpere);
        trigger.SetArea(radius);
        trigger.SetIncomingDirection(incomingDirection);
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
        if (tickTime > 0)
        {
            trigger.SetTickTime(tickTime);
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
            float calcBlockRate = 1f - (0.3f * target.BlockPercent * 0.01f);
            calcDamage *= calcBlockRate;
        }

        calcDamage *= target.DamageTakenMult;

        target.TakeDamage(calcDamage);
        if (Random.Range(0f, 1f) <= data.InduseEffectSuccessRate * 0.01f)
        {
            if (data.InduseEffectPrefab != null)
            {
                ApplyEffect(target, data.InduseEffectPrefab);
            }
        }
    }

    public void ApplyEffect(Unit target, GameObject effectPrefab)
    {
        target.AddEffect(effectPrefab);
    }

    public void GetMark(Unit target)
    {
        if (prevMarkedTargetUnit != null)
        {
            prevMarkedTargetUnit.SetExecuted(false);
        }

        if (target.GetComponent<EnemyUnit>() != null)
        {
            EnemyUnit LastMarkEnemy = target.GetComponent<EnemyUnit>();

            LastMarkEnemy.SetExecuted(true);
            prevMarkedTargetUnit = LastMarkEnemy;
        }
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.AttackType == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackType == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackType == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }
}
