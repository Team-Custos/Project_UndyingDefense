using DG.Tweening;
using InputEventInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltEvents;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static StagePrefsData;

public class LobbyManager : MonoBehaviour, IInputOnSpace
{
    [SerializeField] private AudioClip lobbyBgm;
    [SerializeField] private AudioClip battleStartSfx;
    [SerializeField] private AudioClip endGameSfx;

    private CommandSkillData[] commanderSkils;
    [SerializeField] private CommandSkillData[] commandSkillDatas;

    [SerializeField] private GameObject rosterPanel;
    [SerializeField] private float endDelay = 0.5f;

    [SerializeField] private GameObject stageStartBtn;
    [SerializeField] private Image alarm;

    [Header("Class")]
    [SerializeField] private MessageUI messageUI;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private PlayerInputEventManager pInputManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private AccountInfo accountInfoPanel;
    [SerializeField] private PortraitSelectManager portraitSelectManager;

    [Header("DialogueEvent")]
    [SerializeField] private UltEvent isTutorialEnd;
    [SerializeField]private UltEvent beforeTuorial;
    [SerializeField] private UltEvent winTutorial;
    [SerializeField] private UltEvent afterUiryeong;
    [SerializeField] private UltEvent winUiryeong;
    [SerializeField] private UltEvent winGumsan;
    [SerializeField] private UltEvent winHaengju;
    [SerializeField] private UltEvent winNamwon;

    [Header("초상화")]
    [SerializeField] private Image portraitLine;
    [SerializeField] private Image portrait;

    [Header("닉네임")]
    [SerializeField] private TextMeshProUGUI nicknameTextUI;

    [Header("공로포인트")]
    [SerializeField] private TextMeshProUGUI pointTextUI;
    [SerializeField] private RankSystem rankSystem;

    [Header("StagePrefsData")]
    [SerializeField] private StagePrefsData stagePrefsData;

    [Header("Stage Buttons")]
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Button guemsanBtn;
    [SerializeField] private Button namhanBtn;
    [SerializeField] private Button namwonBtn;
    [SerializeField] private Button uiryeongBtn;    // 의령 전장 테스트용 버튼 (261103 LoPol)

    [Header("tutorial")]
    //[SerializeField] private GameObject tutorialLock;
    [SerializeField] private GameObject tutorialRecordPanel;
    [SerializeField] private TextMeshProUGUI tutorialRecordText;
    [Header("Uiryeong")]
    [SerializeField] private GameObject uiryeongLock;
    [SerializeField] private GameObject uiryeongCloud;
    [SerializeField] private GameObject uiryeongRecordPanel;
    [SerializeField] private TextMeshProUGUI uiryeongRecordText;
    [Header("Guemsan")]
    [SerializeField] private GameObject guemsanLock;
    [SerializeField] private GameObject guemsanCloud;
    [SerializeField] private GameObject guemsanRecordPanel;
    [SerializeField] private TextMeshProUGUI guemsanRecordText;
    [Header("Namhan")]
    [SerializeField] private GameObject namhanLock;
    [SerializeField] private GameObject namhanCloud;
    [SerializeField] private GameObject namhanRecordPanel;
    [SerializeField] private TextMeshProUGUI namhanRecordText;
    [Header("Namwon")]
    [SerializeField] private GameObject namwonLock;
    [SerializeField] private GameObject namwonCloud;
    [SerializeField] private GameObject namwonRecordPanel;
    [SerializeField] private TextMeshProUGUI namwonRecordText;
    [Header("StagePrefsData")]
    [SerializeField] private SpeakingArray winTutorialDialogue;
    [SerializeField] private SpeakingArray winUiryeongDialogue;
    [SerializeField] private SpeakingArray afterWinGusan;

    // -- 씬 로딩 중복 방지용 플래그 -- 260718
    [Header("Screen Blocker")]
    [SerializeField] private GameObject inputBlocker; // Canvas 최하단, 풀스크린 Image, Raycast Target ON
    private bool isLoadingScene = false;

    private ScriptableObject[] so;


