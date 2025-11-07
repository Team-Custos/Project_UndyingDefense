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

        EffectImagePool pool = target.EffectImagePool;  // 유닛이 가진 Pool 사용
        GameObject obj = pool.GetEffectImage();         // 풀에서 꺼냄

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
            target.EffectImagePool.ReturnEffectImage(effectImageObj);
            effectImageObj = null;
            effectImage = null;
        }
    }

}
