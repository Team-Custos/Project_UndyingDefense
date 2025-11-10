using Unity.VisualScripting;
using UnityEngine;

public class PainEffect : DurationEffect
{
    [Header("■ Pain Options")]
    [SerializeField] private float critPercent;

    [Header("■ Sound")]
    [SerializeField] private AudioClip painSound;
 

    public override void Activate()
    {
        target.AddCriticalVulnerability(critPercent);
        SoundManager.Instance.PlaySFX(painSound, target.transform.position);

        effectImagePool = target.EffectImagePool; 
        GameObject obj = effectImagePool.GetEffectImage();        

        effectImageObj = obj;
        effectImage = obj.GetComponent<EffectImage>();
        effectImage.SetIcon(iconSprite);
        effectImage.Initialize(target);

        obj.transform.position = target.HeightPos.position;

    }

    public override void OnRemove()
    {
        target.AddCriticalVulnerability(-critPercent);

        if (effectImageObj != null)
        {
            effectImagePool.ReturnEffectImage(effectImageObj);
            effectImageObj = null;
            effectImage = null;
        }
    }

}
