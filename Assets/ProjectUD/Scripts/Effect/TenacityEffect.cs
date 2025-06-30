using UnityEngine;

public class TenacityEffect : DurationEffect
{
    [Header("■ Tenacity Options")]
    [SerializeField] private float attackSpeedPercent;

    public override void Activate()
    {
        target.AddAttackSpeedMult(attackSpeedPercent);
    }

    public override void Remove()
    {
        target.AddAttackSpeedMult(-attackSpeedPercent);
    }
}
