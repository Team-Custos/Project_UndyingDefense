using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff_Dizzy : UnitDebuff_Rebuild
{
    [Header("====Stats Change====")]
    [SerializeField] private float moveSpeedReductionMultiplier;

    public override void ApplyDebuff(UnitCtrl_ReBuild target)
    {
        target.ChangeMoveSpeed(target.unitData.baseMoveSpeed * moveSpeedReductionMultiplier);
    }
}
