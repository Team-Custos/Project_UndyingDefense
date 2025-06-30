using UnityEngine;

public class HasteEffect : DurationEffect
{
    [Header("■ Haste Options")]
    [SerializeField] private float moveSpeedPercent;

    public override void Activate()
    {
        target.AddMoveSpeedMult(moveSpeedPercent);
    }

    public override void Remove()
    {
        target.AddMoveSpeedMult(-moveSpeedPercent);
    }
}
