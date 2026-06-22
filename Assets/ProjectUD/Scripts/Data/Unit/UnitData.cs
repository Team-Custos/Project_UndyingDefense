using UnityEngine;
using ArmorType = Unit.ArmorType;

public class UnitData : ScriptableObject
{
    //[SerializeField] private float maxHp;
    //[SerializeField] private float moveSpeed;
    //[SerializeField] private float mental;
    //[SerializeField] private float critChance;
    //[SerializeField] private float attackSpeed;
    //[SerializeField] private float sightRange;
    //[SerializeField] private float attackRange;
    [SerializeField] private int tier;

    [Header("■ Unit")]
    [SerializeField] private string unitName;
    [SerializeField] private ArmorType armorType;
    [SerializeField] private string id;

    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite atTypeIcon;
    [SerializeField] private Sprite dfTypeIcon;
    [SerializeField, TextArea] private string description;

    [SerializeField] private SkillData generalSkill;
    [SerializeField] private SkillData specialSkill;
    [SerializeField] private SpecialAbility specialAbility;
    [SerializeField] private string campName;   // 유닛의 진영 정보 (도감 UI에 필요)

    public string Name => unitName;
    public ArmorType ArmorType => armorType;
    public Sprite Icon => icon;
    public Sprite AtTypeIcon => atTypeIcon;
    public Sprite DfTypeIcon => dfTypeIcon;
    public GameObject Prefab => prefab;
    public string Description => description;
    //public string Role => role;
    public string Id => id;
    public string CampName => campName;
    public SkillData SpecialSkill => specialSkill;
    public SkillData GeneralSkill => generalSkill;
    public SpecialAbility SpecialAbility => specialAbility;


    //public float MaxHp => maxHp;
    public int Tier => tier;
    //public float CritChance => critChance;
    //public float Mental => mental;
    //public float MoveSpeed
    //{
    //    get => moveSpeed;
    //    set => moveSpeed = value;
    //}
    //public float AttackSpeed => attackSpeed;
    //public float SightRange => sightRange;
    //public float AttackRange => attackRange;

}
