using UnityEngine;

public class PoisonEffect : TickEffect
{
    [Header("■ Poison Options")]
    [SerializeField] private float hpPercentDamage;

    public override void Activate() { }

    public override void OnRemove() { }

    protected override void OnTick()
    {
        target.TakeDamage(target.Maxhp * hpPercentDamage * 0.01f);
    }
}
