using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using static StageClearData;

public class StageClearData : MonoBehaviour
{
    [SerializeField] private TextAsset stageClearData;

    // 불러와서 저장용
    private Dictionary<string, StageData> stagePlayerPrefs = new Dictionary<string, StageData>();
    // 마지막으로 진입한 전장 저장용 => 하나만 저장할건데 Dictionary생성? 그냥 플레이어 프랩스에 저장?
    private Dictionary<string, StageData> latestPlayStage = new Dictionary<string, StageData>();

    public struct StageData
    {
        public string id;
        public string isOpen;
        public string isStageEnd;
        public string clearTime;
    }

    private StageData lastPlayedStage;

    private void Start()
    {
        LoadStageData(stageClearData.text);
        // 스테이지 PlayerPrefs를 저장한 적이 없으면 초기 StageData 불러오기
        // 불러온적이 있으면 스테이지 PlayerPrefs를 불러오기
    }

    // 초기 데이터 저장
    private void LoadStageData(string st)
    {
        PlayerPrefs.SetString("stage", st);     
    }

    // 딕셔너리에 있는 정보 다시 프랩스로 저장
    private void SaveStageData(Dictionary<string, StageData> dic)     
    {
        string playerPrefData = string.Empty;
        foreach (var kvp in dic)
        {
            string stageID = kvp.Key;
            StageData stageData = kvp.Value;
            playerPrefData += $"{stageID},{stageData.isOpen},{stageData.isStageEnd},{stageData.clearTime}\n";
        }
        PlayerPrefs.SetString("stageData", playerPrefData);
    }

    // 마지막 진입 전장정보 Prefs에 저장
    public void SaveStageData()
    {
        string lastPlayStage = string.Empty;
        lastPlayStage += $"{lastPlayedStage.id},{lastPlayedStage.isOpen},{lastPlayedStage.isStageEnd},{lastPlayedStage.clearTime}\n";

        PlayerPrefs.SetString("lastPlayedStageData", lastPlayStage);

    }

    // 전장 들어갔다 나오기만 할 때 사용 셋팅 메서드
    public void SetLastPlayedStage(string id)
    {

    }

    // 전장 종료시 사용할 셋팅 메서드
    public void SetLastPlayedStage(string id, string isOpen, string isStageEnd, string clearTime)
    {
        lastPlayedStage.id = id;
        lastPlayedStage.isOpen = isOpen;
        lastPlayedStage.isStageEnd = isStageEnd;
        lastPlayedStage.clearTime = clearTime;
    }

    // 저장된 Stage 프랩스 불러오기
    private void ReadPlayerPrefs(Dictionary<string, StageData> dic)      
    {
        string st = PlayerPrefs.GetString("stage");
        
        // 저장데이터 딕셔너리에 저장하기 (인게임에서 정보 변경용)
        string[] lines = st.Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] datas = line.Split(",");

            string stageID = datas[0];
            string isOpen = datas[1];
            string isPlayed = datas[2];
            string clearTime = datas[3];

            StageData stagedata = new StageData();
            stagedata.id = stageID;
            stagedata.isOpen = isOpen;
            stagedata.isStageEnd = isPlayed;
            stagedata.clearTime = clearTime;

            dic.Add(stageID, stagedata);
        }
    }

    // 저장데이터를 불러온 딕셔너리 저장 정보 확인
    public Dictionary<string, StageData > GetStageData()  
    {
        return latestPlayStage;
    }

    // 마지막으로 진입한 전장정보를 가져오기 위한 메서드
    public StageData GetStageData(string stageID)
    {
        return stagePlayerPrefs[stageID];
    }

    // 딕셔너리 정보 변경 메서드
    public void SetStageDictionary(string id, string isOpen, string isPlayed, string clearTime)
    {
        StageData stagedata = new StageData();
        stagedata.isOpen = isOpen;
        stagedata.isStageEnd = isPlayed;
        stagedata.clearTime = clearTime;

        stagePlayerPrefs[id] = stagedata;
    }

}
