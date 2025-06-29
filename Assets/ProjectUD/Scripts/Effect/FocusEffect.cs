using UnityEngine;

public class FocusEffect : DurationEffect
{
    [Header("■ Focus Options")]
    [SerializeField] private float mental;

    public override void Activate()
    {
        target.AddMental(mental);
    }

    public override void Remove()
    {
        target.AddMental(-mental);
    }
}
