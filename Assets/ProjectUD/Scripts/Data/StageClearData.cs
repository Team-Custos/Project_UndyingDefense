using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class StageClearData : MonoBehaviour
{
    [SerializeField] private TextAsset stageClearData;
    private List<StageData> startStageList = new List<StageData>();
    private List<StageData> stagePlayerPrefs = new List<StageData>();
    private Dictionary<string, string> stageClear = new Dictionary<string, string>();

    public struct StageData
    {
        public string id;
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
            stagedata.id = stageID;
            stagedata.isClear = isClear;

            startStageList.Add(stagedata);

            readLine = sr.ReadLine();   // 다음줄 읽기
        }
    }

    private void SaveToPlayerPrefs(List<StageData> stageDatas)
    {
        string playerPrefData = "";
        for (int i = 0;  i < startStageList.Count; i++)
        {
            //playerPrefData += $"{startStageList[i].id}, {startStageList[i].isClear}";
            playerPrefData += $"{stageDatas[i].id}, {stageDatas[i].isClear}";
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
            //int isClear = int.Parse(data[1]);
            string isClear = data[1];

            StageData stageData = new StageData();
            stageData.id = id;
            stageData.isClear = isClear;

            //stagePlayerPrefs.Add(stageData);
            stageClear.Add(id, isClear);    // 딕셔너리

            sr.ReadLine();
        }
    }

    public Dictionary<string, string> GetStageData()    // 변경예정
    {
        return stageClear;
    }

}
