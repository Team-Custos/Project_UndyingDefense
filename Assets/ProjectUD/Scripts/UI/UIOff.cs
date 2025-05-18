using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOnOff : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private float fadeDuration = 0.5f; // 페이드 인/아웃 시간
    private float showDuration = 2.0f; // UI가 유지되는 시간

    private float timer = 0f;
    private bool isFadingIn = true;
    private bool isShowing = false;
    private bool isFadingOut = false;


    private void OnEnable()
    {
        timer = 0f;
        isFadingIn = true;
        isShowing = false;
        isFadingOut = false;
        canvasGroup.alpha = 0f;
    }

    private void OnDisable()
    {
        timer = 0f;
    }

    private void Update()
    {
        if (isFadingIn)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);

            if (timer >= fadeDuration)
            {
                isFadingIn = false;
                isShowing = true;
                timer = 0f;
            }
        }
        else if (isShowing)
        {
            timer += Time.deltaTime;

            if (timer >= showDuration)
            {
                isShowing = false;
                isFadingOut = true;
                timer = 0f;
            }
        }
        else if (isFadingOut)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (timer / fadeDuration));

            if (timer >= fadeDuration)
            {
                isFadingOut = false;
                gameObject.SetActive(false);
            }
        }
    }

    //private float delay = 3.0f;

    //private void OnEnable()
    //{
    //    Invoke("OffUI", delay);
    //}

    //private void OffUI()
    //{
    //    gameObject.SetActive(false);
    //}

    //private void OnDisable()
    //{
    //    // UI가 비활성화되면 OffUI 호출 예약 취소
    //    CancelInvoke("OffUI");
    //}
}