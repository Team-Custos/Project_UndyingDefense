using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectImage : MonoBehaviour
{
    private Unit target;
    private EffectImagePool effectImagePool;
    private Vector3 screenPos;
    [SerializeField] private Image effectIcon;
    public Image EffectIcon => effectIcon;
    [SerializeField] private float yOffset = 0.2f;

    [SerializeField] private float startOffset = 0.5f;
    [SerializeField] private float endOffset = 1.5f;
    [SerializeField] private float duration = 3f;

    private float offsetTimer = 0f;
    private float currentOffset;

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
        Vector3 worldPos = target.transform.position + Vector3.up * (target.HeightPos.position.y + yOffset); 
        screenPos = Camera.main.WorldToScreenPoint(worldPos); transform.position = screenPos;
    }
    
    public void SetIcon(Sprite sprite)
    {
        effectIcon.sprite = sprite;
    }
}