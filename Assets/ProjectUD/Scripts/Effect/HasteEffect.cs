using UnityEngine;

public class HasteEffect : DurationEffect
{
    [Header("■ Haste Options")]
    [SerializeField] private float moveSpeedPercent;

    public override void Activate()
    {
        target.AddMoveSpeedMult(moveSpeedPercent);

        effectImage = target.ApplyEffectImage(iconSprite, false, 0);

    }

    public override void OnRemove()
    {
        target.AddMoveSpeedMult(-moveSpeedPercent);

        if (effectImage != null)
        {
            target.RemoveEffectImage(effectImage);
            effectImage = null;
        }
    }

    public override void Reapply(GameObject effectPrefab)
    {
        base.Reapply(effectPrefab);

        if (effectImage != null)
        {
            target.ReapplyEffectImage(effectImage, false, 0);
        }
    }
}
