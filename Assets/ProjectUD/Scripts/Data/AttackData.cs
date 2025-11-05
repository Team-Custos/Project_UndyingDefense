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
    [SerializeField] private Sprite typeIcon;

    [Header("■ VFX")]
    [SerializeField] private GameObject critVFX;
    [SerializeField] private GameObject hitVFX;
    //[SerializeField] private float vfxDuration;     // VFX 스스로가 가질 데이터로 변경
    [SerializeField] private string vfxName;

    [Header("■ SFX")]
    [SerializeField] private AudioClip[] hitSFXClip;
    [SerializeField] private AudioClip critSFXClip;

    public AttackType Type => type;
    public Sprite TypeIcon => typeIcon;
    public GameObject CritEffectPrefab => critEffectPrefab;
    public GameObject CritVFX => critVFX;
    public GameObject HitVFX => hitVFX;
    //public float VFXDuration => vfxDuration;
    public string VFXName => vfxName;
    public AudioClip[] HitSFXClip => hitSFXClip;
    public AudioClip CritSFXClip => critSFXClip;
}
