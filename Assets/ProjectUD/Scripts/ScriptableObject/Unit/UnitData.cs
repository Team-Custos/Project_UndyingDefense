using UnityEngine;
using UnityEngine.Rendering;
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
    [SerializeField] private string gSkillText;
    [SerializeField] private string sSkillText;
    [SerializeField] private Sprite gSkillIcon;
    [SerializeField] private Sprite sSkillIcon;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject prefab;

    public string Name => unitName;
    public int Tier => tier;
    public ArmorType ArmorType => armorType;
    public float MaxHp => maxHp;
    public float CritChance => critChance;
    public float Mental => mental;
    public float MoveSpeed => moveSpeed;
    public float AttackSpeed => attackSpeed;
    public float SightRange => sightRange;
    public float AttackRange => attackRange;

    public string GSkillText => gSkillText;
    public string SSkillText => sSkillText;
    public Sprite SSkillIcon => sSkillIcon;
    public Sprite GSkillIcon => gSkillIcon;
    public Sprite Icon => icon;
    public GameObject Prefab => prefab;
}
