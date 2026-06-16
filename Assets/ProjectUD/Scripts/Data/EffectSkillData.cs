using UnityEngine;

[CreateAssetMenu(fileName = "EffectSkillData", menuName = "ProjectUD/EffectSkillData")]
public class EffectSkillData : SkillData
{
    [Header("■ EffectSkill")]
    [SerializeField] protected GameObject effectPrefab; // 버프 프리팹

    public GameObject EffectPrefab => effectPrefab;
}
