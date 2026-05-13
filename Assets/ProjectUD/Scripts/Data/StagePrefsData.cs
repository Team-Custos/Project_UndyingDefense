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

    public class StageData
    {
        public string id;
        public bool isOpen;
        public bool isStageEnd;                         // 승패 여부에 따라 저장. 중간에 나갔을 경우에는 포함 안 함
        public float clearTime;
    }

    private StageData lastPlayedStage = new StageData();
    private StringBuilder sb = new StringBuilder();

    private void Start()
    {
        if (PlayerPrefs.GetInt("SetBeginningStage") == 0)
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
        if (sb.Length > 0)
            sb.Clear();

        string[] lines = st.Split('\n');

        for (int i = 1; i < lines.Length; i++)  // 맨 윗줄 빼고라서 1부터.
        {
            sb.AppendLine(lines[i]);
        }

        PlayerPrefs.SetString("stage", sb.ToString());
        PlayerPrefs.SetInt("SetBeginningStage", 1);
    }

    // 딕셔너리에 있는 정보 다시 프랩스로 저장
    public void SaveStageData()
    {
        if (sb.Length > 0)
            sb.Clear();

        foreach (var kvp in stagePlayerPrefs)
        {
            string stageID = kvp.Key;
            StageData stageData = kvp.Value;
            int isOpen = stageData.isOpen ? 1 : 0;
            int isStageEnd = stageData.isStageEnd ? 1 : 0;
            sb.AppendLine($"{stageID},{isOpen},{isStageEnd},{stageData.clearTime}");
        }
        PlayerPrefs.SetString("stage", sb.ToString());
    }

    // 마지막 진입 전장정보 Prefs에 저장
    public void SaveLastStageData()
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
        for (int i = 0; i < lines.Length; i++)      // 첫 줄은 건너뛰고 읽어오기
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] datas = line.Split(",");

            string stageID = datas[0].Trim();
            Debug.Log($"전장이름 : {stageID}");
            string isOpen = datas[1].Trim();
            Debug.Log($"해금여부 : {isOpen}");
            string isPlayed = datas[2].Trim();
            string clearTime = datas[3].Trim();

            StageData stagedata = new StageData();
            stagedata.id = stageID;
            //stagedata.isOpen = bool.Parse(isOpen);          // 문자열로 되어있는 저장정보 파싱
            //stagedata.isStageEnd = bool.Parse(isPlayed);
            stagedata.isOpen = isOpen != "0";
            stagedata.isStageEnd = isPlayed != "0";
            stagedata.clearTime = float.Parse(clearTime);

            dic.Add(stageID, stagedata);
        }
    }

    // 저장데이터를 불러온 딕셔너리 저장 정보 확인
    public Dictionary<string, StageData> GetStageData()
    {
        return latestPlayStage;
    }

    // 전장 정보 변경 메서드
    public void SetStageDictionary(string id, bool isOpen, bool isPlayed, float clearTime)
    {
        StageData stagedata = stagePlayerPrefs[id];
        stagedata.isOpen = isOpen;
        stagedata.isStageEnd = isPlayed;
        stagedata.clearTime = clearTime;

        stagePlayerPrefs[id] = stagedata;
    }
    public void SetGuemsanFinish()  // 인게임매니저 이벤트용 메서드
    {
        PlayerPrefs.SetInt("IsGeumsanFinished", 1);
        Debug.Log("금산전투종료");

        StageData stagedata = stagePlayerPrefs["UNQ_gumsanCastle"];
        stagedata.isStageEnd = true;

        //stagePlayerPrefs["UNQ_gumsanCastle"] = stagedata;
        SaveStageData();
    }
    public void SetGeumsanWin()     // 인게임매니저 이벤트용 메서드
    {
        PlayerPrefs.SetInt("GeumsanWin", 1);
        Debug.Log("금산전투 이김");

        StageData stagedata = stagePlayerPrefs["UNQ_gumsanCastle"];
        stagedata.isStageEnd = true;
        //stagedata.clearTime = "1111";     // 클리어 시간 적용

        //stagePlayerPrefs["UNQ_gumsanCastle"] = stagedata;
        StageData namhan = stagePlayerPrefs["UNQ_namhanFortress"];
        namhan.isOpen = true;
        SaveStageData();
        //stagePlayerPrefs["UNQ_namhanFortress"] = namhan;
    }

    //--- 전장 클리어 여부 저장 메서드
    //--- 나중에 재활용 가능하도록 만들기---260512 ayo
    public void SetNamhanFinish()  // 인게임매니저 이벤트용 메서드
    {
        //PlayerPrefs.SetInt("IsNamhanFinished", 1);
        Debug.Log("남한산성전투종료");
        StageData stagedata = stagePlayerPrefs["UNQ_namhanFortress"];
        stagedata.isStageEnd = true;
        SaveStageData();
    }

    public void SetNamhanWin()     // 인게임매니저 이벤트용 메서드
    {
        //PlayerPrefs.SetInt("NamhanWin", 1);
        Debug.Log("남한산성전투 이김");
        StageData stagedata = stagePlayerPrefs["UNQ_namhanFortress"];
        stagedata.isStageEnd = true;
        StageData nextStage = stagePlayerPrefs["UNQ_namwonCastle"];
        nextStage.isOpen = true;
        SaveStageData();
    }

    public void SetRecordTime(float recordTime, string id)
    {
        float bestTime = stagePlayerPrefs[id].clearTime;
        if (bestTime == 0f || recordTime < bestTime)
        {
            stagePlayerPrefs[id].clearTime = recordTime;
            SaveStageData();
        }
    }

    public void SetTutorialEnd()
    {
        PlayerPrefs.SetInt("IsTutorialEnd", 1);
        Debug.Log("훈련장끝");
    }

    public bool IsNewRecord(float recordTime, string id)
    {
        float bestTime = stagePlayerPrefs[id].clearTime;
        if (bestTime == 0f || recordTime < bestTime)
        {
            return true;
        }
        return false;
    }
}
