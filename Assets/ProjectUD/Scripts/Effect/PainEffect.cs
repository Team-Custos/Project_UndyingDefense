using UnityEngine;

public class PainEffect : DurationEffect
{
    [Header("■ Pain Options")]
    [SerializeField] private float blockPercent;

    public override void Activate()
    {
        target.AddBlockPercent(blockPercent);
    }

    public override void OnRemove()
    {
        target.AddBlockPercent(-blockPercent);
    }
}
