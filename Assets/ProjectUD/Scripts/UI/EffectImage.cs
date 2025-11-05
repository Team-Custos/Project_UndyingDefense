using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectImage : MonoBehaviour
{
    private Unit target;
    private EffectImagePool effectImagePool;
    private float duration = 3.0f;
    private Vector3 screenPos;
    [SerializeField] private Image effectIcon;

    public void Initialize(EffectImagePool effectImagePool)
    {
        this.effectImagePool = effectImagePool;
    }

    public void Initialize(Unit target, float duration, Sprite icon)
    {
        this.target = target;
        this.duration = duration;
        effectIcon.sprite = icon;
    }

    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0f)
        {
            effectImagePool.ReturnEffectImage(this.gameObject);
        }
        else
        {
            Vector3 worldPos = target.transform.position + Vector3.up * target.HeightPos.position.y;
            screenPos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = screenPos;
        }

        
    }
}