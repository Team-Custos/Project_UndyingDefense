using UnityEngine;
using TargetType = CommandSkill.TargetType;
using CommandSkillType = CommandSkill.CommandSkillType;
using AttackType = AttackSkill.AttackType;

[CreateAssetMenu(fileName = "CommandSkillData", menuName = "ProjectUD/CommandSkillData")]
public class CommandSkillData : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField, TextArea] private string description;
    [SerializeField] private float coolTime;
    [SerializeField] private AudioClip[] startSFX;
    [SerializeField] private ParticleSystem startVFX;
    [SerializeField] private Sprite icon;
    [Header("■ CommandSkill")]
    [SerializeField] private CommandSkillType commandType;
    [SerializeField] private TargetType targetType;
    [SerializeField] private float damage;
    [SerializeField] private AttackType attackType;
    [SerializeField] private float bonusCrit;
    
    [Header("■ CommandSkill - InduseEffect")]
    [SerializeField] protected Effect induseEffct;
    [SerializeField] protected float induseEffectSuccessRate;

    public string Name => name;
    public string Description => description;
    public float CoolTime => coolTime;
    public TargetType TargetType => targetType;
    public AudioClip[] StartSFX => startSFX;
    public ParticleSystem StartVFX => startVFX;
    public Sprite Icon => icon;
    public CommandSkillType CommandType => commandType;
    public float Damage => damage;
    public AttackType AttackType => attackType;
    public float BonusCrit => bonusCrit;
    public Effect InduseEffct => induseEffct;
    public float InduseEffectSuccessRate => induseEffectSuccessRate;

}