using UnityEngine;
using TargetingType = AllyUnit.TargetingType;

[CreateAssetMenu(fileName = "AllyUnitData", menuName = "ProjectUD/AllyUnitData")]
public class AllyUnitData : UnitData
{
    [Header("■ Ally Unit")]
    [SerializeField] private float cost;    // 소환시 필요한 골드
    [SerializeField] private UnitData[] upgradeUnits;
    [SerializeField] private TargetingType targetingType;

    public float Cost => cost;
    public TargetingType TargetingType => targetingType;

    public UnitData[] UpgradeUnits => upgradeUnits;
}
