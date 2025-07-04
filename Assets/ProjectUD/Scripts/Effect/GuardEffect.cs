using UnityEngine;

public class GuardEffect : DurationEffect
{
    [Header("■ Guard Options")]
    [SerializeField] private float damageTakenMult;
    public override void Activate()
    {
        target.AddDamageTakenMult(damageTakenMult);
    }

    public override void OnRemove()
    {
        target.AddDamageTakenMult(-damageTakenMult);
    }
}
