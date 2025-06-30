using UnityEngine;

public class IgniteEffect : TickStackEffect
{
    [Header("■ Ignite Options")]
    [SerializeField] private float damagePerStack;

    protected override void OnMaxStack()
    {
        // 작열 효과
    }

    protected override void OnTick()
    {
        target.TakeDamage(damagePerStack * stack);
    }

    public override void Remove() { }
}
