using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class DurationTickEffectSkill : EffectSkill
{
    [SerializeField] private Unit unit;
    [SerializeField] private UltEvent<Unit> onTick; // tickTime 마다 실행될 이벤트

    [SerializeField] private float duration = 5f;
    private float durationCheck = 0f;

    [SerializeField] private float tickTime = 1f;
    private float tickTimeCheck = 0f;

    private bool isSkillActivated = false;

    public override void Initialize()
    {
        base.Initialize();

        durationCheck = 0f;
        tickTimeCheck = 0f;
        isSkillActivated = false;
    }

    public void ActivateSkill(Unit unit)
    {
        isSkillActivated = true;
    }

    public override void Update()
    {
        base.Update();

        if (!isSkillActivated)
            return;

        if(unit.IsDead)
            isSkillActivated = false;

        if (unit is AllyUnit allyUnit)
        {
            if (allyUnit.IsChange || allyUnit.IsUpgrade)
                isSkillActivated = false;
        }

        if (durationCheck < duration)    // 지속 시간 진행 중
        {
            durationCheck += Time.deltaTime;
            tickTimeCheck += Time.deltaTime;

            if (tickTimeCheck >= tickTime)
            {
                onTick.Invoke(unit);
                tickTimeCheck -= tickTime;
                Debug.Log("onTickTime");
            }
        }
        else
        {
            durationCheck -= duration;
            isSkillActivated = false;
        }
    }


}
