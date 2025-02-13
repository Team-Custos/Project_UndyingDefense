using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurDebuff
{
    [SerializeField] private UnitDebuff name;
    [SerializeField] private int stack;
    [SerializeField] private float duration;
    [SerializeField] private float currentTime;
}

public class UnitDebuffManager_Rebuild : MonoBehaviour
{
    [SerializeField] UnitDebuff_Rebuild[] debuffs;
    [SerializeField] List<CurDebuff> activeDebuffs = new List<CurDebuff>();
}
