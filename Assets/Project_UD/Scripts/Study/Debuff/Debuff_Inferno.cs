using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff_Inferno : UnitDebuff_Rebuild
{
    [Header("====Damage====")]
    [SerializeField] private float tickTimeIntervals; //틱 시간 간격
    [SerializeField] private float Cur_tickTime; //틱 시간
    public float tickDamage; //틱 데미지

    public override void ApplyDebuff(UnitCtrl_ReBuild target)
    {
        //출혈 효과 적용
        target.TakeDamage(tickDamage);
    }

    protected override void Update()
    {
        if (Cur_tickTime <= 0)
        {
            Debug.Log("Burn Tick");
            ApplyDebuff(target);
            Cur_tickTime = tickTimeIntervals;
        }
        else
        {
            Cur_tickTime -= Time.deltaTime;
        }

        base.Update();
    }
}
