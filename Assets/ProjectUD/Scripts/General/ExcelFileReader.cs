using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExcelFileReader : MonoBehaviour
{

    public string filePath = "charData";
    public UnitExcelDataManager unitExcelDataManager;

    // Update is called once per frame
    void Start()
    {
        if (unitExcelDataManager == null)
        {
            unitExcelDataManager = GetComponent<UnitExcelDataManager>();
        }

        List<UnitExcelDataManager.UnitExcelData> unitExcelDatas = ReadCSV(filePath);
        unitExcelDataManager.SetUnitData(unitExcelDatas);

        //PrintAllUnitData(unitExcelDatas);
    }


    List<UnitExcelDataManager.UnitExcelData> ReadCSV(string fileName)
    {
        List<UnitExcelDataManager.UnitExcelData> unitExcelDatas = new List<UnitExcelDataManager.UnitExcelData>();
        TextAsset csvData = Resources.Load<TextAsset>(fileName);

        if (csvData == null)
        {
            Debug.Log("파일 없음");
            return unitExcelDatas;
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

            if (dataValues.Length < 7 || string.IsNullOrWhiteSpace(dataString))
            {
                continue; // 데이터가 유효하지 않으면 다음으로 건너뜀
            }

            string unitCode = dataValues[0];
            string name = dataValues[1];
            if (!int.TryParse(dataValues[2], out int level))
            {
                level = 0;
            }
            if (!int.TryParse(dataValues[3], out int cost))
            {
                cost = 0;
            }
            string g_SkillName = dataValues[4];
            string s_SkillName = dataValues[5];
            string g_SkillInfo = dataValues[6];
            string s_SkillInfo = dataValues[7];


            UnitExcelDataManager.UnitExcelData unitData = new UnitExcelDataManager.UnitExcelData(unitCode, name, level, cost, g_SkillName, s_SkillName,
                                                              g_SkillInfo, s_SkillInfo);
            unitExcelDatas.Add(unitData);
        }

        return unitExcelDatas;
    }

    void PrintAllUnitData(List<UnitExcelDataManager.UnitExcelData> unitExcelDatas)
    {
        foreach (var unitData in unitExcelDatas)
        {
            Debug.Log($"Code: {unitData.unitCode}, Name: {unitData.name}, Level: {unitData.level}, Cost: {unitData.cost}, " +
                      $"g_Skill: {unitData.g_SkillName}, g_SkillInfo: {unitData.g_SkillInfo}, " +
                      $"s_Skill: {unitData.s_SkillName}, s_SkillInfo: {unitData.s_SkillInfo}");
        }
    }



}
