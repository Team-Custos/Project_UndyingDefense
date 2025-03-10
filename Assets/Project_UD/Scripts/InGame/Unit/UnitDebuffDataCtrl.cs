using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 유닛의 디버프의 기본 데이터를 관리하기 위한 스크립트입니다.
public enum UnitDebuff
{
    None,
    Bleed,
    Dizzy,
    Stun,
    Trapped,
    Burn,
    Inferno,
    Poison
}

[Serializable]
public class UnitDebuffData
{
    public UnitDebuff name; //이름
    public bool Stackable; //스택 가능 여부
    public int stackLimit; //스택 최대 수
    public float duration; //지속 시간
    public int tickDamage; //틱 데미지
    public AudioClip StartSFX; //효과 시작시 SFX
    public AudioClip EndSFX; //효과 종료시 SFX
    public ParticleSystem startParticle; //효과 시작시 VFX
}

public class UnitDebuffDataCtrl : MonoBehaviour
{
    public UnitDebuffData[] debuffDatas;
}
