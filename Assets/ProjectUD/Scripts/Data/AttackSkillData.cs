using UnityEngine;
using AttackType = AttackData.AttackType;

[CreateAssetMenu(fileName = "AttackSkillData", menuName = "ProjectUD/AttackSkillData")]
public class AttackSkillData : SkillData
{
    [Header("■ AttackSkill")]
    [SerializeField] protected AttackData info;
    [SerializeField] protected float damage;
    [SerializeField] protected float bonusCritPercent;
    [SerializeField] private bool ignoreDefenseType = false; // 방어 속성 무시 여부

    [Header("■ AttackSkill - InduseEffect")]
    [SerializeField] protected GameObject induseEffectPrefab;
    [SerializeField] protected float induseEffectSuccessRate;

    public float Damage => damage;
    public float BonusCritPercent => bonusCritPercent;
    public AttackData Info => info;
    public GameObject InduseEffectPrefab => induseEffectPrefab;
    public float InduseEffectSuccessRate => induseEffectSuccessRate;
    public bool IgnoreDefenseType => ignoreDefenseType;

    //[Header("■ AttackSkill")]
    //[SerializeField] protected AttackType attackType;




    //public AttackType AttackType => attackType;
    //public float Damage => damage;
    //public float BonusCrit => bonusCrit;


}
