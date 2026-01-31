using UnityEngine;

public class FocusEffect : DurationEffect
{
    [Header("■ Focus Options")]
    [SerializeField] private float mental;

    public override void Activate()
    {
        target.AddMental(mental);

        effectImage = target.ApplyEffectImage(iconSprite, false, 0);
    }

    public override void OnRemove()
    {
        target.AddMental(-mental);

        if (effectImage != null)
        {
            target.RemoveEffectImage(effectImage);
            effectImage = null;
        }
    }

    public override void Reapply(GameObject effectPrefab)
    {
        base.Reapply(effectPrefab);

        if (effectImage != null)
        {
            target.ReapplyEffectImage(effectImage, false, 0);
        }
    }
}
