using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArmorType = Unit.ArmorType;

public class ArmorDestructionEffect : DurationEffect
{
    private ArmorType targetArmorType;

    public override void Activate()
    {
        targetArmorType = target.unitArmorType;
        target.ChangeArmorType(ArmorType.NONE);
        effectImage = target.ApplyEffectImage(iconSprite, false, 0);

    }

    public override void OnRemove()
    {
        target.ChangeArmorType(targetArmorType);
        targetArmorType = ArmorType.NONE;

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
