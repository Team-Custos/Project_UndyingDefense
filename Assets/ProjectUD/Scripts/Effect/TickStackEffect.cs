using UnityEngine;

public abstract class TickStackEffect : TickEffect
{
    [Header("■ StackEffect Options")]
    [SerializeField] protected int maxStack;

    protected int stack;

    public override void Activate()
    {
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

    protected abstract void OnMaxStack();
}
