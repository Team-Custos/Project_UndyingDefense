using UnityEngine;

public class TenacityEffect : DurationEffect
{
    [Header("■ Tenacity Options")]
    [SerializeField] private float intervalPercent;

    public override void Activate()
    {
        target.ChangeInterval(intervalPercent);
    }

    public override void OnRemove()
    {
        target.RevertInterval(intervalPercent);
    }
}
