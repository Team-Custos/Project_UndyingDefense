using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private AudioClip lobbyBgm;
    [SerializeField] private AudioClip battleStartSfx;
    [SerializeField] private AudioClip endGameSfx;

    private CommandSkillData[] commanderSkils;
    [SerializeField] private CommandSkillData[] commandSkillDatas;

    [SerializeField] private GameObject rosterPanel;
    [SerializeField] private float endDelay = 0.5f;

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private UltEvent isTutorialEnd;
    [SerializeField]private UltEvent beforeTuorial;


    private ScriptableObject[] so;


    private void Start()
    {
       SoundManager.Instance.PlayBGM(lobbyBgm);
       LoadCommandSkillData();

        so = Resources.LoadAll<ScriptableObject>("Data/UnitData");

        if (UserDataModel.instance.IsGameFinshed)
            dialogueUI.gameObject.SetActive(false);

        if (UserDataModel.instance.IsTutorialEnd)
        {
            isTutorialEnd.Invoke();
        }
        else
        {
            beforeTuorial.Invoke();
        }

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
}
