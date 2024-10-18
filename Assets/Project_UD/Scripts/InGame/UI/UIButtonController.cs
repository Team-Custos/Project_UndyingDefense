using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    public RectTransform buttonRectTransform;
    public Text buttonText;
    public GameObject textPaenl;
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 1f);
    public Vector3 normalScale = Vector3.one;
    public float scaleDuration = 0.1f;
    public float textScaleFactor = 1.2f;

    private Vector3 originalTextScale;
    private Vector3 originalTextPaenlScale;


    private void Start()
    {
        // 초기 텍스트 크기 저장
        if (buttonText != null)
        {
            originalTextScale = buttonText.transform.localScale;
        }

        if (textPaenl != null)
        {
            originalTextPaenlScale = textPaenl.transform.localScale;
        }

        buttonRectTransform.localScale = normalScale;
    }

    // 버튼 호버링
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 버튼 크기 zmrp
        StartCoroutine(ScaleButton(pressedScale));
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextScale * 0.9f));  // 버튼 텍스트
            StartCoroutine(ScaleText(originalTextScale * 0.9f));  // 버튼 텍스 판텔
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }


    // 버튼 클릭
    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼 크기를 원래대로 되돌림
        StartCoroutine(ScaleButton(normalScale));
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextScale));  // 텍스트 크기도 원래대로 되돌림
        }
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextPaenlScale));  // 텍스트 크기도 원래대로 되돌림
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
        Vector3 currentScale = textPaenl.transform.localScale;
        float time = 0f;

        while (time < scaleDuration)
        {
            time += Time.deltaTime;
            textPaenl.transform.localScale = Vector3.Lerp(currentScale, targetScale, time / scaleDuration);
            yield return null;
        }

        buttonText.transform.localScale = targetScale;
    }


    private IEnumerator ScalePanel(Vector3 targetScale)
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