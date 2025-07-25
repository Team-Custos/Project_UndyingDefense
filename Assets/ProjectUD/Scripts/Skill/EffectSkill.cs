using UnityEngine;

public class EffectSkill : SkillBase
{
    [Header("■ Data")]
    [SerializeField] private EffectSkillData data; // 스킬 데이터
    public override SkillData Data => data; // 스킬 데이터
    [SerializeField] private LayerMask targetLayerMask; // 스킬이 적용될 대상 레이어 마스크
    private Unit unit;

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
                {
                    unit = pivotTarget;
                    continue; // 자기 자신 제외
                }
                    

                //ActivateSkill(target);
            }
        }
    }

    public void ActivateSkill(Unit target)
    {
        target.GetProvoked(unit);

        //if (Random.Range(0f, 1f) <= data.SuccessRate * 0.01f)
        //{
        //    target.GetProvoked(target); // 대상이 도발 상태가 되도록 함
        //    //target.AddEffect(data.EffectPrefab);
        //}
    }
}
