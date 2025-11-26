using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ChoiceManager : MonoBehaviour, IInputOnSpace, IInputUpArrow, IInputDownArrow
{
    [SerializeField] private DialTextTableLoader dialTextLoader;
    [SerializeField] private GameObject choiceUIObj; // choiceUI GameObject 자체 (활성/비활성 제어용)
    [SerializeField] private ChoiceUI choiceui;   // ChoiceUI 컴포넌트 (실제 UI 요소 제어용)
    [SerializeField] private Image selectIndicator;
    [SerializeField] private Image selectedUI;

    [Header("InputManager")]
    [SerializeField] private PlayerInputEventManager inputManager;
    private int indicatorIndex = 0;

    private ChoiceArray choicearray; // currentChoiceArray 등으로 이름 변경 고려

    private void Start()
    {
        if (choiceUIObj != null) // Null 체크 추가
        {
            choiceUIObj.SetActive(false);
            choiceui.gameObject.SetActive(false);
        }

    }

    public void ShowChoiceArray(ChoiceArray choiceArrayData) // 매개변수 이름 변경 (필드와 구분)
    {
        inputManager.OnSpaceTarget = this;
        inputManager.OnUpArrowTarget = this;
        inputManager.OnDownArrowTarget = this;

        if (choiceui == null)
        {
            Debug.LogError("ChoiceManager: choiceui (ChoiceUI 컴포넌트)가 할당되지 않았습니다.");
            if (choiceUIObj != null) choiceUIObj.SetActive(false);
            return;
        }
        if (dialTextLoader == null)
        {
            Debug.LogError("ChoiceManager: tableLoader가 할당되지 않았습니다.");
            if (choiceUIObj != null) choiceUIObj.SetActive(false);
            return;
        }

        choiceui.ResetButton();
        this.choicearray = choiceArrayData;

        indicatorIndex = 0;
        selectIndicator.transform.position = choiceui.GetButton(indicatorIndex).transform.position;      // 선택될 선택지 표시 위치

        int visibleButtonIndex = 0; // 실제로 화면에 표시될 버튼의 인덱스
        
        for (int i = 0; i < this.choicearray.GetChoiceCount(); i++)
        {
            Choice currentChoice = this.choicearray.GetChoice(i);

            //if (currentChoice.IsConditionSatisfied())
            {
                if (visibleButtonIndex < choiceui.GetButtonCount()) // GetButtonCount()는 ChoiceUI에 추가 필요
                {
                    //choiceui.SetButtonData(visibleButtonIndex, dialTextLoader.GetChoiceData(currentChoice.GetChoiceID()), currentChoice.NextEvent());

                    //-- 로컬라이즈 적용
                    choiceui.SetButtonData(
                        visibleButtonIndex,
                        LocalizationSettings.StringDatabase.GetLocalizedString($"{currentChoice.GetLocalTableName()}",$"{currentChoice.GetChoiceID()}",
                            LocalizationSettings.SelectedLocale), 
                        currentChoice.NextEvent());
                    //--
                    visibleButtonIndex++;
                }
                else
                {
                    Debug.LogWarning($"ChoiceManager: 표시할 수 있는 버튼의 최대 개수({choiceui.GetButtonCount()})를 초과했습니다. 선택지 '{currentChoice.GetChoiceID()}'는 표시되지 않습니다.");
                }
            }
        }
        //실제로 사용된 버튼이 하나라도 있을 때만 UI 활성화
        if (visibleButtonIndex > 0)
        {
            if (choiceUIObj != null) 
                choiceUIObj.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ChoiceManager: 조건을 만족하는 선택지가 하나도 없어 선택지 UI를 표시하지 않습니다.");
            if (choiceUIObj != null) choiceUIObj.SetActive(false); // 표시할 선택지가 없으면 UI 끔
        }
    }

    // 선택지 UI를 닫고 버튼 상태를 초기화하는 공용 메소드
    public void HideAndResetChoiceUI()
    {
        if (choiceUIObj != null)
        {
            choiceUIObj.SetActive(false);
        }
        if (choiceui != null)
        {
            choiceui.ResetButton(); // 버튼 상태도 초기화
        }

        inputManager.OnSpaceTarget = null;
        inputManager.OnUpArrowTarget = null;
        inputManager.OnDownArrowTarget = null;
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

    public void SelectChoice(ChoiceButtonUI choiceButton)   // 선택지 버튼 이벤트용
    {
        ChoiceButtonUI choice = choiceButton;
        if (choice.GetIndex() == indicatorIndex)
        {
            HideAndResetChoiceUI();
            choice.GetEvent().Invoke();
        }
        else
        {
            indicatorIndex = choice.GetIndex();
            selectIndicator.transform.position = choiceui.GetButton(indicatorIndex).transform.position;
        }
    }

    public void SelectChoice()
    {
        Choice choice = choicearray.GetChoice(indicatorIndex);
        if (choice != null) 
        {
            choice.InvokeNextEvent(); 
        }
    }
    IEnumerator SelectedCoroutine()
    {
        yield return new WaitForSeconds(1f);

        HideAndResetChoiceUI();
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SelectChoice();
        }
    }

    public void OnUpArrow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (indicatorIndex <= 0)
                return;

            indicatorIndex--;
            selectIndicator.transform.position = choiceui.GetButton(indicatorIndex).transform.position;
        }
    }

    public void OnDownArrow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (indicatorIndex >= choicearray.GetChoiceCount() - 1)
                return;

            indicatorIndex++;
            selectIndicator.transform.position = choiceui.GetButton(indicatorIndex).transform.position;
        }
    }
}
