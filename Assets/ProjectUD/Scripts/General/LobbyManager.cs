using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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

    [SerializeField] private MessageUI messageUI;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private UltEvent isTutorialEnd;
    [SerializeField]private UltEvent beforeTuorial;
    [SerializeField] private UltEvent isGameEnd;
    [SerializeField] private UltEvent isGameWin;
    [SerializeField] private PlayerInputEventManager pInputManager;
    [SerializeField] private DialogueManager dialogueManager;

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
        //PlayerPrefs.SetInt("IsGeumsanFinished", 0);
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

            if(PlayerPrefs.GetInt("GeumsanWin") == 1 && PlayerPrefs.GetInt("AfterGameDialogue") == 1)
            {
                pInputManager.OnSpaceTarget = dialogueManager;
                PlayerPrefs.SetInt("AfterGameWinDialogue", 1);
                isGameWin.Invoke();
            }
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

        /*
        if (!UserDataModel.instance.IsTutorialEnd && !UserDataModel.instance.IsGameFinished
            && !UserDataModel.instance.IsGameWin && !UserDataModel.instance.FirstMainDialogue)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            UserDataModel.instance.SetFirstMainDialogue(true);
            beforeTuorial.Invoke();
        }

            else if (UserDataModel.instance.IsTutorialEnd && !UserDataModel.instance.IsGameFinished
                && !UserDataModel.instance.IsGameWin && !UserDataModel.instance.AfterTutorialDialogue)
            {
                pInputManager.OnSpaceTarget = dialogueManager;
                UserDataModel.instance.SetAfterTutorialDialogue(true);
                isTutorialEnd.Invoke();
            }
            else if (UserDataModel.instance.IsTutorialEnd && UserDataModel.instance.IsGameFinished
                && !UserDataModel.instance.IsGameWin && !UserDataModel.instance.AfterGameDialogue)
            {
                pInputManager.OnSpaceTarget = dialogueManager;
                UserDataModel.instance.SetAfterGameDialogue(true);
                isGameEnd.Invoke();
            }
        else if (UserDataModel.instance.IsTutorialEnd && UserDataModel.instance.IsGameFinished
            && UserDataModel.instance.IsGameWin && !UserDataModel.instance.AfterGameWinDialogue)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            UserDataModel.instance.SetAfterGameWinDialogue(true);
            isGameWin.Invoke();
        }
        */

    }

    public void ShowWinGumsamDialogue()
    {
        dialogueManager.ShowDialogue();
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

    public void LoadTutorialScene()
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        LoadingSceneManager.LoadScene("TutorialScene");
        //UserDataModel.instance.SetTutorialEnd(true);
        //PlayerPrefs.SetInt("IsTutorialEnd", 1);
    }

    public void LoadInGameScene()
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        LoadingSceneManager.LoadScene("Stage1_MergeScene  25.0608");
        //UserDataModel.instance.SetGameFinished(true);
        //PlayerPrefs.SetInt("IsGeumsanFinished", 1);
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
