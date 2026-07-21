using UnityEngine;

public class PoisonEffect : TickEffect
{
    [Header("■ Poison Options")]
    [SerializeField] private GameObject vfx;
    [SerializeField] private float hpPercentDamage;

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
        target.TakeDamage(target.Maxhp * hpPercentDamage * 0.01f, null);
    }


    public override void Reapply(GameObject effectPrefab)
    {

    }
}
