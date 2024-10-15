using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommandSkillManager;

public class CommandSkillManager : MonoBehaviour
{
    public static CommandSkillManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    [System.Serializable]
    public class SkillData
    {
        public int Number;
        public string SkillID;
        public string SkillName;
        public string SkillScript;
        public string SkillType;
        public int CoolTime;
        public string TargetType;
        public string SkillCastType;
        public string TraceType;
        public string AreaShape;
        public int AreaLength;
        public int AreaWidth;
        public string DamageType;
        public int BaseDamage;
        public int DamagePs;
        public string LinkID1;
        public string LinkID2;
        public int Time;
        public int SpeedChange;
        public int CriRateChange;
        public int AttackSpeed;
        public int MentalChange;
        public int HpRengen;
        public int DamageBlock;
        public string TargetSelectTypeChange;

        public SkillData(int number, string skillID, string skillName, string skillScript, string skillType, int coolTime, string targetType, 
                         string skillCastType, string traceType, string areaShape, int areaLength, int areaWidth, string damageType, int baseDamage, 
                         int damagePs, string linkID1, string linkID2, int time, int speedChange, int criRateChange, int attackSpeed, int mentalChange,
                         int hpRengen, int damageBlock, string targetSelectTypeChange)
        {
            Number = number;
            SkillID = skillID;
            SkillName = skillName;
            SkillScript = skillScript;
            SkillType = skillType;
            CoolTime = coolTime;
            TargetType = targetType;
            SkillCastType = skillCastType;
            TraceType = traceType;
            AreaShape = areaShape;
            AreaLength = areaLength;
            AreaWidth = areaWidth;
            DamageType = damageType;
            BaseDamage = baseDamage;
            DamagePs = damagePs;
            LinkID1 = linkID1;
            LinkID2 = linkID2;
            Time = time;
            SpeedChange = speedChange;
            CriRateChange = criRateChange;
            AttackSpeed = attackSpeed;
            MentalChange = mentalChange;
            HpRengen = hpRengen;
            DamageBlock = damageBlock;
            TargetSelectTypeChange = targetSelectTypeChange;
        }
    }

    public Dictionary<string, SkillData> skillDataDictionary = new Dictionary<string, SkillData>();

    public void SetSkillData(List<SkillData> skillDataList)
    {
        foreach (var skillData in skillDataList)
        {
            skillDataDictionary[skillData.SkillID] = skillData;
        }

        //ShowSkillData();
    }


    public SkillData GetSkillData(string skillID)
    {
        if (skillDataDictionary.TryGetValue(skillID, out SkillData skillData))
        {
            return skillData;
        }
        else
        {
            Debug.Log("스킬 데이터 없음");
            return null;
        }
    }


    public void ShowSkillData()
    {
        foreach (var skillDataEntry in skillDataDictionary)
        {
            var skillID = skillDataEntry.Key;
            var skillData = skillDataEntry.Value;

            Debug.Log($"Skill ID : {skillID}, Number : {skillData.Number}, Name : {skillData.SkillName}, Script : {skillData.SkillScript}, " +
                      $"Type : {skillData.SkillType}, CoolTime : {skillData.CoolTime}, TargetType : {skillData.TargetType}, " +
                      $"SkillCastType : {skillData.SkillCastType}, TraceType : {skillData.TraceType}, AreaShape : {skillData.AreaShape}, " +
                      $"AreaLength : {skillData.AreaLength}, AreaWidth : {skillData.AreaWidth}, DamageType : {skillData.DamageType}, " +
                      $"BaseDamage : {skillData.BaseDamage}, DamagePs : {skillData.DamagePs}, LinkID1 : {skillData.LinkID1}, " +
                      $"LinkID2 : {skillData.LinkID2}, Time : {skillData.Time}, SpeedChange : {skillData.SpeedChange}, " +
                      $"CriRateChange : {skillData.CriRateChange}, AttackSpeed : {skillData.AttackSpeed}, " +
                      $"MentalChange : {skillData.MentalChange}, HpRengen : {skillData.HpRengen}, DamageBlock : {skillData.DamageBlock}, " +
                      $"TargetSelectTypeChange : {skillData.TargetSelectTypeChange}");
        }
    }
}
