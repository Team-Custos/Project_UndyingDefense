using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "ProjectUD/AttackData")]
public class AttackData : ScriptableObject
{
    public enum AttackType
    {
        SLASH,
        PIERCE,
        CRUSH,
        NONE
    }

    [Header("■ Data")]
    [SerializeField] private AttackType type;
    [SerializeField] private GameObject critEffectPrefab;
    
    [Header("■ VFX")]
    [SerializeField] private GameObject critVFX;
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private float vfxDuration;

    public AttackType Type => type;
    public GameObject CritEffectPrefab => critEffectPrefab;
    public GameObject CritVFX => critVFX;
    public GameObject HitVFX => hitVFX;
    public float VFXDuration => vfxDuration;
}
