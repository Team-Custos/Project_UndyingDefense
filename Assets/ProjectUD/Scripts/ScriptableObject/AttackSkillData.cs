using UnityEngine;
using AttackType = AttackSkill.AttackType;

[CreateAssetMenu(fileName = "AttackSkillData", menuName = "ProjectUD/AttackSkillData")]
public class AttackSkillData : SkillData
{
    [Header("■ AttackSkill")]
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected float damage;
    [SerializeField] protected float bonusCrit;

    [Header("■ AttackSkill - InduseEffect")]
    [SerializeField] protected Effect induseEffect;
    [SerializeField] protected float induseEffectSuccessRate;

    public AttackType AttackType => attackType;
    public float Damage => damage;
    public float BonusCrit => bonusCrit;

    public Effect InduseEffect => induseEffect;
    public float InduseEffectSuccessRate => induseEffectSuccessRate;
}
