using UnityEngine;
using UltEvents;

public class Effect : MonoBehaviour
{
    [Header("■ Effect Options")]
    [SerializeField] protected string id;
    [SerializeField] protected int maxStack;
    [SerializeField] protected ParticleSystem[] stackVFX;
    [SerializeField] protected ParticleSystem endVFX;

    [Header("■ Activate Effect")]
    [SerializeField] protected UltEvent onActivate;
    [SerializeField] protected UltEvent onMaxStack;

    protected UltEvent onRemove = new UltEvent();

    protected int stack;
    protected Unit unit;
    protected Unit target;

    public string Id => id;

    public virtual void Initialize(Unit unit, Unit target) // 처음 효과가 유닛에 추가되었을 때
    {
        this.unit = unit;
        this.target = target;
        this.transform.localPosition = Vector3.zero;
        Initialize();
    }

    public virtual void Initialize() // 이미 유닛에 있는 효과를 초기화할 때
    {
        stack = 0;
        ChangeStackVFX();
    }

    public virtual void Activate() // 효과를 발동할 때
    {
        if (stack < maxStack)
        {
            stack++;
            ChangeStackVFX();
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

    private void ChangeStackVFX()
    {
        if (stackVFX.Length > 0)
        {
            for (int idx = 0; idx < stackVFX.Length; idx++)
            {
                if (idx == stack)
                {
                    if (idx > 1)
                        Debug.Log(22);

                    stackVFX[idx].gameObject.SetActive(true);
                }
                else
                {
                    stackVFX[idx].gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// "damage" 만큼 데미지를 줍니다.
    /// </summary>
    public void DealDamage(float damage)
    {
        target.TakeDamage(damage);
    }

    /// <summary>
    /// 목표 유닛의 최대 HP에 비례한 "percent"(%) 만큼 데미지를 줍니다.
    /// </summary>
    public void DealDamagePercent(float percent)
    {
        target.TakeDamage(target.Data.MaxHp * 0.01f * percent);
    }

    public virtual void AddMoveSpeedPercent(float percent)
    {
        target.AddMoveSpeedMultiplier(percent * 0.01f);

        float removeValue = -percent * 0.01f;
        onRemove.AddListener(() => target.AddMoveSpeedMultiplier(removeValue));
    }

    public virtual void AddAttackSpeedPercent(float percent)
    {
        target.AddAttackSpeedMultiplier(percent * 0.01f);

        float removeValue = -percent * 0.01f;
        onRemove.AddListener(() => target.AddAttackSpeedMultiplier(removeValue));
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

    public virtual void AddAdditionalDamage(float percent)
    {
        target.AddAdditionalDamage(percent);
        float removeValue = -percent * maxStack;
        onRemove.AddListener(() => AddAdditionalDamage(removeValue));
    }

    public virtual void AddMental(float amount)
    {
        target.AddMental(amount);
        float removeValue = -amount * maxStack;
        onRemove.AddListener(() => AddMental(removeValue));
    }

    public virtual void AddDamageReduction(float percent)
    {
        target.AddDamageReduction(percent);
        float removeValue = -percent * maxStack;
        onRemove.AddListener(() => AddDamageReduction(removeValue));
    }

    public virtual void GetProvoked()
    {
        Debug.Log("getprovoked");
        target.GetProvoked(unit);
        onRemove.AddListener(() => target.RemoveProvoked());
    }

    public virtual void AddEffect(GameObject effectPrefab)
    {
        if(effectPrefab.TryGetComponent(out Effect effect))
            target.AddEffect(unit, effect);
    }
}
