using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnitDataManager;

public class CommandSkillDataReader : MonoBehaviour
{
    public string CDFilePath = "240924 JSD_CDskillData";
    public CommandSkillManager cdSkillManager;

    // Start is called before the first frame update
    void Start()
    {
        if (cdSkillManager == null)
        {
            cdSkillManager = GetComponent<CommandSkillManager>();
        }

        List<CommandSkillManager.SkillData> skillDatas = ReadCSV(CDFilePath);
        cdSkillManager.SetSkillData(skillDatas);

    }
    List<CommandSkillManager.SkillData> ReadCSV(string fileName)
    {
        List<CommandSkillManager.SkillData> skillDatas = new List<CommandSkillManager.SkillData>();

        TextAsset csvCDData = Resources.Load<TextAsset>(fileName);

        if (csvCDData == null)
        {
            Debug.Log("파일 없음");
            return skillDatas;
        }

        StringReader reader = new StringReader(csvCDData.text);
        bool endOfFile = false;

        // 첫 줄 건너 띄기
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

            if (dataValues.Length >= 25)
            {
                if (!int.TryParse(dataValues[0], out int number))
                {
                    number = 0;
                }
                string skillID = dataValues[1];
                string skillName = dataValues[2];
                string skillScript = dataValues[3];
                string skillType = dataValues[4];
                if (!int.TryParse(dataValues[5], out int cooldown))
                {
                    cooldown = 0;
                }
                string targetType = dataValues[6];
                string skillCastType = dataValues[7];
                string trackingType = dataValues[8];
                string areaShape = dataValues[9];
                if (!int.TryParse(dataValues[10], out int areaLength))
                {
                    areaLength = 0;
                }
                if (!int.TryParse(dataValues[11], out int areaWidth))
                {
                    areaWidth = 0;
                }
                string damageType = dataValues[12];
                if (!int.TryParse(dataValues[13], out int baseDamage))
                {
                    baseDamage = 0;
                }
                if (!int.TryParse(dataValues[14], out int damagePerSecond))
                {
                    damagePerSecond = 0;
                }
                string linkID1 = dataValues[15];
                string linkID2 = dataValues[16];
                if (!int.TryParse(dataValues[17], out int duration))
                {
                    duration = 0;
                }
                if (!int.TryParse(dataValues[18], out int speedChange))
                {
                    speedChange = 0;
                }
                if (!int.TryParse(dataValues[19], out int critChanceChange))
                {
                    critChanceChange = 0;
                }
                if (!int.TryParse(dataValues[20], out int attackSpeedChange))
                {
                    attackSpeedChange = 0;
                }
                if (!int.TryParse(dataValues[21], out int mentalChange))
                {
                    mentalChange = 0;
                }
                if (!int.TryParse(dataValues[22], out int healthRegen))
                {
                    healthRegen = 0;
                }
                if (!int.TryParse(dataValues[23], out int damageBlock))
                {
                    damageBlock = 0;
                }
                string targetSelectChange = dataValues[24];

                CommandSkillManager.SkillData skillData = new CommandSkillManager.SkillData(
                    number, skillID, skillName, skillScript, skillType, cooldown, targetType, skillCastType, trackingType,
                    areaShape, areaLength, areaWidth, damageType, baseDamage, damagePerSecond, linkID1, linkID2, duration,
                    speedChange, critChanceChange, attackSpeedChange, mentalChange, healthRegen, damageBlock, targetSelectChange
                );

                skillDatas.Add(skillData);
            }
        }

        return skillDatas;
    }


}
