using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ElectricShockEffect : DurationEffect
{
    [SerializeField] int mentalAmount;
    [SerializeField] float moveSpeedPercent;


    public override void Activate()
    {
        target.AddMental(mentalAmount);
        target.AddMoveSpeedMult(-moveSpeedPercent);

        effectImage = target.ApplyEffectImage(iconSprite, false, 0);
    }

    public override void OnRemove()
    {
        target.AddMental(-mentalAmount);
        target.AddMoveSpeedMult(moveSpeedPercent);

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
