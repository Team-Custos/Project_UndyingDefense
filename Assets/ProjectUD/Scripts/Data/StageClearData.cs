using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class StageClearData : MonoBehaviour
{
    [SerializeField] private TextAsset stageClearData;

    // 초기 데이터 저장용 -> 필요없음
    private Dictionary<string, StageData> stageData = new Dictionary<string, StageData>();
    // 불러와서 저장용
    private Dictionary<string, StageData> stagePlayerPrefs = new Dictionary<string, StageData>();

    public struct StageData
    {
        //public string id;
        public string isClear;
    }

    private void Start()
    {
        LoadStageData();
        // 스테이지 PlayerPrefs를 저장한 적이 없으면 초기 StageData 불러오기
        // 불러온적이 있으면 스테이지 PlayerPrefs를 불러오기
    }

    private void LoadStageData()    // 초기 stageClearData.text 불러오기
    {
        StringReader sr = new StringReader(stageClearData.text); // 여기서 PlayerPrefs 자체를 읽기

        string readLine = sr.ReadLine();

        while (readLine != null)
        {
            string[] data = readLine.Split(',');
            string stageID = data[0];
            string isClear = data[1];

            StageData stagedata = new StageData();
            //stagedata.id = stageID;
            stagedata.isClear = isClear;

            // startStageList.Add(stagedata);  // 리스트 => 딕셔너리 변경예정

            //stageData.Add(stageID, stagedata);    // 딕셔너리

            readLine = sr.ReadLine();   // 다음줄 읽기
        }
    }

    private void SaveToPlayerPrefs(Dictionary<string, StageData> dic)     // 초기데이터 딕셔너리에 저장
    {
        string playerPrefData = "";
        foreach (var kvp in dic)
        {
            string stageID = kvp.Key;
            StageData stageData = kvp.Value;
            playerPrefData += $"{stageID},{stageData.isClear}\n";
        }
        PlayerPrefs.SetString("stageData", playerPrefData);
    }
    private void ReadPlayerPrefs(string st, Dictionary<string, StageData> dic)      // 저장된 Stage 불러오기
    {
        PlayerPrefs.SetString("stage", st);     // 초기 데이터 저장
        
        // 저장데이터 딕셔너리에 저장하기 (인게임에서 정보 변경용)
        string readLine = st;
        string[] lines = readLine.Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] datas = readLine.Split(",");
            string stageID = datas[0];  // 엑셀 첫칸 ID     // 후에 엑셀 두번째칸은 전장이름으로 예시만들어서 수정예정
            string isClear = datas[1];  // 

            StageData stageData = new StageData();
            stageData.isClear = isClear;

            stagePlayerPrefs.Add(stageID, stageData);
        }
    }

    private void ReadPlayerPrefs()      // 저장된 Stage 불러오기
    {
        StringReader sr = new StringReader(PlayerPrefs.GetString("stageData"));
        string readLine = sr.ReadLine();
        while (readLine != null)
        {
            string[] data = readLine.Split(",");
            string id = data[0];
            string isClear = data[1];

            StageData stageData = new StageData();
            //stageData.id = id;
            stageData.isClear = isClear;

            stagePlayerPrefs.Add(id, stageData);    // 딕셔너리로 저장

            sr.ReadLine();
        }
    }

    // 저장데이터를 불러온 딕셔너리 저장 정보 확인
    public Dictionary<string, StageData > GetStageData()  
    {
        return stageData;
    }

    // 딕셔너리 정보 변경 메서드
    public void SetStageDictionary(string key, string value)
    {
        StageData stagedata = new StageData();
        stagedata.isClear = value;
        stagePlayerPrefs[key] = stagedata;
    }

}
