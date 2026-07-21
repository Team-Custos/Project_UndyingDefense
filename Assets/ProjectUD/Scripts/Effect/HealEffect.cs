using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : TickEffect
{
    [Header("■ Heal Options")]
    [SerializeField] private GameObject vfx;
    [SerializeField] private float hpPercentHeal;

    public override void Activate()
    {
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
        target.TakeDamage(target.Maxhp * -hpPercentHeal * 0.01f, null);
        //Debug.Log("회복 : " + -target.Maxhp * -hpPercentHeal * 0.01f);
        //Debug.Log("현재 체력 : " + target.Hp);
    }


    public override void Reapply(GameObject effectPrefab)
    {

    }
}
