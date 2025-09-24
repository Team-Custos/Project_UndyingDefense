using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    //[SerializeField] protected DialogueData dialogueData;
    //[SerializeField] protected DialogueData ingamedialogueData;
    //[SerializeField] private IntroScene introScene;

    //[SerializeField] protected TextMeshProUGUI dialogueText;
    //[SerializeField] protected Button nextBtn;

    //[SerializeField] private Button acceptBtn;
    //[SerializeField] private Button refuseBtn;

    //protected int currentLineIndex = 0;

    //------------------------------------------------------
    [SerializeField] protected DialTextTableLoader tableLoader;
    [SerializeField] protected DialogueUI dialogueui; // DialogueUI.cs 레퍼런스
    [SerializeField] protected TextMeshProUGUI dialogueLine;
    [SerializeField] protected Button nextBtn;
    //[SerializeField] private SpeakingArray speakingArray;

    private int currentLineIndex = 0;  // 현재 내가 출력해야할 대사의 줄 번호
    private int currentSpeakingIndex = 0;  // 현재 내가 출력해야할 speaking의 번호
    private Speaking currentSpeaking;
    private SpeakingArray currentSpeakingArray;
    private List<string> lines;
    //private CharacterData currentCharData; // 현재 진행중인 Speaking의 캐릭터 데이터 // -> 지역변수로 만들기


    protected virtual void Start()
    {
        //StartDialogue(dialogueData);
        //ShowDialogue(speakingArray);
    }

    protected void StartDialogue(DialogueData dialogueData)
    {
        //this.dialogueData = dialogueData;
        //currentLineIndex = 0;
        //ShowDialogueLine();
    }

    public void OnNextButtonClicked()
    {
        //currentLineIndex++;

        //if (currentLineIndex < dialogueData.Lines.Length - 1)
        //{
        //    ShowDialogueLine();
        //}
        //else if (currentLineIndex == dialogueData.Lines.Length - 1)
        //{
        //    ShowDialogueLine();
        //    EndDialogue();
        //}
        //----------------
        ReadLine();
    }

    //protected void ShowDialogueLine()
    //{
    //    var line = dialogueData.Lines[currentLineIndex];
    //    dialogueText.text = line.Text;
    //}
    //-------------------------------------------------------------------------------
    // 선택지 이벤트에 넣어줄 함수
    public void ShowDialogue(SpeakingArray speakingArray)
    {
        currentSpeakingArray = speakingArray;
        dialogueui.gameObject.SetActive(true);
        ShowDialogue();
    }

    public void ShowDialogue()
    {
        currentSpeaking = currentSpeakingArray.GetSpeaking(currentSpeakingIndex);
        CharacterData currentCharData = currentSpeaking.GetCharacterData();   // 지역변수로 만들기
        lines = tableLoader.GetTextData(currentSpeaking.GetSpeakingID());
        dialogueui.SetDialogueCharacter(currentCharData.characterSprite, currentCharData.characterName);

        dialogueLine.text = lines[currentLineIndex];
        if(lines.Count > 1)
            nextBtn.gameObject.SetActive(true);

    }
    public void ReadLine()  // 다른곳에서 불러올 이벤트로
    {
        if ( currentLineIndex < lines.Count - 1)
        {
            currentLineIndex++;
            dialogueLine.text = lines[currentLineIndex];
            return;
        }

        if (currentLineIndex >= lines.Count - 1)
        {
            currentSpeakingIndex++;
            currentLineIndex = 0;
            // 배열이 끝나면 UI 비활성화 > j 사용
            if (currentSpeakingIndex >= currentSpeakingArray.GetArrayLength())      // firstSpeakingArray
            {
                currentSpeakingIndex = 0;
                EndSpeaking();
                return;
            }
            ShowDialogue();
        }
    }

    //protected virtual void EndDialogue()
    public void EndSpeaking()
    {
        nextBtn.gameObject.SetActive(false);
        currentSpeakingArray.InvokeNextEvent();
    }

    public void EndDialogue()   // 이벤트
    {
        dialogueui.gameObject.SetActive(false);
    }

    //public void PlayDeclaration()   // 수락
    //{
    //    introScene.PlayDeclarationDropAnimation();
    //}

    public void RetrunTitle()   // 거절
    {
        LoadingSceneManager.LoadScene("TitleScene_LoPol");
    }
}
