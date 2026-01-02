using UnityEngine;
using TargetType = SkillBase.TargetType;

public class SkillData : ScriptableObject
{
    [Header("■ Data")]
    [SerializeField] private new string name;
    [SerializeField] private Sprite icon;
    [SerializeField, TextArea] private string description;
    [SerializeField] private float coolTime;
    [SerializeField] private TargetType targetType;
    [SerializeField] private int needMental = 0;    // 스킬 사용에 필요한 정신력
    //[SerializeField] private AudioClip[] startSFX;
    //[SerializeField] private ParticleSystem startVFX;
    [SerializeField] private AudioClip[] attackSFX;


    public string Name => name;
    public string Description => description;
    public float CoolTime => coolTime;
    public TargetType TargetType => targetType;
    //public AudioClip[] StartSFX => startSFX;
    //public ParticleSystem StartVFX => startVFX;
    public Sprite Icon => icon;
    public AudioClip[] AttackSFX => attackSFX;
    public int NeedMental => needMental;
}
