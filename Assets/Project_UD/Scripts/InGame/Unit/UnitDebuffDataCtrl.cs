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
    public UnitDebuff name;
    public bool Stackable;
    public int stackLimit;
    public float duration;
    public int tickDamage;
    public AudioClip StartSFX;
    public AudioClip EndSFX;
    public ParticleSystem startParticle;
}

public class UnitDebuffDataCtrl : MonoBehaviour
{
    public UnitDebuffData[] debuffDatas;
}
