using UnityEngine;

public abstract class StackEffect : DurationEffect
{
    [Header("■ StackEffect Options")]
    [SerializeField] protected int maxStack;

    protected int stack;

    public override void Reapply(GameObject effectPrefab)
    {
        base.Reapply(effectPrefab);
        if (stack < maxStack)
        {
            stack++;
            OnStack();
        }
        else
        {
            Remove();
            OnMaxStack();
        }
    }

    public override void Initialize(Unit target)
    {
        base.Initialize(target);
        stack = 0;
    }

    public override void Activate()
    {
        stack++;
        OnStack();
    }

    protected abstract void OnStack();

    protected abstract void OnMaxStack();
}
