using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectImagePool : MonoBehaviour
{
    private Queue<GameObject> effectImagePool = new Queue<GameObject>();
    private Sprite icon;
    private float duration;
    [SerializeField] private GameObject effectImagePrefab;


    public GameObject GetEffectImage()
    {
        GameObject obj = null;

        if (effectImagePool.Count > 0)
        {
            obj = effectImagePool.Dequeue();
        }
        else
            obj = CreateEffectImage();

        //obj.SetActive(true);
        return obj;

    }


    private GameObject CreateEffectImage()
    {
        GameObject obj = Instantiate(effectImagePrefab);
        obj.transform.SetParent(this.transform);
        EffectImage effectImage = obj.GetComponent<EffectImage>();
        effectImage.Initialize(this);

        return obj;
    }

    public void ReturnEffectImage(GameObject obj)
    {
        effectImagePool.Enqueue(obj);
        obj.SetActive(false);
    }
}
