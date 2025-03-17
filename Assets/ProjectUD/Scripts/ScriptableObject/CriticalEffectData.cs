using UnityEngine;

[CreateAssetMenu(fileName = "CriticalEffectData", menuName = "ProjectUD/CriticalEffectData")]
public class CriticalEffectData : ScriptableObject
{
    [SerializeField] private GameObject[] effectPrefabs;
    public GameObject[] EffectPrefabs => effectPrefabs;
}
