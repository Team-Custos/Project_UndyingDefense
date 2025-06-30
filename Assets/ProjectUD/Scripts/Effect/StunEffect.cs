using UnityEngine;

public class StunEffect : DurationEffect
{
    public override void Activate()
    {
        target.GetStun();
    }

    public override void Remove()
    {
        target.RemoveStun();
    }
}
