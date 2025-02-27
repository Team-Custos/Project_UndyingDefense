using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff_Trapped : UnitEffect
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

    private void Awake()
    {
        tickTimeIntervals = 3f;
        Cur_tickTime = tickTimeIntervals;
        tickDamage = 1f;
    }

    public override void ApplyDebuff(Unit target)
    {
        //이속 0으로 만들기?

    }

}
