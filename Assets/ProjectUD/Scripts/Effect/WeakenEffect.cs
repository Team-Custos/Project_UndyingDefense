using UnityEngine;

public class WeakenEffect : StackEffect
{
    [Header("■ Weaken Options")]
    [SerializeField] private float damageTakenPercent;

    [Header("■ Max Stack")]
    [SerializeField] private GameObject maxStackEffectPrefab;

    protected override void OnStack()
    {
        target.AddDamageTakenMult(-damageTakenPercent);

    }

    public override void Activate()
    {
        base.Activate();
        effectImage = target.ApplyEffectImage(iconSprite, true, stack);
    }

    protected override void OnMaxStack()
    {
        target.AddEffect(maxStackEffectPrefab, target);
    }

    public override void OnRemove()
    {
        target.AddMoveSpeedMult(damageTakenPercent * stack);

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
            target.ReapplyEffectImage(effectImage, true, stack);
        }
    }

    public override bool IsSameType(GameObject effectPrefab)
    {
        return prefab == effectPrefab || maxStackEffectPrefab == effectPrefab;
    }
}
