using UnityEngine;
using TargetType = CommandSkill.TargetType;



public class CommandSkillData : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private string id;
    [SerializeField] int rank;
    [SerializeField, TextArea] private string description;
    [SerializeField] private float coolTime;
    [SerializeField] private AudioClip startSFX;
    [SerializeField] private AudioClip loopSFX;
    [SerializeField] private ParticleSystem startVFX;
    [SerializeField] private ParticleSystem loopVFX;
    [SerializeField] private Sprite icon;
    [SerializeField] private TargetType targetType;


    public string Name => name;
    public string Id => id;
    public int Rank => rank;
    public string Description => description;
    public float CoolTime => coolTime;
    public AudioClip StartSFX => startSFX;
    public AudioClip LoopSFX => loopSFX;
    public ParticleSystem StartVFX => startVFX;
    public ParticleSystem LoopVFX => loopVFX;
    public Sprite Icon => icon;
    public TargetType TargetType => targetType;
}