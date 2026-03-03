using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class RankSystem : MonoBehaviour
{
    [SerializeField] private LobbyManager lobbyManager;

    [SerializeField] private RankData[] rankDatas;
    [SerializeField] private TextMeshProUGUI commanderName;
    //[SerializeField] private MessageUI rewardAlarm;

    private int currentRank;
    private float currentPoints;
    private List<string> rewardAlarms = new List<string>();

    private void Start()
    {
        //UpdateRank();
    }

    public void UpdateRank()
    {
        string commanderID = PlayerPrefs.GetString("CommanderID");
        Debug.Log($"[RankSystem] 현재 지휘관 ID: {commanderID}");
        commanderName.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", $"{commanderID}", LocalizationSettings.SelectedLocale);

        currentPoints = PlayerPrefs.GetFloat("Point");
        currentRank = PlayerPrefs.GetInt("CommanderRank");

        foreach (var rankData in rankDatas)
        {
            // 초기 랭크인 경우 (랭크 1)
            //if(currentRank == 1 && rankData.rank == currentRank)
            //{
            //    lobbyManager.SetLobbyPortrait(rankData.rewardCommanderProfile);
            //    Debug.Log($"[RankSystem] 초기 랭크 설정: {rankData.rank}로 설정, 초상화 변경");
            //}

            if (currentPoints >= rankData.requirePoint && rankData.rank > currentRank)
            {
                RankUp(rankData);
            }

            else if (currentPoints >= rankData.requirePoint && rankData.rank == currentRank)
            {
                lobbyManager.SetLobbyPortrait(rankData.rewardCommanderProfile);
                Debug.Log($"[RankSystem] : {rankData.rank}로 설정, 초상화 변경");
            }
        }
    }

    private void RankUp(RankData rankData)
    {
        currentRank = rankData.rank;
        PlayerPrefs.SetInt("CommanderRank", currentRank);
        PlayerPrefs.SetString("CommanderID", rankData.commanderID);
        commanderName.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", $"{rankData.commanderID}", LocalizationSettings.SelectedLocale);

        // 로비 초상화테두리 변경
        lobbyManager.SetLobbyPortrait(rankData.rewardCommanderProfile);

        // 일단 지휘관 스킬만 해금
        for (int i = 0; i < rankData.rewardCommandSkillID.Count; i++)
        {
            PlayerPrefsData.instance.SetHaveCommanderSkills(rankData.rewardCommandSkillID[i]);

            string skillName = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{rankData.rewardCommandSkillID[i]}_name", LocalizationSettings.SelectedLocale);

            rewardAlarms.Add(skillName);
            Debug.Log($"[RankSystem] {skillName} 보상 알림 추가");
        }
    }

    public IReadOnlyList<string> GetRewardAlarms()
    {
        return rewardAlarms;
    }

    public void ResetAlarmList()
    {
        rewardAlarms.Clear();
        Debug.Log("[RankSystem] 보상 알림 리스트 초기화");
    }
}
