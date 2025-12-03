using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class EffectImage : MonoBehaviour
{
    private Unit target;
    private EffectImagePool effectImagePool;
    private Vector3 screenPos;
    [SerializeField] private Image effectIconImage;
    [SerializeField] private TextMeshProUGUI stackText;

    private float posIndex = 0f;
    [SerializeField] private float distance = 40f;
    [SerializeField] private float duration = 0.5f;
    float durationCheck = 0f;

    private float startYOffset = -20f; 
    [SerializeField] private float endYOffset = 20f;    
    private float currentYOffset = 0f;
    private bool isDisappaer = false;


    public void Initialize(EffectImagePool effectImagePool)
    {
        this.effectImagePool = effectImagePool;
    }

    public void Initialize(Unit target)
    {
        this.target = target;
        durationCheck = 0f;
        isDisappaer = false;
        InitializePosition();
    }

    private void InitializePosition()
    {
        Vector3 worldPos = target.transform.position + Vector3.up * (target.HeightPos.position.y);
        screenPos = Camera.main.WorldToScreenPoint(worldPos); transform.position = screenPos;
        transform.position = screenPos;

        Color color = effectIconImage.color;
        color.a = 0f;
        effectIconImage.color = color;
    }

    private void Update()
    {
        if (target == null)
            return;

        Vector3 worldPos = target.transform.position + Vector3.up * target.HeightPos.position.y;

        screenPos = Camera.main.WorldToScreenPoint(worldPos);

        screenPos.x += posIndex * distance;

        durationCheck += Time.deltaTime;
        float t = Mathf.Clamp01(durationCheck / duration);

        if (!isDisappaer)
        {
            if (durationCheck < duration)
            {
                currentYOffset = Mathf.Lerp(startYOffset, endYOffset, t);

                Color color = effectIconImage.color;
                color.a = t;
                effectIconImage.color = color;
            }
        }
        else
        {
            if (durationCheck < duration)
            {
                currentYOffset = Mathf.Lerp(endYOffset, startYOffset, t);

                Color color = effectIconImage.color;
                color.a = 1f - t;
                effectIconImage.color = color;
            }
            else
                gameObject.SetActive(false);
        }
        

        screenPos.y += currentYOffset;

        transform.position = screenPos;


    }

    public void SetIcon(Sprite icon)
    {
        effectIconImage.sprite = icon;
    }

    public void SetXOffset(float posIndex)
    {
        this.posIndex = posIndex;
    }

    public void ResetTarget()
    {
        target = null;
    }

    public void Disappear()
    {
        isDisappaer = true;
        durationCheck = 0f;
    }

    public void SetStack(bool hasStack, int stack)
    {
        if(hasStack)
        {
            stackText.gameObject.SetActive(true);
            stackText.text = stack.ToString();
        }
        else
        {
            stackText.gameObject.SetActive(false);
        }
    }

    public void Reapply()
    {

    }
}