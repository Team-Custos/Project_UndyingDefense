using UnityEngine;

public class StunEffect : DurationEffect
{
    [SerializeField] private GameObject shockEffectPrefab; 

    public override void Activate()
    {
        target.GetStun();
        transform.position = target.HeightPos.position;


        effectImage = target.ApplyEffectImage(iconSprite, false, 0);
    }

    public override void OnRemove()
    {
        target.RemoveStun();

        if(effectImage != null)
        {
            target.RemoveEffectImage(effectImage);
            effectImage = null;
        }
    }

    public override bool IsSameType(GameObject effectPrefab)
    {
        return prefab == effectPrefab || shockEffectPrefab == effectPrefab;
    }
}
