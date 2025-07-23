using UnityEngine;

public class EffectSkill : SkillBase
{
    [Header("■ Data")]
    [SerializeField] private EffectSkillData data; // 스킬 데이터
    public override SkillData Data => data; // 스킬 데이터
    [SerializeField] private LayerMask targetLayerMask; // 스킬이 적용될 대상 레이어 마스크

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 5;

    public void AreaEffectSkill(Unit pivotTarget, float radius) //원형
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        int targetCount = Physics.OverlapSphereNonAlloc(pivotTarget.transform.position, radius, targets, targetLayerMask);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                if(target == pivotTarget)
                    continue; // 자기 자신 제외

                ActivateSkill(target);
            }
        }
    }

    public void ActivateSkill(Unit target)
    {
        if (Random.Range(0f, 1f) <= data.SuccessRate * 0.01f)
        {
            //Debug.Log($"[EffectSkill] {target.name}에게 {data.Effect} 효과를 부여합니다.");
            target.AddEffect(data.EffectPrefab);
        }
    }
}
