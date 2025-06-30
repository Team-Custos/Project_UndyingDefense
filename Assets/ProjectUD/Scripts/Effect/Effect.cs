using UnityEngine;
using UltEvents;

<<<<<<< HEAD
=======
<<<<<<< Updated upstream
>>>>>>> KimJK
public class Effect : MonoBehaviour
{
    [Header("■ Effect Options")]
    [SerializeField] protected string id;
    [SerializeField] protected string effectName;
    [SerializeField, TextArea] protected string description;

    [SerializeField] protected int maxStack;
    [SerializeField] protected ParticleSystem[] stackVFX;
    [SerializeField] protected ParticleSystem endVFX;

    [Header("■ Activate Effect")]
    [SerializeField] protected UltEvent onActivate;
    [SerializeField] protected UltEvent onMaxStack;

    protected UltEvent onRemove = new UltEvent();
    [SerializeField] protected GameObject maxStackEffectPrefab;

    protected int stack;
    protected Unit unit;
    protected Unit target;

    public string Id => id;
    public string Name => effectName;
    public string Description => description;
    public GameObject MaxStackEffectPrefab => maxStackEffectPrefab;

    public string MaxStackEffectId
    {
        get
        {
            if (!maxStackEffectPrefab)
                return string.Empty;
            else
                return maxStackEffectPrefab.GetComponent<Effect>().Id;
        }
    }


    public virtual void Initialize(Unit unit, Unit target) // 처음 효과가 유닛에 추가되었을 때
    {
        this.unit = unit;
        this.target = target;
        this.transform.localPosition = Vector3.zero;
        stack = 0;
        ChangeStackVFX(stack);
    }

    public virtual void Initialize()
    {
        stack = 0;
        ChangeStackVFX(stack);
    }

    public bool IsMaxStackEffect(Effect effect)
    {
        if (!maxStackEffectPrefab)
            return false;
        else
            return effect.id == MaxStackEffectId;
    }

    public bool IsSameEffect(Effect effect)
    {
        return effect.id == id; 
    }


    public virtual void AddStack() // 이미 유닛에 있는 효과의 스택을 추가.
    {
        if (stack < maxStack)
        {
            stack++;
            ChangeStackVFX(stack);
        }
    }

    public virtual void Activate() // 효과를 발동할 때
    {
        if (stack < maxStack || maxStack == 0)
        {
            if (onActivate != null)
                onActivate.Invoke();
        }
        else
        {
            if (maxStack > 0)
            {
                if (onMaxStack != null)
                {
                    //Debug.Log("onMaxStack");

                    if (maxStackEffectPrefab != null)
                    {
                        Effect maxStackEffect = maxStackEffectPrefab.GetComponent<Effect>();
                        target.AddEffect(unit, maxStackEffect);
                    }
                    Remove();
                    onMaxStack.Invoke();
                }
            }
        }
    }

    public virtual void Remove()
    {
        if(onRemove != null)
        {
            target.EffectList.Remove(this);
            this.unit.UpdateState();

            onRemove.Invoke();
            onRemove.Clear();
        }

        stack = 0;
        gameObject.SetActive(false);
    }

