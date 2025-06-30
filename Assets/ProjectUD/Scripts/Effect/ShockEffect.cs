using UnityEngine;

public class ShockEffect : StackEffect
{
    [Header("■ Shock Options")]
    [SerializeField] private float moveSpeedPercent;
    [SerializeField] private float attackSpeedPercent;

    protected override void OnStack()
    {
        target.AddMoveSpeedMult(moveSpeedPercent);
        target.AddAttackSpeedMult(attackSpeedPercent);
    }

    protected override void OnMaxStack()
    {
        // 기절 효과 추가.
    }

    public override void Remove()
    {
        target.AddMoveSpeedMult(-moveSpeedPercent * stack);
        target.AddAttackSpeedMult(-attackSpeedPercent * stack);
    }

}
