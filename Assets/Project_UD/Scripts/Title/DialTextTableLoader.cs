using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public class DialTextTableLoader : MonoBehaviour
{
    [SerializeField] private TextAsset dialogueTable;
    [SerializeField] private TextAsset choiceTable;

    private Dictionary<string, List<string>> dialogueDictionary = new Dictionary<string, List<string>>();

    private Dictionary<string, string> choiceDic = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        LoadDialTable(dialogueTable);
        LoadChoiceTable(choiceTable);
    }



    public void LoadDialTable(TextAsset textAsset)
    {
        StringReader sr = new StringReader(textAsset.text);
        string readline = sr.ReadLine();
        string currentLineID = string.Empty;

        while (!string.IsNullOrEmpty(readline))
        {
            string[] data = readline.Split(',');
            string id = data[0];
            string line = data[1];

            // lineID 가 Empty가 아니라면 currentLineID 로 저장
            if (id != string.Empty)
            {
                currentLineID = id;

                // 리스트를 생성해서 line을 저장 
                List<string> lines = new List<string>();
                lines.Add(line);

                //딕셔너리에 lineID 와 List를 저장
                dialogueDictionary.Add(id, lines);
            }

            // 줄을 불러왔는데 lineID가 비어있다면
            // currentLineID 에 저장했던 lineID를 Key값으로 딕셔너리를 찾아 line을 추가
            else
            {
                dialogueDictionary[currentLineID].Add(line);
            }

            // 다음 줄 읽고 while문 종료 ( while문 돌 때 다음 줄을 저장하기 위함 )
            readline = sr.ReadLine();

        }
    }

    public void LoadChoiceTable(TextAsset textAsset)
    {
        StringReader sr = new StringReader(textAsset.text);
        string readline = sr.ReadLine();

        while (!string.IsNullOrEmpty(readline))
        {
            string[] data = readline.Split(',');
            string id = data[0];
            string line = data[1];

            choiceDic.Add(id, line);

            // 다음 줄 읽고 while문 종료 ( while문 돌 때 다음 줄을 저장하기 위함 )
            readline = sr.ReadLine();

        }
    }

    public List<string> GetTextData(string id)  // lineID를 통해 List 불러오는 함수
    {
        return dialogueDictionary[id];
    }

    public string GetChoiceData(string id)  // id를 통해 string을 불러오는 함수
    {
        return choiceDic[id];
    }
}
