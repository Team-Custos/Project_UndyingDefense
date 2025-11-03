using System.Text;
using System.Collections.Generic;
using UnityEngine;
using static StagePrefsData;

public class StagePrefsData : MonoBehaviour
{
    [SerializeField] private TextAsset stageClearData;

    // 불러와서 저장용
    private Dictionary<string, StageData> stagePlayerPrefs = new Dictionary<string, StageData>();
    // 마지막으로 진입한 전장 저장용 
    private Dictionary<string, StageData> latestPlayStage = new Dictionary<string, StageData>();

    public struct StageData
    {
        public string id;
        public bool isOpen;
        public bool isStageEnd;   // 승패 여부에 따라 저장. 중간에 나갔을 경우에는 포함 안 함
        public float clearTime;
    }

    private StageData lastPlayedStage;
    private StringBuilder sb = new StringBuilder();

    private void Start()
    {
        if(PlayerPrefs.GetInt("SetBeginningStage") == 0)
        {
            LoadStageData(stageClearData.text);
        }
        // 스테이지 PlayerPrefs를 저장한 적이 없으면 초기 StageData 불러오기
        // 불러온적이 있으면 스테이지 PlayerPrefs를 불러오기
        ReadPlayerPrefs(stagePlayerPrefs);
    }
    // 저장데이터 모두 초기화
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    // 초기 데이터 저장
    private void LoadStageData(string st)
    {
        PlayerPrefs.SetString("stage", st);
        PlayerPrefs.SetInt("SetBeginningStage", 1);
    }

    // 딕셔너리에 있는 정보 다시 프랩스로 저장
    private void SaveStageData(Dictionary<string, StageData> dic)     
    {
        //string playerPrefData = string.Empty;
        if (sb.Length > 0)
            sb.Clear();

        foreach (var kvp in dic)
        {
            string stageID = kvp.Key;
            StageData stageData = kvp.Value;
            // bool값을 int-> string으로 치환과정 필요
            sb.AppendLine($"{stageID},{stageData.isOpen},{stageData.isStageEnd},{stageData.clearTime}");
            //playerPrefData += $"{stageID},{stageData.isOpen},{stageData.isStageEnd},{stageData.clearTime}\n";
        }
        // PlayerPrefs.SetString("stageData", playerPrefData);
        PlayerPrefs.SetString("stage", sb.ToString());
    }

    // 마지막 진입 전장정보 Prefs에 저장
    public void SaveStageData()
    {
        string lastPlayStage = $"{lastPlayedStage.id},{lastPlayedStage.isOpen},{lastPlayedStage.isStageEnd},{lastPlayedStage.clearTime}\n";

        PlayerPrefs.SetString("lastPlayedStageData", lastPlayStage);

    }

    // 전장 들어갔다 나오기만 할 때 사용 셋팅 메서드
    public void SetLastPlayedStage(string id)
    {
        //lastPlayedStage.id = id;

        // 기존에 있었던 정보 불러오기
        lastPlayedStage.id = stagePlayerPrefs[id].id;
        lastPlayedStage.isOpen = stagePlayerPrefs[id].isOpen;
        lastPlayedStage.isStageEnd = stagePlayerPrefs[id].isStageEnd;
        lastPlayedStage.clearTime = stagePlayerPrefs[id].clearTime;

    }

    // 전장 종료시 사용할 셋팅 메서드 
    public void SetLastPlayedStage(string id, bool isOpen, bool isStageEnd, float clearTime)
    {
        lastPlayedStage.id = id;
        lastPlayedStage.isOpen = isOpen;
        lastPlayedStage.isStageEnd = isStageEnd;
        lastPlayedStage.clearTime = clearTime;
    }

    // 마지막으로 진입한 전장정보를 가져오기 위한 메서드
    public StageData GetStageData(string stageID)
    {
        return stagePlayerPrefs[stageID];
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
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] datas = line.Split(",");

            string stageID = datas[0];
            string isOpen = datas[1];
            string isPlayed = datas[2];
            string clearTime = datas[3];

            StageData stagedata = new StageData();
            stagedata.id = stageID;
            //stagedata.isOpen = bool.Parse(isOpen);          // 문자열로 되어있는 저장정보 파싱
            //stagedata.isStageEnd = bool.Parse(isPlayed);
            stagedata.isOpen = int.Parse(isOpen) != 0;
            stagedata.isStageEnd = int.Parse(isPlayed) != 0;
            stagedata.clearTime = float.Parse(clearTime);

            dic.Add(stageID, stagedata);
        }
    }

    // 저장데이터를 불러온 딕셔너리 저장 정보 확인
    public Dictionary<string, StageData > GetStageData()  
    {
        return latestPlayStage;
    }

    // 전장 정보 변경 메서드
    public void SetStageDictionary(string id, bool isOpen, bool isPlayed,float clearTime)
    {
        StageData stagedata = stagePlayerPrefs[id];
        stagedata.isOpen = isOpen;
        stagedata.isStageEnd = isPlayed;
        stagedata.clearTime = clearTime;

        stagePlayerPrefs[id] = stagedata;
    }
    public void SetGuemsanFinish()
    {
        PlayerPrefs.SetInt("IsGeumsanFinished", 1);
        Debug.Log("금산전투종료");

        StageData stagedata = stagePlayerPrefs["UNQ_gumsan"];
        stagedata.isStageEnd = true;

        stagePlayerPrefs["UNQ_gumsan"] = stagedata;
    }
    public void SetGeumsanWin()
    {
        PlayerPrefs.SetInt("GeumsanWin", 1);
        Debug.Log("금산전투 이김");

        StageData stagedata = stagePlayerPrefs["UNQ_gumsan"];
        stagedata.isStageEnd = true;
        //stagedata.clearTime = "1111";     // 클리어 시간 적용

        stagePlayerPrefs["UNQ_gumsan"] = stagedata;

        StageData namhan = stagePlayerPrefs["UNQ_namhanFortress"];
        namhan.isOpen = true;

        stagePlayerPrefs["UNQ_namhanFortress"] = namhan;
    }

    public void SetTutorialEnd()
    {
        PlayerPrefs.SetInt("IsTutorialEnd", 1);
        Debug.Log("훈련장끝");
    }
}
