using UnityEngine;

public class NervousEffect : StackEffect
{
    [Header("■ Nervous Options")]
    [SerializeField] private float atkPercent;

    protected override void OnStack()
    {
        target.AddAtkMult(atkPercent);
    }

    protected override void OnMaxStack()
    {
        // 공포 효과 추가
    }

    public override void OnRemove()
    {
        target.AddAtkMult(-atkPercent * stack);
    }

}
