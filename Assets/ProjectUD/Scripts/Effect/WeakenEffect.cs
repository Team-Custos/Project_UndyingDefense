using UnityEngine;

public class WeakenEffect : DurationEffect
{
    [Header("■ Weaken Options")]
    [SerializeField] private float damageTakenPercent;

    public override void Activate()
    {
        target.AddDamageTakenMult(damageTakenPercent);
    }

    public override void OnRemove()
    {
        target.AddDamageTakenMult(-damageTakenPercent);
    }
}
