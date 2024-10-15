using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonState
    {
        Default,
        Hovered,
        Clicked,
        ClickedAfter,
        Disabled
    }

    public Image buttonImage;
    public Color defaultColor = new Color(1f, 0.5f, 0f);
    public Color hoveredColor = new Color(1f, 0.8f, 0.6f);
    public Color clickedColor = Color.yellow;
    public Color disabledColor = Color.gray;
    public float animationDuration = 0.2f;

    private ButtonState currentState = ButtonState.Default;


    public RectTransform buttonRectTransform;
    public Text buttonText;
    public Vector3 pressedScale = new Vector3(1.2f, 1.2f, 1f);
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
    }

    // 버튼이 눌렸을 때
    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼 크기와 텍스트 크기 확대
        StartCoroutine(ScaleButton(pressedScale));
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextScale * textScaleFactor));
        }
    }

    // 버튼에서 마우스를 뗐을 때
    public void OnPointerUp(PointerEventData eventData)
    {
        // 버튼 크기와 텍스트 크기를 원래대로 되돌림
        StartCoroutine(ScaleButton(normalScale));
        if (buttonText != null)
        {
            StartCoroutine(ScaleText(originalTextScale));
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

    // 버튼 상태에 따른 색상 변경
    public void ChangeButtonState(ButtonState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case ButtonState.Default:
                StartCoroutine(ChangeColor(defaultColor));
                break;
            case ButtonState.Hovered:
                StartCoroutine(ChangeColor(hoveredColor));
                break;
            case ButtonState.Clicked:
                StartCoroutine(ChangeColor(clickedColor));
                break;
            case ButtonState.ClickedAfter:
                StartCoroutine(ChangeColor(defaultColor));
                break;
            case ButtonState.Disabled:
                StartCoroutine(ChangeColor(disabledColor));
                break;
        }
    }

    // 색상 변경 코루틴
    private IEnumerator ChangeColor(Color targetColor)
    {
        Color initialColor = buttonImage.color;
        float time = 0f;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            //buttonImage.color = Color.Lerp(initialColor, targetColor, time / animationDuration);
            yield return null;
        }

        buttonImage.color = targetColor; // 최종 색상 적용
    }


    // 마우스가 버튼 위로 올라올 때 호출
    public void OnPointerEnter()
    {
        if (currentState != ButtonState.Disabled)
            ChangeButtonState(ButtonState.Hovered);
    }

    // 마우스가 버튼에서 벗어날 때 호출
    public void OnPointerExit()
    {
        if (currentState != ButtonState.Disabled)
            ChangeButtonState(ButtonState.Default);
    }

    // 버튼이 클릭될 때 호출
    public void OnPointerClick()
    {
        if (currentState != ButtonState.Disabled)
            ChangeButtonState(ButtonState.Clicked);
    }

    // 버튼 비활성화
    public void DisableButton()
    {
        ChangeButtonState(ButtonState.Disabled);
    }


    // 버튼 활성화
    public void EnableButton()
    {
        ChangeButtonState(ButtonState.Default);
    }


}
