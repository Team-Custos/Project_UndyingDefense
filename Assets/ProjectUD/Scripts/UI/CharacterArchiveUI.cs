using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterArchiveUI : MonoBehaviour
{
    [SerializeField] private FactionCharacterRepository fRepository;
    [SerializeField] private CharacterButtonUI[] characterBtnArray;
    private int index;

    public void OnButtonClick(int buttonIndex)  // 버튼 클릭 이벤트용 함수
    {
        index = buttonIndex;
    }

    public void SetButtonData(int i, Sprite image, string text)
    {
        characterBtnArray[i].SetButton(image, text);
        characterBtnArray[i].gameObject.SetActive(true);
    }

    public void ResetButton()
    {
        for (int i = 0; i < characterBtnArray.Length; i++)
        {
            characterBtnArray[i].gameObject.SetActive(false);
            characterBtnArray[i].ResetButton();
        }
    }
}
