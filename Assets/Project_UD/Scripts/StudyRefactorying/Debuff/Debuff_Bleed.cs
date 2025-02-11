using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff_Bleed : UnitDebuff_Rebuild
{
    [Header("====FX====")]
    public AudioClip StartSFX; //효과 시작시 SFX
    public AudioClip EndSFX; //효과 종료시 SFX
    public ParticleSystem startParticle; //효과 시작시 VFX
    public ParticleSystem activePartice; //효과 중 VFX

    [Header("====Damage====")]
    [SerializeField] public float tickTimeIntervals = 3f; //틱 시간 간격
    [SerializeField] private float Cur_tickTime = 0f; //틱 시간
    public float tickDamage; //틱 데미지

    public override void ApplyDebuff(UnitCtrl_ReBuild target)
    {
        //출혈 효과 적용
        target.TakeDamage(tickDamage);
    }

    public override void Update()
    {
        if (Cur_tickTime <= 0)
        {
            Debug.Log("Bleed Tick");
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
