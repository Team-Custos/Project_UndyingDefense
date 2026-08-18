using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : TickEffect
{
    [Header("■ Heal Options")]
    [SerializeField] private GameObject vfx;
    [SerializeField] private float healRate;

    public override void Activate()
    {
        transform.position = target.transform.position;
        effectImage = target.ApplyEffectImage(iconSprite, false, 0);
    }

    public override void OnRemove()
    {
        if (effectImage != null)
        {
            target.RemoveEffectImage(effectImage);
            effectImage = null;
        }
    }

    protected override void OnTick()
    {
        target.RecoveryHp(target.Maxhp * healRate);
        target.PlayHealStateSFX();
        Debug.Log($"대상 hp : {target.Maxhp}, 회복량 : {target.Maxhp * healRate}");
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
