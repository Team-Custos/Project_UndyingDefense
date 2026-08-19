using DG.Tweening;
using InputEventInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UltEvents;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static DialogueData;

public class DialogueManager : MonoBehaviour, IInputOnSpace
{
    //------------------------------------------------------
    [Header("PlayerInputManager")]
    [SerializeField] protected PlayerInputEventManager inputManager;
    //[SerializeField] protected DialTextTableLoader tableLoader;
    [SerializeField] protected DialogueUI dialogueui; // DialogueUI.cs 레퍼런스
    [SerializeField] protected TextMeshProUGUI dialogueLine;
    [SerializeField] protected Button nextBtn;
    //[SerializeField] private GameObject spaceText;    // 이미지로 변경
    //[SerializeField] private Animator dialSpaceText;
    [SerializeField] private Image spaceBarImage;
    [SerializeField] protected float fadeDuration = 1f;
    [SerializeField] protected float nextEventDelay = 1.5f; // 다음 이벤트로 넘어가기 전에 대기할 시간

    protected int currentLineIndex = 0;  // 현재 내가 출력해야할 대사의 줄 번호
    protected int currentSpeakingIndex = 0;  // 현재 내가 출력해야할 speaking의 번호
    protected Speaking currentSpeaking;
    protected SpeakingArray currentSpeakingArray;
    protected List<string> lines = new List<string>();
    //private CharacterData currentCharData; // 현재 진행중인 Speaking의 캐릭터 데이터 // -> 지역변수로 만들기

    //-- 로컬라이즈 대화 저장용
    private Dictionary<string, List<string>> dialogueDictionary = new Dictionary<string, List<string>>();

    [Header("Typewriter")]
    [SerializeField] protected float typeSpeed = 0.03f; // 글자당 딜레이
    protected bool isTyping = false;
    protected Coroutine typingCoroutine;
    protected System.Action onTypingComplete; // 타이핑 완료 시 실행할 콜백


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
    public virtual void ShowDialogue(SpeakingArray speakingArray)
    {
        inputManager.OnSpaceTarget = this;  // 대화를 보여줄 때 타겟가져오기
        currentSpeakingArray = speakingArray;

        SetupCurrentSpeaking(); // 캐릭터 데이터 + 대사 목록을 미리 준비

        bool alreadyActive = dialogueui.gameObject.activeSelf;
        if (alreadyActive) {
            ShowDialogue(); // 이미 활성화되어 있다면 바로 대사 시작
            return;
        }
        dialogueui.gameObject.SetActive(true);
        //dialogueui.onFadeInComplete = () => ShowDialogue(); // 대화 UI 페이드인 끝나고 대사 시작
        dialogueui.onFadeInComplete = ShowDialogue; // 대화 UI 페이드인 끝나고 대사 시작
    }

    // 기존 ShowDialogue()에서 캐릭터 세팅 + 대사 로드 부분만 분리
    protected void SetupCurrentSpeaking()
    {
        dialogueLine.text = "";
        currentSpeaking = currentSpeakingArray.GetSpeaking(currentSpeakingIndex);
        CharacterData currentCharData = currentSpeaking.GetCharacterData();

        lines = GetLocalDialogue(currentSpeaking.GetTableName(), currentSpeaking.GetSpeakingID());

        dialogueui.SetDialogueCharacter(currentCharData.characterSprite, currentCharData.characterName);
    }

    public virtual void ShowDialogue()
    {
        /*
        currentSpeaking = currentSpeakingArray.GetSpeaking(currentSpeakingIndex);
        CharacterData currentCharData = currentSpeaking.GetCharacterData();   // 지역변수로 만들기

        //lines = tableLoader.GetTextData(currentSpeaking.GetSpeakingID());

        //-- 로컬라이즈 수정
        lines = GetLocalDialogue( currentSpeaking.GetTableName(), currentSpeaking.GetSpeakingID());
        //--

        dialogueui.SetDialogueCharacter(currentCharData.characterSprite, currentCharData.characterName);
        */

        //-- 타이핑 연출 수정
        //dialogueLine.text = lines[currentLineIndex];
        /* //260808 콜백 추가 전 코드
        SetDialogueText(lines[currentLineIndex]);
        //--
        if (lines.Count > 1)
        {
            //nextBtn.gameObject.SetActive(true);
            //spaceText.SetActive(true);
            //dialSpaceText.SetTrigger("ShowSpaceText");
            ShowSpaceImage(3f);
        }
        else
            ReadLine();
        */

        if(lines.Count > 1)
        {
            ShowSpaceImage(3f);
        }

        ShowLine(0);
    }

