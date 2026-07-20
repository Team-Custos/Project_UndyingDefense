using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProclamationDialogueManager : DialogueManager
{
    //[SerializeField] protected IntroStatementUI introStatementUI;
    public override void ShowDialogue(SpeakingArray speakingArray)
    {
        inputManager.OnSpaceTarget = this;  // 대화를 보여줄 때 타겟가져오기
        currentSpeakingArray = speakingArray;
        //ShowDialogue();
        // Statement ui로 변경하기-> 애니메이션에 플래그 추가(메서드 생성해야함)
        //dialogueui.onFadeInComplete = () => ShowDialogue(); // 대화 UI 페이드인 끝나고 대사 시작
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
