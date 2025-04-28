using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealSkillData", menuName = "ProjectUD/HealSkillData")]
public class HealSkillData : SkillData
{
    [Header("■ HealSkill")]
    [SerializeField] protected float healAmountPercent; // 회복량

    public float HealAmountPercent => healAmountPercent;
}
