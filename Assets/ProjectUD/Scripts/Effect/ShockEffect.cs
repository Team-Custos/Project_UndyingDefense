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
        currentVfx.SetActive(true);

    }

    protected override void OnMaxStack()
    {
        // 기절 효과 추가.
        target.AddEffect(maxStackEffectPrefab);
    }

    public override void OnRemove()
    {
        target.AddMoveSpeedMult(-moveSpeedPercent * stack);
        target.AddAttackSpeedMult(-attackSpeedPercent * stack);
    }

    public override bool IsSameType(GameObject effectPrefab)
    {
        return prefab == effectPrefab || maxStackEffectPrefab == effectPrefab;
    }
}