    // 특정 줄(index)을 타이핑으로 보여주고, 마지막 줄이면 완료 시 자동으로 다음 이벤트로 진행
    protected void ShowLine(int index)
    {
        currentLineIndex = index;
        bool isLastLine = (currentLineIndex >= lines.Count - 1);

        SetDialogueText(lines[currentLineIndex]);

        if (isLastLine)
        {
            // 마지막 줄: 타이핑이 끝나는 순간 자동으로 다음으로 진행 (space 불필요)
            onTypingComplete = AdvanceSpeaking;
        }
        else
        {
            // 마지막 줄이 아님: 타이핑만 끝내고 space 입력 대기
            onTypingComplete = null;
        }
    }

    // 타이핑 연출을 위한 메서드 (코루틴)
    // 기존 ShowDialogue()에서 dialogueLine.text = lines[currentLineIndex]; 대신
    protected void SetDialogueText(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(text));
    }

    protected IEnumerator TypeLine(string text)
    {
        Debug.Log($"[TypeLine] 시작, text: {text}");
        isTyping = true;
        dialogueLine.text = "";

        foreach (char c in text)
        {
            dialogueLine.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
        Debug.Log("[TypeLine] 완료");

        onTypingComplete?.Invoke();
        onTypingComplete = null;
    }

    protected void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueLine.text = lines[currentLineIndex];
        isTyping = false;
        typingCoroutine = null;

        onTypingComplete?.Invoke();
        onTypingComplete = null;
    }

    //-------------------------------------------------------------------------

    public void ReadLine()  // 다른곳에서 불러올 이벤트로
    {
        if (currentLineIndex < lines.Count - 1)
        {
            if(currentLineIndex < lines.Count - 1)
            {
                ShowLine(currentLineIndex + 1);
            }
            //-- 260808-- ShowLine()로 통합
            //currentLineIndex++;
            //SetDialogueText(lines[currentLineIndex]);
            ////-- 타이핑 연출 수정
            ////dialogueLine.text = lines[currentLineIndex];

        }
        /*  // 260808-- AdvanceSpeaking()로 통합
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
        }*/
    }

    protected void AdvanceSpeaking()
    {
        currentSpeakingIndex++;
        currentLineIndex = 0;

        if (currentSpeakingIndex >= currentSpeakingArray.GetArrayLength())
        {
            currentSpeakingIndex = 0;
            inputManager.OnSpaceTarget = null;
            EndSpeaking();
            return;
        }

        SetupCurrentSpeaking();
        ShowDialogue();
    }

    public void EndSpeaking()
    {
        //nextBtn.gameObject.SetActive(false);
        //spaceText.SetActive(false);
        //HideSpaceImage();
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
        // 260720 -- 이전 코드
        //if (context.performed)
        //{
        //    ReadLine();
        //}

        if (!context.performed) return;
        Debug.Log($"[OnSpace] 호출됨, isTyping = {isTyping}");

        if (isTyping)
        {
            // 타이핑 중이면 스킵 -> 전체 텍스트 즉시 표시
            SkipTyping();
        }
        else
        {
            // 다 보여준 상태면 다음 대사로
            ReadLine();
        }
    }

    public void ShowSpaceImage(float delay = 0f)
    {
        spaceBarImage.DOKill(); // 이전에 실행중이던 트윈이 있다면 제거
        spaceBarImage.DOFade(1f, fadeDuration).SetDelay(delay);
    }

    public void HideSpaceImage()    // 기획 변경 space바 이미지 항상 띄우기로 인한 사용 안 함
    {
        spaceBarImage.DOKill();
        spaceBarImage.DOFade(0f, fadeDuration);
    }
}
