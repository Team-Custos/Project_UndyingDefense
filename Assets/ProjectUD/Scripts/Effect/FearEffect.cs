using UnityEngine;

public class FearEffect : DurationEffect
{
    [Header("■ Fear Options")]
    [SerializeField] private float atkPercent;
    [SerializeField] private float mental;

    public override void Activate()
    {
        target.AddAtkMult(atkPercent);
        target.AddMental(mental);
    }

    public override void Remove()
    {
        target.AddAtkMult(-atkPercent);
        target.AddMental(-mental);
    }
}
