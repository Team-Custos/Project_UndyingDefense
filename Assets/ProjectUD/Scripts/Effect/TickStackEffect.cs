using UnityEngine;

public abstract class TickStackEffect : TickEffect
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
        stack = 1;
    }

    public override void Activate()
    {
        stack++;
    }

    protected abstract void OnMaxStack();
}
