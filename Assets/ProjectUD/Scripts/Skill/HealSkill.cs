using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkill : SkillBase
{
    [Header("■ Data")]
    [SerializeField] private HealSkillData data; // 스킬 데이터
    public override SkillData Data => data; // 스킬 데이터

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 5;

    public void AreaSkill(Unit unit, Unit pivotTarget, float radius) //원형 공격
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];
        int targetCount = Physics.OverlapSphereNonAlloc(pivotTarget.transform.position, radius, targets, unit.EnemyLayer);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                ActivateSkill(target);
            }
        }
    }

    public void ActivateSkill(Unit target)
    {
<<<<<<< HEAD
        target.TakeDamage(-1 * (target.UnitStats.maxHp * 0.01f * data.HealAmountPercent));
=======
<<<<<<< Updated upstream
        target.TakeDamage(-1 * (target.Data.MaxHp * 0.01f * data.HealAmountPercent));
=======
        target.TakeDamage(-1 * (target.UnitStats.maxHp * 0.01f * data.HealAmountPercent));
>>>>>>> Stashed changes
>>>>>>> KimJK
    }
}
