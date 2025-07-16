using UnityEngine;

public abstract class TickEffect : DurationEffect
{
    [Header("■ TickEffect Options")]
    [SerializeField] protected float tickTime;

    protected float tickTimeCheck;

    protected override void Update()
    {
        tickTimeCheck += Time.deltaTime;
        if (tickTimeCheck >= tickTime)
        {
            tickTimeCheck -= tickTime;
            
            OnTick();
            Debug.Log("aa");
            
        }

        base.Update();
    }

    public override void Reapply(GameObject effectPrefab)
    {
        base.Reapply(effectPrefab);
        tickTimeCheck = 0f;
    }

    public override void Initialize(Unit target)
    {
        base.Initialize(target);
        tickTimeCheck = 0f;
    }

    protected abstract void OnTick();
}
