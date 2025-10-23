using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class StageClearData : MonoBehaviour
{
    [SerializeField] private TextAsset stageClearData;
    private List<StageData> startStageList = new List<StageData>();
    //private List<StageData> stagePlayerPrefs = new List<StageData>();

    private Dictionary<string, StageData> stageData = new Dictionary<string, StageData>();
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

    private void LoadStageData()    // 초기 StageData 불러오기
    {
        StringReader sr = new StringReader(stageClearData.text);
        string readLine = sr.ReadLine();

        while (readLine != null)
        {
            string[] data = readLine.Split(',');
            string stageID = data[0];
            string isClear = data[1];

            StageData stagedata = new StageData();
            //stagedata.id = stageID;
            stagedata.isClear = isClear;

            startStageList.Add(stagedata);  // 리스트 => 딕셔너리 변경예정

            stageData.Add(stageID, stagedata);    // 딕셔너리

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
            playerPrefData += $"{stageID}, {stageData.isClear}\n";
        }
        PlayerPrefs.SetString("stageData", playerPrefData);
    }

    private void SaveToPlayerPrefs(List<StageData> stageDatas)  // 리스트에 저장했을 때 사용한 코드
    {
        string playerPrefData = "";
        for (int i = 0;  i < startStageList.Count; i++)
        {
            //playerPrefData += $"{startStageList[i].id}, {startStageList[i].isClear}";
            //playerPrefData += $"{stageDatas[i].id}, {stageDatas[i].isClear}";   
            
            playerPrefData += "\n";
        }
        PlayerPrefs.SetString("stageData" , playerPrefData);
    }

    private void ReadPlayerPrefs(string st)      // 저장된 Stage 불러오기
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

    public Dictionary<string, StageData > GetStageData()    // 변경예정
    {
        return stageData;
    }

}
