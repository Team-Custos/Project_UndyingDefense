using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectImagePool : MonoBehaviour
{
    private Queue<EffectImage> effectImagePool = new Queue<EffectImage>();
    [SerializeField] private GameObject effectImagePrefab;


    public EffectImage GetEffectImage()
    {
        EffectImage effectImage = null;

        if (effectImagePool.Count > 0)
        {
            effectImage = effectImagePool.Dequeue();
        }
        else
            effectImage = CreateEffectImage();

        return effectImage;
    }


    private EffectImage CreateEffectImage()
    {

        GameObject obj = Instantiate(effectImagePrefab);
        obj.transform.SetParent(transform, false);

        EffectImage effectImage = obj.GetComponent<EffectImage>();
        effectImage.Initialize(this);
        return effectImage;
    }

    public void ReturnEffectImage(EffectImage effectImage)
    {
        effectImagePool.Enqueue(effectImage);
        effectImage.Disappear();
        //obj.SetActive(false);
    }

    public void SetIcon(EffectImage effectImage)
    {
        //EffectImage effectImage = EffectImagePool.Get
    }
}
