using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff_Dizzy : UnitEffect
{
    [Header("====Stats Change====")]
    [SerializeField] private float moveSpeedReductionMultiplier;
    //[SerializeField] private GameObject[] vfxByStack; //스택에 따른 VFX.

    public override void ApplyDebuff(Unit target)
    {
        target.ChangeMoveSpeed(target.unitData.baseMoveSpeed * moveSpeedReductionMultiplier);
    }

    private void Start()
    {
        ApplyDebuff(target);
    }
}
