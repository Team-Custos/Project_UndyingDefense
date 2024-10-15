using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int stackLimit;
    public float duration;
    public int tickDamage;
    public AudioClip StartSFX;
    public AudioClip EndSFX;
}

public class UnitDebuffDataCtrl : MonoBehaviour
{
    public UnitDebuffData[] debuffDatas;
}
