using UnityEngine;

public class StunEffect : DurationEffect
{
    [SerializeField] private GameObject shockEffectPrefab; 

    public override void Activate()
    {
        target.GetStun();
    }

    public override void OnRemove()
    {
        target.RemoveStun();
    }

    public override bool IsSameType(GameObject effectPrefab)
    {
        return prefab == effectPrefab || shockEffectPrefab == effectPrefab;
    }
}
