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

    [SerializeField] private AttackType type;
    [SerializeField] private GameObject critEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab;

    public AttackType Type => type;
    public GameObject CritEffectPrefab => critEffectPrefab;
    public GameObject HitEffectPrefab => hitEffectPrefab;
}
