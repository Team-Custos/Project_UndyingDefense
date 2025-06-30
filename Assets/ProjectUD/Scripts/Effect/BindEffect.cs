using UnityEngine;

public class BindEffect : DurationEffect
{
    // 250629 : 임시로 이동 속도 계수를 0으로 설정함. 추후 변경 예정.

    public override void Activate()
    {
        target.AddMoveSpeedMult(-1f);
    }

    public override void Remove()
    {
        target.AddMoveSpeedMult(1f);
    }
}
