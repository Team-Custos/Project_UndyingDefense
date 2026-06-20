using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakEffect : DurationEffect
{
    [Header("■ Weak Options")]
    [SerializeField] private float atpercent;


    public override void Activate()
    {
        target.AddAtkMult(atpercent);

        effectImage = target.ApplyEffectImage(iconSprite, false, 0);
    }

    public override void OnRemove()
    {
        target.AddAtkMult(-atpercent);


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
