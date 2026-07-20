using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speaker;
    [SerializeField] private Image characterImage;

    public System.Action onFadeInComplete;

    // 애니메이션 이벤트가 호출할 함수
    public void OnFadeInAnimationComplete()
    {
        Debug.Log("[OnFadeInAnimationComplete] 호출됨");
        onFadeInComplete?.Invoke();
        onFadeInComplete = null; // 한 번 쓰고 정리 (원치 않는 중복 호출 방지)
    }

    public void SetDialogueCharacter(Sprite characterSprite, string characterName)
    {
        characterImage.sprite = characterSprite;
        speaker.text = characterName;
    }
}
