using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private AudioClip lobbyBgm;

    private CommandSkillData[] commanderSkils;
    [SerializeField] private CommandSkillData[] commandSkillDatas;

    [SerializeField] private GameObject rosterPanel;


    private void Start()
    {
       SoundManager.Instance.PlayBGM(lobbyBgm);
       LoadCommandSkillData();

        //for(int i = 0; i < commanderSkils.Length; i++)
        //{
        //    commanderSkils[i] = commandSkillDatas[i];
        //}
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
        LoadingSceneManager.LoadScene("TutorialScene");
        UserDataModel.instance.SetTutorialEnd(true);
    }
}
