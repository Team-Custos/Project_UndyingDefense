using DG.Tweening;
using InputEventInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltEvents;
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
    [SerializeField] private UltEvent isTutorialEnd;
    [SerializeField]private UltEvent beforeTuorial;
    [SerializeField] private UltEvent isGameEnd;
    [SerializeField] private UltEvent isGameWin;
    [SerializeField] private PlayerInputEventManager pInputManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private AccountInfo accountInfoPanel;
    [SerializeField] private PortraitSelectManager portraitSelectManager;

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
        StageData guemsan = stagePrefsData.GetStageData("UNQ_gumsanCastle");
        StageData namhan = stagePrefsData.GetStageData(("UNQ_namhanFortress"));
        StageData namwon = stagePrefsData.GetStageData(("UNQ_namwonCastle"));
        Debug.Log($"금산전투 해금여부 : {guemsan.isOpen}, 남한산성 해금여부 : {namhan.isOpen}");
        Debug.Log($"금산전투 Win여부 : {PlayerPrefs.GetInt("GeumsanWin")}");
        Debug.Log($"금산전투 클리어여부 : {guemsan.isStageEnd}, 남한산성 클리어여부 : {namhan.isStageEnd}");

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
        Debug.Log($"훈련장 {PlayerPrefs.GetInt("IsTutorialEnd")}, 금산전투 {PlayerPrefs.GetInt("IsGeumsanFinished")}, 금산 승패 {PlayerPrefs.GetInt("GeumsanWin")}," +
            $"메인대화 {PlayerPrefs.GetInt("FirstMainDialogue")}");

        if(PlayerPrefs.GetInt("IsTutorialEnd") == 0 && PlayerPrefs.GetInt("IsGeumsanFinished") == 0
            && PlayerPrefs.GetInt("GeumsanWin")==0 && PlayerPrefs.GetInt("FirstMainDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("FirstMainDialogue", 1);
            beforeTuorial.Invoke();
        }

        else if(PlayerPrefs.GetInt("IsTutorialEnd") == 1 && PlayerPrefs.GetInt("IsGeumsanFinished") == 0
            && PlayerPrefs.GetInt("GeumsanWin") == 0 && PlayerPrefs.GetInt("AfterTutorialDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("AfterTutorialDialogue", 1);
            isTutorialEnd.Invoke();
            //dialogueManager.ShowDialogue(tutorialEndDialogue);
        }

        else if(PlayerPrefs.GetInt("IsTutorialEnd") == 1 && PlayerPrefs.GetInt("IsGeumsanFinished") == 1
            &&  PlayerPrefs.GetInt("AfterGameDialogue") == 0)   //PlayerPrefs.GetInt("GeumsanWin") == 0 &&
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("AfterGameDialogue",1);
            isGameEnd.Invoke();

        }

        else if(PlayerPrefs.GetInt("IsTutorialEnd") == 1 && PlayerPrefs.GetInt("IsGeumsanFinished") == 1
            && PlayerPrefs.GetInt("GeumsanWin") == 1 &&  PlayerPrefs.GetInt("AfterGameDialogue") == 1 && PlayerPrefs.GetInt("AfterGameWinDialogue") == 0)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            PlayerPrefs.SetInt("AfterGameWinDialogue", 1);
            isGameWin.Invoke();
        }

        else
            return;
    }

    public void ShowWinGumsamDialogue()
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
