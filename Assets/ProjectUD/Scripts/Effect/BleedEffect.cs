using UnityEngine;

public class BleedEffect : TickStackEffect
{
    [Header("■ Bleed Options")]
    [SerializeField] private float damagePerStack;

    [Header("■ VFX")]
    [SerializeField] private GameObject Vfx;
    [SerializeField] private GameObject overBleedVfx;

    [Header("■ Sound")]
    [SerializeField] private AudioClip bleedSound;
    [SerializeField] private AudioClip overBleedSound;


    private const float baseDamage = 1f;

    public override void Activate() 
    {
        //OnTick();
        Vfx.SetActive(true);
        SoundManager.Instance.PlaySFX(bleedSound, target.transform.position);

        effectImage = target.ApplyEffectImage(iconSprite, true, stack);

    }

    protected override void OnMaxStack()
    {
        // 과다 출혈 효과
        target.AddInstantEffect(overBleedVfx);
        target.TakeDamage(20, null);
        SoundManager.Instance.PlaySFX(overBleedSound, target.transform.position);
    }

    protected override void OnTick()
    {
        target.TakeDamage(baseDamage + (damagePerStack * stack), null);
        float a = baseDamage + (damagePerStack * stack);
    }

    public override void OnRemove()
    {
        Vfx.SetActive(false);

        if (effectImage != null)
        {
            target.RemoveEffectImage(effectImage);
            effectImage = null;
        }

        //if (effectImageObj != null)
        //{
        //    target.effectImageList.Remove(effectImage);
        //    effectImagePool.ReturnEffectImage(effectImageObj);
        //    effectImage.ResetTarget();
        //    effectImageObj = null;
        //    effectImage = null;
        //}
    }


}
