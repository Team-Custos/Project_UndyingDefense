using UnityEngine;

public class EffectSkill : SkillBase
{
    [Header("■ Data")]
    [SerializeField] private EffectSkillData data; // 스킬 데이터
    public override SkillData Data => data; // 스킬 데이터
    [SerializeField] private LayerMask targetLayerMask; // 스킬이 적용될 대상 레이어 마스크

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 10;

    public void AreaEffectSkill(Unit unit, float radius) //원형
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        int targetCount = Physics.OverlapSphereNonAlloc(unit.transform.position, radius, targets, targetLayerMask);
        //Debug.Log(targetCount);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                if (target.IsDead)
                    continue;

                if (data.EffectPrefab != null)
                    ActivateEffect(unit, target);
                else
                    Debug.LogWarning("부여 할 상태 누락");     // 이펙트 스킬은 부여할 상태를 무조건 가지고 있어야함;
            }
        }
    }

    public void AreaRecovery(Unit unit, float radius)
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        int targetCount = Physics.OverlapSphereNonAlloc(unit.transform.position, radius, targets, targetLayerMask);
        //Debug.Log(targetCount);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                if (target.IsDead)
                    continue;

                target.RecoveryHp(1000);
                target.ActivateRecovery();
            }
        }
    }

    public void ShowVFX(Unit unit)
    {
        if (data.VFX != null)
        {
            unit.AddVFX(data.VFX, unit.transform.position);
        }
    }

    public void ShowVFX(Unit unit, bool isAttach)
    {
        if (data.VFX != null)   
        {
            unit.AddVFX(data.VFX, unit.transform, isAttach);
        }
    }

}
