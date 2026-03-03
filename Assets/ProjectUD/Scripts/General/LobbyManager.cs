using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static StagePrefsData;
using TMPro;
using System;

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

    [SerializeField] private Button guemsanBtn;
    [SerializeField] private Button namhanBtn;
    [SerializeField] private GameObject guemsanLock;
    [SerializeField] private GameObject guemsanCloud;
    [SerializeField] private GameObject namhanLock;
    [SerializeField] private GameObject namhanCloud;

    [Header("StagePrefsData")]
    [SerializeField] private SpeakingArray afterWinGusan;



    private ScriptableObject[] so;


    private void Start()
    {
        SoundManager.Instance.PlayBGM(lobbyBgm);
       LoadCommandSkillData();

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
        StageData guemsan = stagePrefsData.GetStageData("UNQ_gumsan");
        StageData namhan = stagePrefsData.GetStageData(("UNQ_namhanFortress"));
        Debug.Log($"금산전투 해금여부 : {guemsan.isOpen}, 남한산성 해금여부 : {namhan.isOpen}");
        Debug.Log($"금산전투 Win여부 : {PlayerPrefs.GetInt("GeumsanWin")}");
        Debug.Log($"금산전투 클리어여부 : {guemsan.isStageEnd}, 남한산성 클리어여부 : {namhan.isStageEnd}");

        if (guemsan.isOpen)
        {
            guemsanBtn.enabled = true;
            guemsanLock.SetActive(false);
            guemsanCloud.SetActive(false);
        }
        if(namhan.isOpen)
        {
            namhanBtn.enabled = true;
            namhanLock.SetActive(false);
            namhanCloud.SetActive(false);
        }
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

    public void LoadCommandSkillData()
    {
        //string rawData = PlayerPrefs.GetString("지휘관 스킬");
        //string[] skills = rawData.Split(',');

        //for(int i = 0; i < skills.Length; i++)
        //{
        //    commanderSkils[i] = Resources.Load("SkillData/Command/" + skills[i]) as CommandSkillData;
        //}

    }

    public void OpenRosterPanel()
    {
        if (rosterPanel != null)
        {
            rosterPanel.SetActive(true);
        }
    }

    public void LoadTutorialScene()     // 훈련장 버튼 메서드
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        LoadingSceneManager.LoadScene("TutorialScene");
        //UserDataModel.instance.SetTutorialEnd(true);
        //PlayerPrefs.SetInt("IsTutorialEnd", 1);
    }

    public void LoadInGameScene()   // 금산성 버튼 메서드
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        LoadingSceneManager.LoadScene("Stage1_MergeScene  25.0608");
        //UserDataModel.instance.SetGameFinished(true);
        //PlayerPrefs.SetInt("IsGeumsanFinished", 1);
    }

    public void LoadNamhanGameScene()   // 남한산성 버튼 메서드
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        LoadingSceneManager.LoadScene("Stage2_MergeScene LevelDesign");
    }

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
    }
}
