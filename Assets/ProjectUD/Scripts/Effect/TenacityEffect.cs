using UnityEngine;

public class TenacityEffect : DurationEffect
{
    [Header("■ Tenacity Options")]
    [SerializeField] private float intervalPercent;

    public override void Activate()
    {
        target.ChangeInterval(intervalPercent);
        Debug.Log(target.Interval + " activate");
    }

    public override void OnRemove()
    {
        target.RevertInterval(intervalPercent);
        Debug.Log(target.Interval + " remove");
    }
}
