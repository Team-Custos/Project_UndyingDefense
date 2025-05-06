using UnityEngine;
using TargetType = CommandSkill.TargetType;



public class CommandSkillData : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField, TextArea] private string description;
    [SerializeField] private float coolTime;
    [SerializeField] private AudioClip[] startSFX;
    [SerializeField] private ParticleSystem startVFX;
    [SerializeField] private ParticleSystem loopVFX;
    [SerializeField] private Sprite icon;
    [SerializeField] private TargetType targetType;

    public string Name => name;
    public string Description => description;
    public float CoolTime => coolTime;
    public AudioClip[] StartSFX => startSFX;
    public ParticleSystem StartVFX => startVFX;
    public ParticleSystem LoopVFX => loopVFX;
    public Sprite Icon => icon;

    public TargetType TargetType => targetType;
}