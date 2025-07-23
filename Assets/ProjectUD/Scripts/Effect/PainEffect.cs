using UnityEngine;

public class PainEffect : DurationEffect
{
    [Header("■ Pain Options")]
    [SerializeField] private float critPercent;

    public override void Activate()
    {
        target.AddCriticalVulnerability(critPercent);
        Debug.Log("Add");
    }

    public override void OnRemove()
    {
        target.AddCriticalVulnerability(-critPercent);
        Debug.Log("Remove");
    }
}
