using System;
using UnityEngine;

//이 스크립트는 유닛의 버프의 기본 데이터를 관리하기 위한 스크립트입니다.

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
