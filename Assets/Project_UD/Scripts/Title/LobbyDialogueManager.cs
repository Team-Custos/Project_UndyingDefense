using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyDialogueManager : DialogueManager
{
    [SerializeField] private DialogueData ingameDialogueData;
    [SerializeField] private SpeakingArray speakingArray;
    protected override void Start()
    {
        if(UserDataModel.instance.IsGameFinshed)
            this.gameObject.SetActive(false);
        ShowDialogue(speakingArray);

        if (UserDataModel.instance.IsTutorialEnd)
        {
            //StartDialogue(ingameDialogueData);
            ShowDialogue(speakingArray);
        }
        else
        {
            StartDialogue(dialogueData);
        }
    }

    protected override void EndDialogue()
    {
        this.gameObject.SetActive(false);
    }

    public void OnNextButtonClick()
    {
        currentLineIndex++;  // 인덱스 먼저 증가 (첫 대사가 이미 나왔으니까)

        if (currentLineIndex < dialogueData.Lines.Length)
        {
            ShowDialogueLine();
        }
        else
        {
            EndDialogue();
        }
    }
}
