using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speaker;
    [SerializeField] private Image characterImage;

    public void SetDialogueCharacter(Sprite characterSprite, string characterName)
    {
        characterImage.sprite = characterSprite;
        speaker.text = characterName;
    }
}
