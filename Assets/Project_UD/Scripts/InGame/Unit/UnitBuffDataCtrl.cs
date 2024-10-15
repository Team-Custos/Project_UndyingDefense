using System;
using UnityEngine;

public enum UnitBuff
{
    None,
    Charge,
    Regeneration,
    Samgyetang,
    SkillDamageUp,
}

[Serializable]
public class UnitBuffData
{
    public UnitBuff name;
    public float duration;
    public AudioClip StartSFX;
    public AudioClip EndSFX;
}

public class UnitBuffDataCtrl : MonoBehaviour
{
    public UnitBuffData[] buffDatas;
}
