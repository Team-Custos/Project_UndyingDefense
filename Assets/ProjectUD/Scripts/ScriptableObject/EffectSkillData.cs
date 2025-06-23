using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectSkillData", menuName = "ProjectUD/EffectSkillData")]
public class EffectSkillData : SkillData
{
    [Header("■ EffectSkill")]
    [SerializeField] protected Effect effect; // 버프 프리팹
    [SerializeField] protected float successRate; // 성공률

    public Effect Effect => effect;
    public float SuccessRate => successRate;
}
