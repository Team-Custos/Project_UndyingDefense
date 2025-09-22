using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] protected DialogueData dialogueData;
    [SerializeField] protected DialogueData ingamedialogueData;
    [SerializeField] private IntroScene introScene;

    [SerializeField] protected TextMeshProUGUI dialogueText;
    [SerializeField] protected Button nextBtn;

    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button refuseBtn;

    protected int currentLineIndex = 0;

    //------------------------------------------------------
    [SerializeField] private DialTextTableLoader tableLoader;
    [SerializeField] private DialogueUI dialogueui; // DialogueUI.cs 레퍼런스
    [SerializeField] private TextMeshProUGUI dialogueLine;
    //[SerializeField] private SpeakingArray speakingArray;

    private int i=0;  // 현재 내가 출력해야할 대사의 줄 번호
    private int j=0;  // 현재 내가 출력해야할 speaking의 번호
    private Speaking currentSpeaking;
    private SpeakingArray currentSpeakingArray;
    private List<string> lines;
    private CharacterData currentCharData; // 현재 진행중인 Speaking의 캐릭터 데이터


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

    protected void ShowDialogueLine()
    {
        var line = dialogueData.Lines[currentLineIndex];
        dialogueText.text = line.Text;
    }
    //-------------------------------------------------------------------------------
    // 선택지 이벤트에 넣어줄 함수
    public void ShowDialogue(SpeakingArray speakingArray)
    {
        currentSpeakingArray = speakingArray;
        //dialogueUI.SetActive(true);
        ShowDialogue();
    }

    public void ShowDialogue()
    {
        currentSpeaking = currentSpeakingArray.GetSpeaking(j);
        currentCharData = currentSpeaking.GetCharacterData();
        lines = tableLoader.GetTextData(currentSpeaking.GetSpeakingID());
        dialogueui.SetDialogueCharacter(currentCharData.characterSprite, currentCharData.characterName);

        dialogueLine.text = lines[i];

    }
    public void ReadLine()  // 다른곳에서 불러올 이벤트로
    {
        if ( i < lines.Count - 1)
        {
            i++;
            dialogueLine.text = lines[i];
            return;
        }

        if (i >= lines.Count - 1)
        {
            j++;
            i = 0;
            // 배열이 끝나면 UI 비활성화 > j 사용
            if (j >= currentSpeakingArray.GetArrayLength())      // firstSpeakingArray
            {
                j = 0;
                EndDialogue();
                return;
            }
            ShowDialogue();
        }
    }

    protected virtual void EndDialogue()
    {
        nextBtn.gameObject.SetActive(false);
        currentSpeakingArray.InvokeNextEvent();

        acceptBtn.gameObject.SetActive(true);
        refuseBtn.gameObject.SetActive(true);
    }

    


    public void PlayDeclaration()
    {
        introScene.PlayDeclarationDropAnimation();
    }

    public void RetrunTitle()
    {
        LoadingSceneManager.LoadScene("TitleScene_LoPol");
    }
}