    private void ChangeStackVFX(int stack)
    {
        if (stackVFX.Length > 0)
        {
            for (int idx = 0; idx < stackVFX.Length; idx++)
            {
                if (idx == stack)
                {
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
<<<<<<< HEAD
        target.TakeDamage(target.UnitStats.maxHp * 0.01f * percent);
=======
        target.TakeDamage(target.Data.MaxHp * 0.01f * percent);
>>>>>>> KimJK
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

        float removeValue = -percent * (stack + 1);
        onRemove.AddListener(() => AddCriticalVulnerability(removeValue));
    }

    public virtual void AddBlockRate(float percent)
    {
        target.AddBlockRate(percent * 0.01f);

        float removeValue = -percent * 0.01f * (stack + 1);
        onRemove.AddListener(() => AddBlockRate(removeValue));
    }

    public virtual void AddAdditionalDamage(float percent)
    {
        target.AddAdditionalDamage(percent);
        float removeValue = -percent * (stack + 1);
        onRemove.AddListener(() => AddAdditionalDamage(removeValue));
    }

    public virtual void AddMental(float amount)
    {
        target.AddMental(amount);
        float removeValue = -amount * (stack + 1);
        onRemove.AddListener(() => AddMental(removeValue));
    }

    public virtual void AddDamageReduction(float percent)
    {
        target.AddDamageReduction(percent);
        float removeValue = -percent * (stack + 1);
        onRemove.AddListener(() => AddDamageReduction(removeValue));
    }

    public virtual void GetProvoked()
    {
        Debug.Log("getprovoked");
        target.GetProvoked(unit);
        onRemove.AddListener(() => target.RemoveProvoked());
    }

    public virtual void GetStun()
    {
        target.GetStun();
        onRemove.AddListener(() => target.RemoveStun());
    }

    public virtual void AddEffect(GameObject effectPrefab)
    {
        if(effectPrefab.TryGetComponent(out Effect effect))
            target.AddEffect(unit, effect);
    }
}
<<<<<<< HEAD
=======
=======
#region 250629_기존 코드
//public class Effect : MonoBehaviour
//{
//    [Header("■ Effect Data")]
//    [SerializeField] protected string id;
//    [SerializeField] protected string effectName;
//    [SerializeField, TextArea] protected string description;

//    [SerializeField] protected int maxStack;
//    [SerializeField] protected ParticleSystem[] stackVFX;

//    [Header("■ Activate Effect")]
//    [SerializeField] protected UltEvent onActivate;
//    [SerializeField] protected UltEvent onMaxStack;

//    protected UltEvent onRemove = new UltEvent();
//    [SerializeField] protected GameObject maxStackEffectPrefab;

//    protected int stack;
//    protected Unit unit;
//    protected Unit target;

//    // 스택별로 VFX가 다름?


//    public string Id => id;
//    public string Name => effectName;
//    public string Description => description;
//    public GameObject MaxStackEffectPrefab => maxStackEffectPrefab;

//    public string MaxStackEffectId
//    {
//        get
//        {
//            if (!maxStackEffectPrefab)
//                return string.Empty;
//            else
//                return maxStackEffectPrefab.GetComponent<Effect>().Id;
//        }
//    }


//    public virtual void Initialize(Unit unit, Unit target) // 처음 효과가 유닛에 추가되었을 때
//    {
//        this.unit = unit;
//        this.target = target;
//        this.transform.localPosition = Vector3.zero;
//        stack = 0;
//        ChangeStackVFX(stack);
//    }

//    public virtual void Initialize()
//    {
//        stack = 0;
//        ChangeStackVFX(stack);
//    }

//    public bool IsMaxStackEffect(Effect effect)
//    {
//        if (!maxStackEffectPrefab)
//            return false;
//        else
//            return effect.id == MaxStackEffectId;
//    }

//    public bool IsSameEffect(Effect effect)
//    {
//        return effect.id == id; 
//    }


//    public virtual void AddStack() // 이미 유닛에 있는 효과의 스택을 추가.
//    {
//        if (stack < maxStack)
//        {
//            stack++;
//            ChangeStackVFX(stack);
//        }
//    }

//    public virtual void Activate() // 효과를 발동할 때
//    {
//        if (stack < maxStack || maxStack == 0)
//        {
//            if (onActivate != null)
//                onActivate.Invoke();
//        }
//        else
//        {
//            if (maxStack > 0)
//            {
//                if (onMaxStack != null)
//                {
//                    //Debug.Log("onMaxStack");
//                    onMaxStack.Invoke();
//                }

//                if (maxStackEffectPrefab != null)
//                {
//                    Effect maxStackEffect = maxStackEffectPrefab.GetComponent<Effect>();
//                    target.AddEffect(unit, maxStackEffect);
//                }
//                Remove();
//            }
//        }
//    }

//    public virtual void Remove()
//    {
//        if(onRemove != null)
//        {
//            //target.EffectList.Remove(this);
//            target.UpdateState();

//            onRemove.Invoke();
//            onRemove.Clear();
//        }

//        stack = 0;
//        gameObject.SetActive(false);
//    }

//    private void ChangeStackVFX(int stack)
//    {
//        if (stackVFX.Length > 0)
//        {
//            for (int idx = 0; idx < stackVFX.Length; idx++)
//            {
//                if (idx == stack)
//                {
//                    stackVFX[idx].gameObject.SetActive(true);   
//                }
//                else
//                {
//                    stackVFX[idx].gameObject.SetActive(false);
//                }                
//            }
//        }
//    }

//    /// <summary>
//    /// "damage" 만큼 데미지를 줍니다.
//    /// </summary>
//    public void DealDamage(float damage)
//    {
//        target.TakeDamage(damage);
//    }

//    /// <summary>
//    /// 목표 유닛의 최대 HP에 비례한 "percent"(%) 만큼 데미지를 줍니다.
//    /// </summary>
//    public void DealDamagePercent(float percent)
//    {
//        target.TakeDamage(target.UnitStats.maxHp * 0.01f * percent);
//    }

//    public virtual void AddMoveSpeedPercent(float percent)
//    {
//        target.AddMoveSpeedMultiplier(percent * 0.01f);

//        float removeValue = -percent * 0.01f;
//        onRemove.AddListener(() => target.AddMoveSpeedMultiplier(removeValue));
//    }

//    public virtual void AddAttackSpeedPercent(float percent)
//    {
//        target.AddAttackSpeedMultiplier(percent * 0.01f);

//        float removeValue = -percent * 0.01f;
//        onRemove.AddListener(() => target.AddAttackSpeedMultiplier(removeValue));
//    }

//    public virtual void AddCriticalVulnerability(float percent)
//    {
//        target.AddCriticalVulnerability(percent);

//        float removeValue = -percent * (stack + 1);
//        onRemove.AddListener(() => AddCriticalVulnerability(removeValue));
//    }

//    public virtual void AddBlockRate(float percent)
//    {
//        target.AddBlockRate(percent * 0.01f);

//        float removeValue = -percent * 0.01f * (stack + 1);
//        onRemove.AddListener(() => AddBlockRate(removeValue));
//    }

//    public virtual void AddAdditionalDamage(float percent)
//    {
//        target.AddAdditionalDamage(percent);
//        float removeValue = -percent * (stack + 1);
//        onRemove.AddListener(() => AddAdditionalDamage(removeValue));
//    }

//    public virtual void AddMental(float amount)
//    {
//        target.AddMental(amount);
//        float removeValue = -amount * (stack + 1);
//        onRemove.AddListener(() => AddMental(removeValue));
//    }

//    public virtual void AddDamageReduction(float percent)
//    {
//        target.AddDamageReduction(percent);
//        float removeValue = -percent * (stack + 1);
//        onRemove.AddListener(() => AddDamageReduction(removeValue));
//    }

//    public virtual void GetProvoked()
//    {
//        Debug.Log("getprovoked");
//        target.GetProvoked(unit);
//        onRemove.AddListener(() => target.RemoveProvoked());
//    }

//    public virtual void GetStun()
//    {
//        target.GetStun();
//        onRemove.AddListener(() => target.RemoveStun());
//    }

//    public virtual void AddEffect(GameObject effectPrefab)
//    {
//        if(effectPrefab.TryGetComponent(out Effect effect))
//            target.AddEffect(unit, effect);
//    }
//}
#endregion
>>>>>>> Stashed changes
>>>>>>> KimJK
