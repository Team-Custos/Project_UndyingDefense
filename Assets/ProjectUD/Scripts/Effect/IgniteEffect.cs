using System.Runtime.InteropServices;
using UnityEngine;

public class IgniteEffect : TickStackEffect
{
    [Header("■ Ignite Options")]
    [SerializeField] private float damagePerStack;

    [Header("■ VFX")]
    [SerializeField] private GameObject Vfx;
    [SerializeField] private GameObject infernoVfx;

    [Header("■ Sound")]
    [SerializeField] private AudioClip igniteSound;



    protected override void OnMaxStack()
    {
        //if(!target.HasEffect<InfernoEffect>())
        //{
        //    target.AddEffect(infernoVfx);
        //}

        target.AddEffect(infernoVfx, target, Vector3.zero);
        //target.ApplyEffectImage(iconSprite);

        //infernoEffect.Activate();
        SetCaster(target);
        
        Remove();
    }

    public override void Activate() 
    {
        Vfx.SetActive(true);
        SoundManager.Instance.PlaySFX(igniteSound, target.transform.position);
        effectImage = target.ApplyEffectImage(iconSprite, false, stack);
    }
    protected override void OnTick()
    {
        target.TakeDamage(damagePerStack);
    }

    public override void OnRemove() 
    {
        Vfx.SetActive(false);

        if (effectImage != null)
        {
            target.RemoveEffectImage(effectImage);
            effectImage = null;
        }
    }

    public override bool IsSameType(GameObject effectPrefab)
    {
        return prefab == effectPrefab || infernoVfx == effectPrefab;
    }

}
