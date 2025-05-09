using UnityEngine;
using AttackType = AttackSkill.AttackType;

[CreateAssetMenu(fileName = "ActiveCommandSkillData", menuName = "ProjectUD/ActiveCommandSkillData")]
public class ActiveCommandSkillData : CommandSkillData
{
    [Header("■ ActiveCommandSkill")]

    [SerializeField] private float damage;
    [SerializeField] private AttackType attackType;
    [SerializeField] private float bonusCrit;
    [SerializeField] private GameObject attackTrigger;

    [Header("■ ActiveCommandSkill - InduseEffect")]
    [SerializeField] protected GameObject induseEffct;
    [SerializeField] protected float induseEffectSuccessRate;


    public float Damage => damage;
    public AttackType AttackType => attackType;
    public float BonusCrit => bonusCrit;
    public GameObject AttackTrigger => attackTrigger;
    public GameObject InduseEffct => induseEffct;
    public float InduseEffectSuccessRate => induseEffectSuccessRate;
}
