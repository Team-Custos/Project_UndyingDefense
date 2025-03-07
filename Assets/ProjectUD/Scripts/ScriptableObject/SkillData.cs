using UnityEngine;
using TargetType = SkillBase.TargetType;

public class SkillData : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField, TextArea] private string description;
    [SerializeField] private float coolTime;
    [SerializeField] private TargetType targetType;
    [SerializeField] private Sprite icon;

    public string Name => name;
    public string Description => description;
    public float CoolTime => coolTime;
    public TargetType TargetType => targetType;
    public Sprite Icon => icon;
}
