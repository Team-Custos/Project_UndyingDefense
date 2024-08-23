using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class UD_ExcelFileReader : MonoBehaviour
{

    public string filePath = "240820 UD_charData";
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
            Debug.Log("파일 없음");
            return unitDatas;
        }


        StringReader reader = new StringReader(csvData.text);
        bool endOfFile = false;

        // 첫 두줄 안읽음
        reader.ReadLine();
        reader.ReadLine();

        while (!endOfFile)
        {
            string dataString = reader.ReadLine();
            if (dataString == null)
            {
                endOfFile = true;
                break;
            }

            var dataValues = dataString.Split(',');

            if (dataValues.Length >= 19)
            {
                if (!int.TryParse(dataValues[0], out int number))
                {
                    number = 0;
                }
                string id = dataValues[1];
                string name = dataValues[2];
                if (!int.TryParse(dataValues[3], out int level))
                {
                    level = 0;
                }
                if (!int.TryParse(dataValues[4], out int cost))
                {
                    cost = 0;
                }
                if (!int.TryParse(dataValues[5], out int hp))
                {
                    hp = 0;
                }
                if (!int.TryParse(dataValues[6], out int attackSpeed))
                {
                    attackSpeed = 0;
                }
                string defenseType = dataValues[7];
                if (!float.TryParse(dataValues[8],out float globalTime))
                {
                    globalTime = 0;
                }
                if (!int.TryParse(dataValues[9], out int mental))
                {
                    mental = 0;
                }
                if (!int.TryParse(dataValues[10], out int moveSpeed))
                {
                    moveSpeed = 0;
                }
                if (!int.TryParse(dataValues[11], out int sightRange))
                {
                    sightRange = 0;
                }
                if (!int.TryParse(dataValues[12], out int attackRange))
                {
                    attackRange = 0;
                }
                string targetSelectType = dataValues[13];
                if (!int.TryParse(dataValues[14], out int critRate))
                {
                    critRate = 0;
                }
                string g_skill = dataValues[15];
                string g_skillName = dataValues[16];
                string s_skill = dataValues[17];
                string s_skillName = dataValues[18];



                UD_UnitDataManager.UnitData unitData = new UD_UnitDataManager.UnitData(number, id, name, level, cost, hp, attackSpeed, defenseType, 
                    globalTime, mental, moveSpeed, sightRange, attackRange, targetSelectType, critRate, g_skill, g_skillName, s_skill, s_skillName);
                unitDatas.Add(unitData);
            }
        }
        return unitDatas;
    }

}
