using UnityEngine;

[CreateAssetMenu(fileName = "EffectSkillData", menuName = "ProjectUD/EffectSkillData")]
public class EffectSkillData : SkillData
{
    [Header("■ EffectSkill")]
    [SerializeField] protected GameObject vfx;
    public GameObject VFX => vfx;
}
