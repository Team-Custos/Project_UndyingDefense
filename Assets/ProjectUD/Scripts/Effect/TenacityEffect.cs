using UnityEngine;

public class TenacityEffect : DurationEffect
{
    [Header("■ Tenacity Options")]
    [SerializeField] private float intervalPercent;

    public override void Activate()
    {
        target.ChangeInterval(intervalPercent);

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
        target.RevertInterval(intervalPercent);

        if (effectImageObj != null)
        {
            effectImagePool.ReturnEffectImage(effectImageObj);
            effectImageObj = null;
            effectImage = null;
        }
    }
}
