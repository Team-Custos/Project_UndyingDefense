using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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


    private ScriptableObject[] so;


    private void Start()
    {
       SoundManager.Instance.PlayBGM(lobbyBgm);
       LoadCommandSkillData();

        so = Resources.LoadAll<ScriptableObject>("Data/UnitData");

        DialogueEventInvoke();

    }
    public void BeforeTutorial() // 훈련장으로 안내하기 위한 
    {
        alarm.gameObject.SetActive(true);
        stageStartBtn.SetActive(false);
    }
    public void DialogueEventInvoke()
    {
        if (!UserDataModel.instance.IsTutorialEnd && !UserDataModel.instance.IsGameFinished
            && !UserDataModel.instance.IsGameWin && !UserDataModel.instance.FirstMainDialogue)
        {
            pInputManager.OnSpaceTarget = dialogueManager;
            UserDataModel.instance.SetFirstMainDialogue(true);
            beforeTuorial.Invoke();
            //alarm.gameObject.SetActive(true);
            //stageStartBtn.SetActive(false);
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
        else
            return;
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
        UserDataModel.instance.SetTutorialEnd(true);
    }

    public void LoadInGameScene()
    {
        SoundManager.Instance.PlaySFX(battleStartSfx);
        LoadingSceneManager.LoadScene("Stage1_MergeScene  25.0608");
        UserDataModel.instance.SetGameFinished(true);
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
