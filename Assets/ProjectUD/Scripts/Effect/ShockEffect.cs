using UnityEngine;

public class ShockEffect : StackEffect
{
    [Header("■ Shock Options")]
    [SerializeField] private float moveSpeedPercent;
    [SerializeField] private float attackSpeedPercent;

    [Header("■ VFX")]
    [SerializeField] private GameObject[] Vfx;

    [Header("■ Max Stack")]
    [SerializeField] private GameObject maxStackEffectPrefab;

    private GameObject currentVfx;

    protected override void OnStack()
    {
        target.AddMoveSpeedMult(moveSpeedPercent);
        target.AddAttackSpeedMult(attackSpeedPercent);

        if(currentVfx != null)
            currentVfx.SetActive(false);

        currentVfx = Vfx[stack - 1];
        currentVfx.transform.position = target.HeightPos.position;
        currentVfx.SetActive(true);

    }

    public override void Activate()
    {
        base.Activate();
        effectImage = target.ApplyEffectImage(iconSprite, true, stack);
    }

    protected override void OnMaxStack()
    {
        // 기절 효과 추가.
        target.AddEffect(maxStackEffectPrefab, target, target.HeightPos.position);
    }

    public override void OnRemove()
    {
        target.AddMoveSpeedMult(-moveSpeedPercent * stack);
        target.AddAttackSpeedMult(-attackSpeedPercent * stack);

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
