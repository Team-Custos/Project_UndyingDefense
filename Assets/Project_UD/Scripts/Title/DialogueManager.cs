using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using static DialogueData;

public class DialogueManager : MonoBehaviour, IInputOnSpace
{
    //------------------------------------------------------
    [Header("PlayerInputManager")]
    [SerializeField] private PlayerInputEventManager inputManager;
    //[SerializeField] protected DialTextTableLoader tableLoader;
    [SerializeField] protected DialogueUI dialogueui; // DialogueUI.cs 레퍼런스
    [SerializeField] protected TextMeshProUGUI dialogueLine;
    [SerializeField] protected Button nextBtn;
    [SerializeField] private GameObject spaceText;
    [SerializeField] private Animator dialSpaceText;

    private int currentLineIndex = 0;  // 현재 내가 출력해야할 대사의 줄 번호
    private int currentSpeakingIndex = 0;  // 현재 내가 출력해야할 speaking의 번호
    private Speaking currentSpeaking;
    private SpeakingArray currentSpeakingArray;
    private List<string> lines = new List<string>();
    //private CharacterData currentCharData; // 현재 진행중인 Speaking의 캐릭터 데이터 // -> 지역변수로 만들기

    //-- 로컬라이즈 대화 저장용
    private Dictionary<string, List<string>> dialogueDictionary = new Dictionary<string, List<string>>();

    
    // 로컬라이즈 테이블에서 대화 단락 가져오기
    public List<string> GetLocalDialogue(string table, string id)
    {
        lines.Clear();
        int index = 1;
        string readline = LocalizationSettings.StringDatabase.GetLocalizedString($"{table}", $"{id}{index}",
                    LocalizationSettings.SelectedLocale);
        do
        {
                lines.Add(readline);
                index++;
                readline = LocalizationSettings.StringDatabase.GetLocalizedString($"{table}", $"{id}{index}",
                    LocalizationSettings.SelectedLocale);

        }while(readline != string.Empty);

        return lines;
    }

    //-------------------------------------------------------------------------------
    // 선택지 이벤트에 넣어줄 함수
    public void ShowDialogue(SpeakingArray speakingArray)
    {
        inputManager.OnSpaceTarget = this;  // 대화를 보여줄 때 타겟가져오기
        currentSpeakingArray = speakingArray;
        dialogueui.gameObject.SetActive(true);
        ShowDialogue();
    }

    public void ShowDialogue()
    {
        currentSpeaking = currentSpeakingArray.GetSpeaking(currentSpeakingIndex);
        CharacterData currentCharData = currentSpeaking.GetCharacterData();   // 지역변수로 만들기

        //lines = tableLoader.GetTextData(currentSpeaking.GetSpeakingID());

        //-- 로컬라이즈 수정
        lines = GetLocalDialogue( currentSpeaking.GetTableName(), currentSpeaking.GetSpeakingID());
        //--

        dialogueui.SetDialogueCharacter(currentCharData.characterSprite, currentCharData.characterName);
        
        dialogueLine.text = lines[currentLineIndex];
        if (lines.Count > 1)
        {
            //nextBtn.gameObject.SetActive(true);
            spaceText.SetActive(true);
            dialSpaceText.SetTrigger("ShowSpaceText");
        }
        else
            ReadLine();
    }

    public void ReadLine()  // 다른곳에서 불러올 이벤트로
    {
        if (currentLineIndex < lines.Count - 1)
        {
            currentLineIndex++;
            dialogueLine.text = lines[currentLineIndex];
            if (currentLineIndex == lines.Count - 1)
            {
                //nextBtn.gameObject.SetActive(false);
            }
            //return;
        }

        if (currentLineIndex >= lines.Count - 1)
        {
            currentSpeakingIndex++;
            currentLineIndex = 0;
            // 배열이 끝나면 UI 비활성화 > j 사용
            if (currentSpeakingIndex >= currentSpeakingArray.GetArrayLength())      // firstSpeakingArray
            {
                currentSpeakingIndex = 0;
                inputManager.OnSpaceTarget = null;
                EndSpeaking();
                return;
            }
            ShowDialogue();
        }
    }

    public void EndSpeaking()
    {
        //nextBtn.gameObject.SetActive(false);
        spaceText.SetActive(false);
        currentSpeakingArray.InvokeNextEvent();
    }

    public void EndDialogue()   // 이벤트
    {
        dialogueui.gameObject.SetActive(false);
    }

    public void RetrunTitle()   // 거절
    {
        LoadingSceneManager.LoadScene("TitleScene_LoPol");
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ReadLine();
        }
    }
}
