using UnityEngine;
using UnityEngine.UI;

public enum EffectType
{
    BUFF,       // 버프
    CURSE,      // 저주
    CRIT,       // 치명
    NATURE,       // 자연
    ETC         // 기타
}

public abstract class DurationEffect : MonoBehaviour
{
    [Header("■ Data")]
    [SerializeField] protected string id;
    [SerializeField] protected string effectName;
    [SerializeField] protected Sprite iconSprite;

    [Header("■ Options")]
    [SerializeField] protected float duration;

    protected float durationCheck;      // 지속 시간을 체크하는 변수
    protected GameObject prefab;
    protected Unit caster;      // 상태를 발동 시킨 유닛
    protected Unit target;
    protected ObjectPoolWithList<DurationEffect> pool;
    protected EffectImage effectImage;
    [SerializeField] private EffectType type;
    


    public string Id => id;
    public string Name => effectName;
    public GameObject Prefab => prefab;
    public Sprite IconSprite => iconSprite;
    public EffectType Type => type;

    public void Initialize(GameObject prefab, ObjectPoolWithList<DurationEffect> pool)
    {
        this.prefab = prefab;
        this.pool = pool;
    }


    public virtual void Initialize(Unit target) // 처음 효과가 유닛에 추가되었을 때
    {
        this.target = target;
        durationCheck = 0f;
    }

    public virtual void SetCaster(Unit caster) // 상태를 발동 시킨 유닛을 설정하는 함수.
    {
        this.caster = caster;
    }

    public virtual void Reapply(GameObject effectPrefab) // 효과를 재적용하는 함수.
    {
        durationCheck = 0f;
    }

    protected virtual void Update()
    {
        durationCheck += Time.deltaTime;
        if (durationCheck >= duration)
        {
            durationCheck = 0f;
            Remove();
        }


    }

    public abstract void Activate();
    public abstract void OnRemove();

    protected virtual void Remove()  // 지속시간 지나면 호출
    {
        target.RemoveEffect(this);
        OnRemove();
        transform.SetParent(null);
        gameObject.SetActive(false);
    }

    public void RemoveEffect()      // 유닛이 죽으면 호출
    {
        OnRemove();
        transform.SetParent(null);
        gameObject.SetActive(false);
    }

    public virtual bool IsSameType(GameObject effectPrefab)
    {
        return prefab == effectPrefab;
    }

    private void OnDisable()
    {
        if (pool == null)
            return;

        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);

        pool.List.Remove(this);
        pool.Pool.Release(this);
    }
}
