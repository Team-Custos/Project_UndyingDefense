using UnityEngine;

public class PainEffect : DurationEffect
{
    [Header("■ Pain Options")]
    [SerializeField] private float critPercent;

    public override void Activate()
    {
        target.AddCriticalVulnerability(critPercent);
    }

    public override void OnRemove()
    {
        target.AddCriticalVulnerability(-critPercent);
    }
}
