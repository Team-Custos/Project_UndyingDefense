using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "ProjectUD/AttackData")]
public class AttackData : ScriptableObject
{
    public enum AttackType
    {
        SLASH,          // 베기
        PIERCE,         // 찌르기
        CRUSH,          // 때리기

        ELECTRIC,       // 전기
        FIRE,           // 불
        POISON,         // 독
        TRUE_DAMAGE,    // 고정 데미지
    }

    // 모든 공격 속성 공통 사용
    [Header("■ Data")]
    [SerializeField] private AttackType type;
    [SerializeField] private Sprite typeIcon;


    // SLASH, PIERCE, CRUSH 만 사용
    [SerializeField] private GameObject critEffectPrefab;       // 치명 상태 부여 Prefab

    [Header("■ VFX")]
    [SerializeField] private GameObject critVFX;                // 치명타 VFX
    [SerializeField] private GameObject hitVFX;                 // 일반타 VFX

    [Header("■ SFX")]
    [SerializeField] private AudioClip[] hitSFXClip;            // 치명타 SFX
    [SerializeField] private AudioClip critSFXClip;             // 일반타 VFX

    public AttackType Type => type;
    public Sprite TypeIcon => typeIcon;
    public GameObject CritEffectPrefab => critEffectPrefab;
    public GameObject CritVFX => critVFX;
    public GameObject HitVFX => hitVFX;
    //public float VFXDuration => vfxDuration;
    public AudioClip[] HitSFXClip => hitSFXClip;
    public AudioClip CritSFXClip => critSFXClip;
}
