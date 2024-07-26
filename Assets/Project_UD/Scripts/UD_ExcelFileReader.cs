using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UD_ExcelFileReader : MonoBehaviour
{

    public string filePath1 = "240724 UD_charData";
    public UD_UnitDataManager unitDataManager;

    // Update is called once per frame
    void Start()
    {
        if (unitDataManager == null)
        {
            unitDataManager = GetComponent<UD_UnitDataManager>();
        }

        List<UD_UnitDataManager.UnitData> unitDatas = ReadCSV(filePath1);
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
        Debug.Log(csvData.text);

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

            if (dataValues.Length >= 14)
            {
                string name = dataValues[1];
                string type = dataValues[0];
                int tier;
                if (string.IsNullOrWhiteSpace(dataValues[2]))
                {
                    tier = 0;
                }
                else
                {
                    tier = int.Parse(dataValues[2]);
                }
                string weapon = dataValues[3];
                string skillType = dataValues[4];
                string skill = dataValues[5];
                string description = dataValues[6];
                string attackType = dataValues[7];
                int damage;
                if (!int.TryParse(dataValues[8], out damage))
                {
                    damage = 0;
                }
                int targetCount = int.Parse(dataValues[9]);
                string hitShape = dataValues[10];
                int critRate;
                if (!int.TryParse(dataValues[11], out critRate))
                {
                    critRate = 0;
                }
                string critEffect1 = dataValues[12];
                string critEffect2 = dataValues[13];

                

                UD_UnitDataManager.UnitData unitData = new UD_UnitDataManager.UnitData(
                    name, type, tier, weapon, skillType, skill, description, attackType, damage, targetCount, hitShape, critRate, critEffect1, critEffect2);
                unitDatas.Add(unitData);
            }
        }
        return unitDatas;
    }

}
