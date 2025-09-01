using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionNameTextTable : MonoBehaviour
{
    private Dictionary<string, string> fNameTextTable = new Dictionary<string, string>();

    private void Start()
    {
        SetTextTable();
    }
    public void SetTextTable()
    {
        fNameTextTable.Add("ally", "조선 의병 목록");
        fNameTextTable.Add("moor", "무어 용병단 목록");
        fNameTextTable.Add("pioneer", "태양의 제국 목록");
        fNameTextTable.Add("summon", "신성의 증거단 목록");
    }

    public string GetName(string name)
    {
        string fkName = fNameTextTable[name];
        return fkName;
    }
}
