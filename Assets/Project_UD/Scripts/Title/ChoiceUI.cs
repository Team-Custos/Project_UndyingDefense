using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UltEvents;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private ChoiceButtonUI[] buttonUIArray;
    [SerializeField] private Image selectIndicator;

    public System.Action onFadeInComplete; // 선택지 연출 완료 콜백

    public void SetButtonData(int j, string choiceText, UltEvent choiceEvent)
    {
        buttonUIArray[j].SetButton(j, choiceText, choiceEvent);
        buttonUIArray[j].gameObject.SetActive(true);
    }

    public void ResetButton()
    {
        for (int i = 0; i < buttonUIArray.Length; i++)
        {
            buttonUIArray[i].gameObject.SetActive(false);
            buttonUIArray[i].ResetButton();
        }
    }
    public int GetButtonCount()
    {
        return buttonUIArray.Length;
    }

    public ChoiceButtonUI GetButton(int i)
    {
        return buttonUIArray[i];
    }

    // 페이드인 애니메이션의 마지막 프레임에 Animation Event로 연결
    public void OnFadeInAnimationComplete()
    {
        onFadeInComplete?.Invoke();
        onFadeInComplete = null;
    }

    private void OnDisable()
    {
        int a = 0;
    }
}
