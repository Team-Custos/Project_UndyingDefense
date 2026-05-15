using UnityEngine;

public class IndestructibleEffect : DurationEffect
{
    [SerializeField] private float percent;



    public override void Activate()
    {
        target.AddDamageTakenMult(percent);

        effectImage = target.ApplyEffectImage(iconSprite, false, 0);
    }

    public override void OnRemove()
    {
        target.AddCriticalVulnerability(-percent);


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
