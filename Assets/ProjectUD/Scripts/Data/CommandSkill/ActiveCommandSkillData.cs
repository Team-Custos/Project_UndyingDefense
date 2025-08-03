using UnityEngine;
using AttackType = AttackData.AttackType;

[CreateAssetMenu(fileName = "ActiveCommandSkillData", menuName = "ProjectUD/ActiveCommandSkillData")]
public class ActiveCommandSkillData : CommandSkillData
{
    [Header("■ ActiveCommandSkill")]

    [SerializeField] private float damage;
    [SerializeField] private AttackType attackType;
    [SerializeField] private float bonusCrit;
    [SerializeField] private GameObject attackTrigger;

    [Header("■ ActiveCommandSkill - InduseEffect")]
    [SerializeField] protected GameObject induseEffectPrefab;
    [SerializeField] protected float induseEffectSuccessRate;

    [SerializeField] private GameObject critEffectPrefab;

    [Header("■ Data")]
    [SerializeField] protected AttackData attackData;

    public float Damage => damage;
    public AttackType AttackType => attackType;
    public float BonusCrit => bonusCrit;
    public GameObject AttackTrigger => attackTrigger;
    public GameObject InduseEffectPrefab => induseEffectPrefab;
    public float InduseEffectSuccessRate => induseEffectSuccessRate;
    public GameObject CritEffectPrefab => critEffectPrefab;
    public AttackData AttackData => attackData;
}
