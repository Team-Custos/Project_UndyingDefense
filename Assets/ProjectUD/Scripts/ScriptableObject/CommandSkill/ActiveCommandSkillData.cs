using UnityEngine;
using AttackType = AttackSkill.AttackType;

[CreateAssetMenu(fileName = "ActiveCommandSkillData", menuName = "ProjectUD/ActiveCommandSkillData")]
public class ActiveCommandSkillData : CommandSkillData
{
    [Header("■ ActiveCommandSkill")]

    [SerializeField] private float damage;
    [SerializeField] private AttackType attackType;
    [SerializeField] private float bonusCrit;

    [Header("■ ActiveCommandSkill - InduseEffect")]
    [SerializeField] protected Effect induseEffct;
    [SerializeField] protected float induseEffectSuccessRate;


    public float Damage => damage;
    public AttackType AttackType => attackType;
    public float BonusCrit => bonusCrit;
    public Effect InduseEffct => induseEffct;
    public float InduseEffectSuccessRate => induseEffectSuccessRate;
}
