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
        //target.AddEffectImage(duration, iconSprite);
        
    }

    public override void OnRemove()
    {
        target.AddCriticalVulnerability(-critPercent);
    }

}
