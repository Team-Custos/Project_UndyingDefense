using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UD_ExcelFileReader : MonoBehaviour
{

    public string filePath = "240716 UD_charData"; // 파일 경로
    public UD_UnitDataManager unitDataManager;

    // Update is called once per frame
    void Start()
    {
        if (unitDataManager == null)
        {
            unitDataManager = GetComponent<UD_UnitDataManager>();
        }

        List<UD_UnitDataManager.UnitData> unitDatas = ReadCSV(filePath);
        unitDataManager.SetUnitData(unitDatas);
    }


    List<UD_UnitDataManager.UnitData> ReadCSV(string fileName)
    {
        List<UD_UnitDataManager.UnitData> unitDatas = new List<UD_UnitDataManager.UnitData>();
        TextAsset csvData = Resources.Load<TextAsset>(fileName);

        if (csvData == null)
        {
            return unitDatas;
        }


        Debug.Log(csvData.text);

        StringReader reader = new StringReader(csvData.text);
        bool endOfFile = false;
        int lineNumber = 0;

        
        reader.ReadLine();
        reader.ReadLine();

        while (!endOfFile)
        {
            string dataString = reader.ReadLine();
            lineNumber++;
            if (dataString == null)
            {
                endOfFile = true;
                break;
            }


            var dataValues = dataString.Split(',');

            if (dataValues.Length >= 6)
            {
                string name = dataValues[2];
                string type = dataValues[3];
                int level = int.Parse(dataValues[4]);
                int hp = int.Parse(dataValues[5]);
                string material = dataValues[6];

                UD_UnitDataManager.UnitData unit = new UD_UnitDataManager.UnitData(name, type, level, hp, material);
                unitDatas.Add(unit);
            }
        }
        Debug.Log("파일읽기 끝");
        return unitDatas;
    }

}