    private void Start()
    {
        SoundManager.Instance.PlayBGM(lobbyBgm);
       LoadSavedPortrait();

        so = Resources.LoadAll<ScriptableObject>("Data/UnitData");

        DialogueEventInvoke();

        CheckStage();
        nicknameTextUI.text = PlayerPrefs.GetString("PlayerName");
        pointTextUI.text = PlayerPrefs.GetFloat("Point").ToString();
        //PlayerPrefs.SetInt("IsGeumsanFinished", 0);
        rankSystem.UpdateRank();
    }
    public void CheckStage()
    {
        StageData tutorial = stagePrefsData.GetStageData("UNQ_tutorial");
        StageData uiryeong = stagePrefsData.GetStageData("UNQ_uiryeong");   // 의령 추가
        StageData guemsan = stagePrefsData.GetStageData("UNQ_gumsanCastle");
        StageData namhan = stagePrefsData.GetStageData(("UNQ_namhanFortress"));
        StageData namwon = stagePrefsData.GetStageData(("UNQ_namwonCastle"));
        Debug.Log($"금산전투 해금여부 : {guemsan.isOpen}, 남한산성 해금여부 : {namhan.isOpen}");
        Debug.Log($"금산전투 Win여부 : {PlayerPrefs.GetInt("GeumsanWin")}");
        Debug.Log($"금산전투 클리어여부 : {guemsan.isStageEnd}, 남한산성 클리어여부 : {namhan.isStageEnd}");

        if(tutorial.clearTime != 0)
        {
            tutorialRecordText.text = ConvertToTime(tutorial.clearTime);
            tutorialRecordPanel.SetActive(true);
        }

        if(uiryeong.isOpen)
        {
            uiryeongBtn.enabled = true;
            uiryeongLock.SetActive(false);
            uiryeongCloud.SetActive(false);
            if (uiryeong.clearTime != 0)
            {
                //uiryeongRecordText.text = $"{uiryeong.clearTime}";
                uiryeongRecordText.text = ConvertToTime(uiryeong.clearTime);
                uiryeongRecordPanel.SetActive(true);
            }
        }

        if (guemsan.isOpen)
        {
            guemsanBtn.enabled = true;
            guemsanLock.SetActive(false);
            guemsanCloud.SetActive(false);

            if(guemsan.clearTime != 0)
            {
                //guemsanRecordText.text = $"{guemsan.clearTime}";
                guemsanRecordText.text = ConvertToTime(guemsan.clearTime);
                guemsanRecordPanel.SetActive(true);
            }
        }
        if(namhan.isOpen)
        {
            namhanBtn.enabled = true;
            namhanLock.SetActive(false);
            namhanCloud.SetActive(false);

            if (namhan.clearTime != 0)
            {
                //guemsanRecordText.text = $"{namhan.clearTime}";
                namhanRecordText.text = ConvertToTime(namhan.clearTime);
                namhanRecordPanel.SetActive(true);
            }
        }
        if (namwon.isOpen)
        {
            namwonBtn.enabled = true;
            namwonLock.SetActive(false);
            namwonCloud.SetActive(false);

            if (namwon.clearTime != 0)
            {
                //namwonRecordText.text = $"{namwon.clearTime}";
                namwonRecordText.text = ConvertToTime(namwon.clearTime);
                namwonRecordPanel.SetActive(true);
            }
        }
    }

    // 클리어 타임을 시각용으로 변환
    private string ConvertToTime(float timeRecord)
    {
        int minutes = Mathf.FloorToInt(timeRecord / 60f);
        int seconds = Mathf.FloorToInt(timeRecord % 60f);
        int milliseconds = Mathf.FloorToInt((timeRecord % 1f) * 100f);

        string recordText = $"{minutes:00} : {seconds:00} : {milliseconds:00}\"";

        return recordText ;
    }

