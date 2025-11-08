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
    public Image EffectIcon => effectIcon;
    [SerializeField] private float yOffset = 0.2f;

    public void Initialize(EffectImagePool effectImagePool)
    {
        this.effectImagePool = effectImagePool;
    }

    public void Initialize(Unit target)
    {
        this.target = target;
    }

    private void Update()
    {
        Vector3 worldPos = target.transform.position + Vector3.up * (target.HeightPos.position.y  + yOffset);
        screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = screenPos;


    }

    public void Return()
    {
        effectImagePool.ReturnEffectImage(this.gameObject);
        target = null;
    }
    
    public void SetIcon(Sprite sprite)
    {
        effectIcon.sprite = sprite;
    }
}