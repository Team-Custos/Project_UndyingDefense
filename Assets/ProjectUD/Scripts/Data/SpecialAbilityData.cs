using UnityEngine;

[CreateAssetMenu(fileName = "SpecialAbilityData", menuName = "ProjectUD/SpecialAbilityData")]
public class SpecialAbilityData : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField, TextArea] private string description;

    public string Name => name;
    public string Description => description;
}
