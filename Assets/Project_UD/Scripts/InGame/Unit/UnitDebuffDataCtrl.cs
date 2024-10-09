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
    Tied,
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
}

public class UnitDebuffDataCtrl : MonoBehaviour
{
    public UnitDebuffData[] debuffDatas;
}
