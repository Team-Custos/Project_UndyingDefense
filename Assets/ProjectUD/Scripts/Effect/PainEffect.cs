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

        effectImage = target.ApplyEffectImage(iconSprite, false, 0);

        //float pos = target.GetEffectImagePosIndex();
        //if (pos == -2)
        //    return;

        //effectImagePool = target.EffectImagePool; 
        //GameObject obj = effectImagePool.GetEffectImage();        

        //effectImageObj = obj;
        //effectImage = obj.GetComponent<EffectImage>();
        //effectImage.SetIcon(iconSprite);
        //effectImage.Initialize(target);


        //effectImage.SetXOffset(pos);
        //effectImage.gameObject.SetActive(true);

        //target.effectImageList.Add(effectImage);

    }

    public override void OnRemove()
    {
        target.AddCriticalVulnerability(-critPercent);
        

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

    public override void Reapply(GameObject effectPrefab)
    {
        base.Reapply(effectPrefab);

        if(effectImage != null)
        {
            target.ReapplyEffectImage(effectImage, false, 0);
        }
    }
}
