using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PassiveCommandSkill : CommandSkill
{
    public enum StatsToChange
    {
        None,
        MaxHP,
        SkillDamage,
        AttackSpeed,
        Gold
    }

    [Header("■ Data")]
    [SerializeField] private PassiveCommandSkillData data;

    public override CommandSkillData Data => data;

    public float AddStats(float statToChange)
    {
        float finalStat = statToChange;

        if (data != null)
        {
            finalStat += data.Amount;
        }

        return finalStat;
    }

    public float AddStatsPercent(float statToChange)
    {
        float finalStat = statToChange;

        if (data != null)
        {
            finalStat += finalStat * data.Amount * 0.01f;
        }

        return finalStat;
    }
}
