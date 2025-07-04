using UnityEngine;

public class IgniteEffect : TickStackEffect
{
    [Header("■ Ignite Options")]
    [SerializeField] private float damagePerStack;

    protected override void OnMaxStack()
    {
        // 작열 효과
    }

    public override void Activate() { }
    protected override void OnTick()
    {
        target.TakeDamage(damagePerStack * stack);
    }

    public override void OnRemove() { }
}
