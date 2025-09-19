using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private DialTextTableLoader dialTextLoader;
    [SerializeField] private GameObject choiceUI; // choiceUI GameObject 자체 (활성/비활성 제어용)
    [SerializeField] private ChoiceUI choiceui;   // ChoiceUI 컴포넌트 (실제 UI 요소 제어용)

    private ChoiceArray choicearray; // currentChoiceArray 등으로 이름 변경 고려

    private void Start()
    {
        if (choiceUI != null) // Null 체크 추가
        {
            choiceUI.SetActive(false);
        }
    }

    public void ShowChoiceArray(ChoiceArray choiceArrayData) // 매개변수 이름 변경 (필드와 구분)
    {
        if (choiceui == null)
        {
            Debug.LogError("ChoiceManager: choiceui (ChoiceUI 컴포넌트)가 할당되지 않았습니다.");
            if (choiceUI != null) choiceUI.SetActive(false);
            return;
        }
        if (dialTextLoader == null)
        {
            Debug.LogError("ChoiceManager: tableLoader가 할당되지 않았습니다.");
            if (choiceUI != null) choiceUI.SetActive(false);
            return;
        }

        choiceui.ResetButton();
        this.choicearray = choiceArrayData;

        int visibleButtonIndex = 0; // 실제로 화면에 표시될 버튼의 인덱스
        
        for (int i = 0; i < this.choicearray.GetChoiceCount(); i++)
        {
            Choice currentChoice = this.choicearray.GetChoice(i);

            //if (currentChoice.IsConditionSatisfied())
            {
                if (visibleButtonIndex < choiceui.GetButtonCount()) // GetButtonCount()는 ChoiceUI에 추가 필요
                {
                    choiceui.SetButtonData(visibleButtonIndex, dialTextLoader.GetChoiceData(currentChoice.GetChoiceID()), currentChoice.NextEvent());
                    visibleButtonIndex++;
                }
                else
                {
                    Debug.LogWarning($"ChoiceManager: 표시할 수 있는 버튼의 최대 개수({choiceui.GetButtonCount()})를 초과했습니다. 선택지 '{currentChoice.GetChoiceID()}'는 표시되지 않습니다.");
                }
            }
        }

        // 실제로 사용된 버튼이 하나라도 있을 때만 UI 활성화
        if (visibleButtonIndex > 0)
        {
            if (choiceUI != null) choiceUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ChoiceManager: 조건을 만족하는 선택지가 하나도 없어 선택지 UI를 표시하지 않습니다.");
            if (choiceUI != null) choiceUI.SetActive(false); // 표시할 선택지가 없으면 UI 끔
        }
    }

    // 선택지 UI를 닫고 버튼 상태를 초기화하는 공용 메소드
    public void HideAndResetChoiceUI()
    {
        if (choiceUI != null)
        {
            choiceUI.SetActive(false);
        }
        if (choiceui != null)
        {
            choiceui.ResetButton(); // 버튼 상태도 초기화
        }
    }

    public void EndChoiceUI() // 외부 호출용
    {
        HideAndResetChoiceUI();

    }

    public void EscapeChoice()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideAndResetChoiceUI();
        }
    }
}
