using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProclamationDialogueManager : DialogueManager
{
    public override void ShowDialogue(SpeakingArray speakingArray)
    {
        inputManager.OnSpaceTarget = this;  // 대화를 보여줄 때 타겟가져오기
        currentSpeakingArray = speakingArray;
        ShowDialogue();
    }

    public override void ShowDialogue()
    {
        currentSpeaking = currentSpeakingArray.GetSpeaking(currentSpeakingIndex);

        lines = GetLocalDialogue(currentSpeaking.GetTableName(), currentSpeaking.GetSpeakingID());

        dialogueLine.text = lines[currentLineIndex];
        if (lines.Count > 1)
        {
            dialogueLine.DOFade(1F, fadeDuration);
            ShowSpaceImage(3f);
        }
        else
            ReadLine();
    }
}
