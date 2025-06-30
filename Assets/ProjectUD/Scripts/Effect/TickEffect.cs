using UnityEngine;
using UltEvents;

public class TickEffect : DurationEffect
{
    [Header("■ Tick Damage Options")]
    [SerializeField] private float tickTime;
    [SerializeField] private UltEvent onTick;

    private float tickTimeCheck;
    private int tickCount;

    protected override void Update()
    {
        if (tickTimeCheck < tickTime)
        {
            tickTimeCheck += Time.deltaTime;
        }
        else
        {
            tickTimeCheck -= tickTime;
            tickCount++;
            if (onTick != null)
                onTick.Invoke();
        }

        base.Update();
    }

    public override void AddStack()
    {
        base.AddStack();
        tickCount = 0;
    }

    public override void Activate()
    {
        base.Activate();
        tickTimeCheck = 0f;
    }

    public override void AddMoveSpeedPercent(float percent)
    {
        target.AddMoveSpeedMultiplier(percent * 0.01f);

        float removeValue = -percent * 0.01f * (stack + 1) * tickCount;
        onRemove.AddListener(() => AddMoveSpeedPercent(removeValue));
    }

    public override void AddAttackSpeedPercent(float percent)
    {
        target.AddAttackSpeedMultiplier(percent * 0.01f);

        float removeValue = -percent * 0.01f * (stack + 1) * tickCount;
        onRemove.AddListener(() => AddAttackSpeedPercent(removeValue));
    }

    public override void AddCriticalVulnerability(float percent)
    {
        target.AddCriticalVulnerability(percent);

        float removeValue = -percent * (stack + 1) * tickCount;
        onRemove.AddListener(() => AddCriticalVulnerability(removeValue));
    }

    public override void AddBlockRate(float percent)
    {
        target.AddBlockRate(percent * 0.01f);

        float removeValue = -percent * 0.01f * (stack + 1) * tickCount;
        onRemove.AddListener(() => AddBlockRate(removeValue));
    }
}
