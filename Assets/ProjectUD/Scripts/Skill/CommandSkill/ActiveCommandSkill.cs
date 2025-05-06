using UnityEngine;
using static AttackSkill;
using static Unit;

public class ActiveCommandSkill : CommandSkill
{
    [Header("■ Data")]
    [SerializeField] private ActiveCommandSkillData data;

    [Header("■ Target")]
    [SerializeField] private LayerMask attackTargetLayer;

    public override CommandSkillData Data => data;

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 5;

    public void AreaAttack(Transform pivotTarget, float radius) //원형 공격
    {
        if (data.StartVFX != null)
        {
            GameObject VFXobj = Instantiate(data.StartVFX.gameObject);
            VFXobj.transform.SetParent(pivotTarget);
            VFXobj.transform.localPosition = Vector3.zero;// + Vector3.up * VFXobj.transform.localPosition.y;
            VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            Destroy(VFXobj, data.StartVFX.main.duration);
        }
        if (data.LoopVFX != null)
        {
            GameObject VFXobj = Instantiate(data.LoopVFX.gameObject);
            VFXobj.transform.SetParent(pivotTarget);
            VFXobj.transform.localPosition = Vector3.zero;// + Vector3.up * VFXobj.transform.localPosition.y;
            VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (targets == null)
            targets = new Collider[maxTargetCount];
        int targetCount = Physics.OverlapSphereNonAlloc
            (pivotTarget.transform.position, radius, targets, attackTargetLayer);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                Attack(target);
            }
        }
    }

    public void AreaAttack(Transform pivotTarget, float AreaX, float AreaY, float AreaZ)//사각형 공격
    {
        if (data.StartVFX != null)
        {
            GameObject VFXobj = Instantiate(data.StartVFX.gameObject);
            VFXobj.transform.SetParent(pivotTarget);
            VFXobj.transform.localPosition = Vector3.zero;// + Vector3.up * VFXobj.transform.localPosition.y;
            VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            Destroy(VFXobj, data.StartVFX.main.duration);
        }

        if (targets == null)
            targets = new Collider[maxTargetCount];

        int targetCount = Physics.OverlapBoxNonAlloc(pivotTarget.transform.position + Vector3.forward * AreaZ * 0.5f, new Vector3(AreaX, AreaY, AreaZ), targets);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                Attack(target);
            }
        }
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
    }

    public void ApplyEffect(Unit target, Effect effect)
    {
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
