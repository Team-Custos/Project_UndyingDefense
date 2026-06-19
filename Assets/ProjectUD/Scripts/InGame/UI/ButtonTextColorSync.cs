using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonTextColorSync : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI targetText;

    [Header("텍스트 색상")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color pressedColor; // 눌렸을 때
    [SerializeField] private Color highlightColor;  // 호버

    public void OnPointerDown(PointerEventData eventData)
    {
        targetText.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetText.color = normalColor; // 떼면 바로 복귀
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetText.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetText.color = normalColor;
    }
}
