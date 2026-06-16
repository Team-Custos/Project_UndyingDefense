using UnityEngine;

public class FearEffect : DurationEffect
{
    [SerializeField] private GameObject weakenEffectPrefab;

    [Header("■ Fear Options")]
    [SerializeField] private float atkPercent;
    [SerializeField] private int mental;

    public override void Activate()
    {
        target.AddAtkMult(atkPercent);
        target.AddMental(mental);

        effectImage = target.ApplyEffectImage(iconSprite, false, 0);
    }

    public override void OnRemove()
    {
        target.AddAtkMult(-atkPercent);
        target.AddMental(-mental);

        if (effectImage != null)
        {
            target.RemoveEffectImage(effectImage);
            effectImage = null;
        }
    }

    public override bool IsSameType(GameObject effectPrefab)
    {
        return prefab == effectPrefab || weakenEffectPrefab == effectPrefab;
    }
}
