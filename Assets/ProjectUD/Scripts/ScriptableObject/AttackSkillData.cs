using UnityEngine;
using AttackType = AttackSkill.AttackType;

[CreateAssetMenu(fileName = "AttackSkillData", menuName = "ProjectUD/AttackSkillData")]
public class AttackSkillData : SkillData
{
    [Header("■ AttackSkill")]
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected float damage;
    [SerializeField] protected float bonusCrit;

    public AttackType AttackType => attackType;
    public float Damage => damage;
    public float BonusCrit => bonusCrit;
}
