using UnityEngine;

public abstract class StackEffect : DurationEffect
{
    [Header("■ StackEffect Options")]
    [SerializeField] protected int maxStack;

    protected int stack;

    public override void Reapply(GameObject effectPrefab)
    {
        base.Reapply(effectPrefab);
        stack = 0;
    }

    public override void Activate()
    {
        if(stack < maxStack)
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

    protected abstract void OnStack();
    protected abstract void OnMaxStack();
}
