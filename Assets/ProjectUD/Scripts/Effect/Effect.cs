using UnityEngine;
using UltEvents;

public class Effect : MonoBehaviour
{
    [Header("■ Effect Options")]
    [SerializeField] protected string id;
    [SerializeField] protected int maxStack;

    [Header("■ Activate Effect")]
    [SerializeField] protected UltEvent onActivate;
    [SerializeField] protected UltEvent onMaxStack;

    protected UltEvent onRemove;

    protected int stack;
    protected Unit unit;
    protected Unit target;

    public string Id => id;

    public virtual void Initialize(Unit unit, Unit target) // 처음 효과가 유닛에 추가되었을 때
    {
        this.unit = unit;
        this.target = target;
        Initialize();
    }

    public virtual void Initialize() // 이미 유닛에 있는 효과를 초기화할 때
    {
        stack = 0;
    }

    public virtual void Activate() // 효과를 발동할 때
    {
        if (stack < maxStack)
        {
            stack++;
            if (onActivate != null)
                onActivate.Invoke();
        }
        else
        {
            if (onMaxStack != null)
                onMaxStack.Invoke();
        }
    }

    public virtual void Remove()
    {
        if(onRemove != null)
        {
            onRemove.Invoke();
            onRemove.Clear();
        }

        gameObject.SetActive(false);
    }

    public void DealDamage(float damage)
    {
        target.TakeDamage(damage);
    }

    public virtual void AddMoveSpeedPercent(float percent)
    {
        target.AddMoveSpeedMultiplier(percent * 0.01f);

        float removeValue = -percent * 0.01f * maxStack;
        onRemove.AddListener(() => AddMoveSpeedPercent(removeValue));
    }

    public virtual void AddAttackSpeedPercent(float percent)
    {
        target.AddAttackSpeedMultiplier(percent * 0.01f);

        float removeValue = -percent * 0.01f * maxStack;
        onRemove.AddListener(() => AddAttackSpeedPercent(removeValue));
    }

    public virtual void AddCriticalVulnerability(float percent)
    {
        target.AddCriticalVulnerability(percent);

        float removeValue = -percent * maxStack;
        onRemove.AddListener(() => AddCriticalVulnerability(removeValue));
    }

    public virtual void AddBlockRate(float percent)
    {
        target.AddBlockRate(percent * 0.01f);

        float removeValue = -percent * 0.01f * maxStack;
        onRemove.AddListener(() => AddBlockRate(removeValue));
    }

    public virtual void AddEffect(GameObject effectPrefab)
    {
        if(effectPrefab.TryGetComponent(out Effect effect))
            target.AddEffect(unit, effect);
    }
}
