using UnityEngine;

public class BleedEffect : TickStackEffect
{
    [Header("■ Bleed Options")]
    [SerializeField] private float damagePerStack;

    private const float baseDamage = 1f;

    protected override void OnMaxStack()
    {
        // 과다 출혈 효과
    }

    protected override void OnTick()
    {
        target.TakeDamage(baseDamage + (damagePerStack * stack));
    }

    public override void Remove() { }
}
