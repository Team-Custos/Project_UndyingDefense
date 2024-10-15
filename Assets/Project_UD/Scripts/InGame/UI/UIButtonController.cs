using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform buttonRectTransform;
    public Text buttonText;
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 1f);
    public Vector3 normalScale = Vector3.one;
    public float scaleDuration = 0.1f;
    public float textScaleFactor = 1.2f; 

    private Vector3 originalTextScale;

    private void Start()
    {
        // 초기 텍스트 크기 저장
        if (buttonText != null)
        {
            originalTextScale = buttonText.transform.localScale;
        }

        buttonRectTransform.localScale = normalScale;
    }

    // 버튼이 눌렸을 때
    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼 크기를 작게 만듦
        StartCoroutine(ScaleButton(pressedScale));
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextScale * 0.9f));  // 텍스트 크기도 줄임
        }
    }


    // 버튼에서 마우스를 뗐을 때
    public void OnPointerUp(PointerEventData eventData)
    {
        // 버튼 크기를 원래대로 되돌림
        StartCoroutine(ScaleButton(normalScale));
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextScale));  // 텍스트 크기도 원래대로 되돌림
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

    

}