    public void BeforeTutorial() // 훈련장으로 안내하기 위한 
    {
        alarm.gameObject.SetActive(true);
        stageStartBtn.SetActive(false);
    }
    public void DialogueEventInvoke()
    {
        Debug.Log($"훈련장 {PlayerPrefs.GetInt("IsTutorialEnd")}, " +
            $"훈련장 승패 {PlayerPrefs.GetInt("TutorialWin")}, " +
            $"의령전투 {PlayerPrefs.GetInt("IsUiryeongFinished")}, " +
            $"의령 승패 {PlayerPrefs.GetInt("UiryeongWin")}, " +
            $"금산전투 {PlayerPrefs.GetInt("IsGeumsanFinished")}, " +
            $"금산 승패 {PlayerPrefs.GetInt("GeumsanWin")}," +
            $"행주 승패 {PlayerPrefs.GetInt("HaengjuWin")}," +
            $"남원 승패 {PlayerPrefs.GetInt("NamwonWin")}," +
            $"메인대화_01 {PlayerPrefs.GetInt("FirstMainDialogue")}");

        // Main01 훈련장 진입 전
        //if (PlayerPrefs.GetInt("IsTutorialEnd") == 0 && PlayerPrefs.GetInt("IsGeumsanFinished") == 0
        //    && PlayerPrefs.GetInt("GeumsanWin") == 0 && PlayerPrefs.GetInt("FirstMainDialogue") == 0)
        if (PlayerPrefs.GetInt("IsTutorialEnd") == 0 && PlayerPrefs.GetInt("TutorialWin") == 0
            && PlayerPrefs.GetInt("IsUiryeongFinished") ==0 && PlayerPrefs.GetInt("FirstMainDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("FirstMainDialogue", 1);
            beforeTuorial.Invoke();
        }
        //Main02 훈련장 종료, 훈련장 승리 전
        else if (PlayerPrefs.GetInt("IsTutorialEnd") == 1
            && PlayerPrefs.GetInt("IsUiryeongFinished") == 0 && PlayerPrefs.GetInt("TutorialDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("TutorialDialogue", 1);
            isTutorialEnd.Invoke();
            //dialogueManager.ShowDialogue(tutorialEndDialogue);
        }
        //Main03 훈련장 승리 후, 의령 전장 진입 전
        else if (PlayerPrefs.GetInt("IsTutorialEnd") == 1 && PlayerPrefs.GetInt("TutorialWin") == 1
            &&  PlayerPrefs.GetInt("TutoWinDialogue") == 0 && PlayerPrefs.GetInt("TutorialDialogue") == 1)   //PlayerPrefs.GetInt("GeumsanWin") == 0 &&
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("TutoWinDialogue", 1);
            winTutorial.Invoke();

        }
        //Main04 의령 종료
        else if (PlayerPrefs.GetInt("IsTutorialEnd") == 1 && PlayerPrefs.GetInt("TutorialWin") == 1
            && PlayerPrefs.GetInt("IsUiryeongFinished") == 1 &&  PlayerPrefs.GetInt("TutoWinDialogue") == 1 && PlayerPrefs.GetInt("UiryeongDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("UiryeongDialogue", 1);
            afterUiryeong.Invoke();
        }
        //Main05 의령 승리
        else if (PlayerPrefs.GetInt("IsUiryeongFinished") == 1 && PlayerPrefs.GetInt("UiryeongWin") == 1
            && PlayerPrefs.GetInt("UiryeongDialogue") == 1 && PlayerPrefs.GetInt("UiryeongWinDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("UiryeongWinDialogue", 1);
            winUiryeong.Invoke();
        }
        //Main06 금산 승리
        else if (PlayerPrefs.GetInt("IsGeumsanFinished") == 1 && PlayerPrefs.GetInt("GeumsanWin") == 1 
            && PlayerPrefs.GetInt("GumsanWinDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("GumsanWinDialogue", 1);
            winGumsan.Invoke();
        }
        //Main07 행주 승리
        else if (PlayerPrefs.GetInt("HaengjuWin") == 1 && PlayerPrefs.GetInt("HaengjuWinDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("HaengjuWinDialogue", 1);
            winHaengju.Invoke();
        }
        //Main08 남원 승리
        else if (PlayerPrefs.GetInt("NamwonWin") == 1 && PlayerPrefs.GetInt("NamwonWinDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("NamwonWinDialogue", 1);
            winNamwon.Invoke();
        }
        else 
            return;
    }

    public void ShowWinGumsamDialogue() //  금산 한 번에 승리했을 때, 금산End 대화 이후 이벤트용
    {
        if(PlayerPrefs.GetInt("GeumsanWin") == 1 && PlayerPrefs.GetInt("AfterGameDialogue") == 1)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("AfterGameWinDialogue", 1);
            dialogueManager.ShowDialogue(afterWinGusan);
            return;
        }
        dialogueManager.EndDialogue();

    }

    public void ShowWinTutorialDialogue()
    {
        if (PlayerPrefs.GetInt("TutorialWin") == 1 && PlayerPrefs.GetInt("TutorialDialogue") == 1)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("TutoWinDialogue", 1);
            dialogueManager.ShowDialogue(winTutorialDialogue);
            return;
        }
        dialogueManager.EndDialogue();
    }

    public void ShowWinUiryeongDialogue()
    {
        if (PlayerPrefs.GetInt("UiryeongWin") == 1 && PlayerPrefs.GetInt("UiryeongDialogue") == 1)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("UiryeongWinDialogue", 1);
            dialogueManager.ShowDialogue(winUiryeongDialogue);
            return;
        }
        dialogueManager.EndDialogue();
    }

    public void EndGame()
    {
        SoundManager.Instance.PlaySFX(endGameSfx);

        Invoke(nameof(QuitGame), endDelay);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void SaveCommandSkillData()
    {
        if (commanderSkils.Length < 3)
            return;

        string[] names = new string[commanderSkils.Length];
        for (int i = 0; i < commanderSkils.Length; i++)
        {
            names[i] = commanderSkils[i].Name;
        }

        string commanderSkillName = string.Join(",", names);

        PlayerPrefs.SetString("지휘관 스킬", commanderSkillName);
        PlayerPrefs.Save();

        Debug.Log($"지휘관 스킬 저장됨: {commanderSkillName}");

    }

    public void LoadSavedPortrait()
    {
        portraitSelectManager.LoadPortraitData();
        portraitSelectManager.LoadSavedPortrait();

    }

    public void OpenRosterPanel()
    {
        if (rosterPanel != null)
        {
            rosterPanel.SetActive(true);
        }
    }

    // -- 씬 로딩 메서드 -- 260718
    private void LoadSceneWithSfx(string sceneName, Button clickedButton)
    {
        if (isLoadingScene) return;
        isLoadingScene = true;

        inputBlocker.SetActive(true);

        clickedButton.enabled = false; // pointer 이벤트 처리 중단 → Pressed 상태 유지
        //clickedButton.GetComponent<Animator>()?.Play("Pressed");
        var anim = clickedButton.GetComponent<Animator>();
        Debug.Log(anim == null ? "Animator 못 찾음" : "Animator 찾음");
        if (anim != null)
        {
            anim.ResetTrigger("Normal");
            anim.ResetTrigger("Highlighted");
            anim.ResetTrigger("Selected");
            anim.ResetTrigger("Disabled");
            anim.Play("Pressed", 0, 0f);
        }

        SoundManager.Instance.PlaySFX(battleStartSfx);
        DOTween.Sequence()
            .AppendInterval(battleStartSfx.length)
            .OnComplete(() =>
            {
                LoadingSceneManager.LoadScene(sceneName);
            });
    }

    public void LoadTutorialScene() => LoadSceneWithSfx("TutorialScene", tutorialBtn);
    public void LoadInGameScene() => LoadSceneWithSfx("Stage1_MergeScene  25.0608", guemsanBtn);
    public void LoadNamhanGameScene() => LoadSceneWithSfx("Stage2_MergeScene LevelDesign", namhanBtn);
    public void LoadNamwonGameScene() => LoadSceneWithSfx("Stage3_MergeScene LevelDesign", namwonBtn);
    public void LoadUiryeongGameSceme() => LoadSceneWithSfx("Stage0_MergeScene LevelDesign", uiryeongBtn);  // 의령 전장 진입용 함수 (261103 LoPol)

    // 260718 이전 전장 열기 코드, 주석 처리
    /*
    public void LoadTutorialScene()     // 훈련장 버튼 메서드
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        DOTween.Sequence()
            .AppendInterval(battleStartSfx.length)
            .OnComplete(() =>
            {
                LoadingSceneManager.LoadScene("TutorialScene");
            });
        //LoadingSceneManager.LoadScene("TutorialScene");
        //UserDataModel.instance.SetTutorialEnd(true);
        //PlayerPrefs.SetInt("IsTutorialEnd", 1);
    }

    public void LoadInGameScene()   // 금산성 버튼 메서드
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        DOTween.Sequence()
            .AppendInterval(battleStartSfx.length)
            .OnComplete(() =>
            {
                LoadingSceneManager.LoadScene("Stage1_MergeScene  25.0608");
            });
        //LoadingSceneManager.LoadScene("Stage1_MergeScene  25.0608");
        //UserDataModel.instance.SetGameFinished(true);
        //PlayerPrefs.SetInt("IsGeumsanFinished", 1);
    }

    public void LoadNamhanGameScene()   // 남한산성 버튼 메서드
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        DOTween.Sequence()
            .AppendInterval(battleStartSfx.length)
            .OnComplete(() =>
            {
                LoadingSceneManager.LoadScene("Stage2_MergeScene LevelDesign");
            });
        //LoadingSceneManager.LoadScene("Stage2_MergeScene LevelDesign");
    }

    public void LoadNamwonGameScene()   // 남원성 버튼 메서드
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        DOTween.Sequence()
            .AppendInterval(battleStartSfx.length)
            .OnComplete(() =>
            {
                LoadingSceneManager.LoadScene("Stage3_MergeScene LevelDesign");
            });
       //LoadingSceneManager.LoadScene("Stage3_MergeScene LevelDesign");
    }
    */

    public void OnSpace(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void SetLobbyNickName(string text)
    {
        nicknameTextUI.text = text;
    }

    public void SetLobbyPortrait(Sprite portrait)
    {
        portraitLine.sprite = portrait;
    }

    public void OnClickAccountBtn()
    {
        // 계정 정보 패널 열기
        accountInfoPanel.ShowAccountPanel(portraitLine.sprite);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
    }
}
