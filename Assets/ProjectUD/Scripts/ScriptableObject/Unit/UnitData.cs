using UnityEngine;
using ArmorType = Unit.ArmorType;

public class UnitData : ScriptableObject
{
    [Header("■ Unit")]
    [SerializeField] private string unitName;
    [SerializeField] private int tier;
    [SerializeField] private ArmorType armorType;
    [SerializeField] private float maxHp;
    [SerializeField] private float critChance;
    [SerializeField] private float mental;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite atTypeIcon;
    [SerializeField] private Sprite dfTypeIcon;
    [SerializeField, TextArea] private string description;
    [SerializeField] private string attackType;
    [SerializeField] private string role;
    

    public string Name => unitName;
    public int Tier => tier;
    public ArmorType ArmorType => armorType;
    public float MaxHp => maxHp;
    public float CritChance => critChance;
    public float Mental => mental;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
    public float AttackSpeed => attackSpeed;
    public float SightRange => sightRange;
    public float AttackRange => attackRange;
    public Sprite Icon => icon;
    public Sprite AtTypeIcon => atTypeIcon;
    public Sprite DfTypeIcon => dfTypeIcon;
    public GameObject Prefab => prefab;
    public string Description => description;
    public string AttackType => attackType;
    public string Role => role;

}
