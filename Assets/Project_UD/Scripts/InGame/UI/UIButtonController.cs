using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    public RectTransform buttonRectTransform;
    public Text buttonText;
    public GameObject textPanel;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f); 
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 1f);
    public Vector3 normalScale = Vector3.one;
    public float scaleDuration = 0.1f;

    private Vector3 originalTextScale;
    private Vector3 originalTextPanelScale;

    private void Start()
    {
        if (buttonText != null)
        {
            originalTextScale = buttonText.transform.localScale;
        }

        if (textPanel != null)
        {
            originalTextPanelScale = textPanel.transform.localScale;
        }

        buttonRectTransform.localScale = normalScale;
    }

    // 버튼 호버링 
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 호버링 시 버튼, 텍스트, 패널 크기 증가
        StartCoroutine(ScaleButton(hoverScale));
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextScale * 1.1f));
        }
        if (textPanel != null)
        {
            StartCoroutine(ScalePanel(originalTextPanelScale * 1.1f));
        }
    }

    // 버튼에서 마우스가 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(ScaleButton(normalScale));
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextScale));
        }
        if (textPanel != null)
        {
            StartCoroutine(ScalePanel(originalTextPanelScale)); 
        }
    }

    // 버튼 클릭 시 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        
        StartCoroutine(ScaleButton(pressedScale));  
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(pressedScale));
        }
        if (textPanel != null)
        {
            StartCoroutine(ScalePanel(pressedScale));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 클릭을 떼면 원래 크기로 돌아감
        StartCoroutine(ScaleButton(normalScale));
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextScale)); 
        }
        if (textPanel != null)
        {
            StartCoroutine(ScalePanel(originalTextPanelScale));
        }
    }

    // 버튼 크기 변경 코루틴
    private IEnumerator ScaleButton(Vector3 targetScale)
    {
        Vector3 currentScale = buttonRectTransform.localScale;
        float time = 0f;

        while (time < scaleDuration)
        {
            time += Time.deltaTime;
            buttonRectTransform.localScale = Vector3.Lerp(currentScale, targetScale, time / scaleDuration);
            yield return null;
        }

        buttonRectTransform.localScale = targetScale;
    }

    // 텍스트 크기 변경 코루틴
    private IEnumerator ScaleText(Vector3 targetScale)
    {
        Vector3 currentScale = buttonText.transform.localScale;
        float time = 0f;

        while (time < scaleDuration)
        {
            time += Time.deltaTime;
            buttonText.transform.localScale = Vector3.Lerp(currentScale, targetScale, time / scaleDuration);
            yield return null;
        }

        buttonText.transform.localScale = targetScale;
    }

    // 패널 크기 변경 코루틴
    private IEnumerator ScalePanel(Vector3 targetScale)
    {
        Vector3 currentScale = textPanel.transform.localScale;
        float time = 0f;

        while (time < scaleDuration)
        {
            time += Time.deltaTime;
            textPanel.transform.localScale = Vector3.Lerp(currentScale, targetScale, time / scaleDuration);
            yield return null;
        }

        textPanel.transform.localScale = targetScale;
    }
}
