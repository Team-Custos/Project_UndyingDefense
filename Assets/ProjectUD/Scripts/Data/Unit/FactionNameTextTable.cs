using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTextTable
{
    public string id;
    public string name;
}

public class FactionNameTextTable : MonoBehaviour
{
    [SerializeField] private TextAsset nameTable;   // 데이터가 저장된 CSV 파일

    private Dictionary<string, string> fNameTextTable = new Dictionary<string, string>();

    private void Start()
    {
        //SetTextTable();
        LoadTextTable();
    }

    private void LoadTextTable()
    {
        if (nameTable == null)
        {
            Debug.Log("데이터 없음");
            return;
        }
        string[] lines = nameTable.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            string[] values = line.Split(',');
            if (string.IsNullOrWhiteSpace(line)) continue;
            NameTextTable nameText = new NameTextTable
            {
                id = values[0],
                name = values[1],
            };
            fNameTextTable.Add(nameText.id, nameText.name);
        }
    }

    public void SetTextTable()
    {
        fNameTextTable.Add("ally", "조선 의병 목록");
        fNameTextTable.Add("moor", "무어 용병단 목록");
        fNameTextTable.Add("pioneer", "태양의 제국 목록");
        fNameTextTable.Add("summon", "신성의 증거단 목록");
    }

    public string GetName(string id)
    {
        string kName = fNameTextTable[id];
        return kName;
    }
}
