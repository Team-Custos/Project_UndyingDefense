using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UltEvents;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private ChoiceButtonUI[] buttonUIArray;
    [SerializeField] private Image selectIndicator;

    private void Start()
    {
       // selectIndicator.transform.position = buttonUIArray[0].transform.position;
    }

    public void SetButtonData(int j, string choiceText, UltEvent choiceEvent)
    {
        buttonUIArray[j].SetButton(choiceText, choiceEvent);
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
}
