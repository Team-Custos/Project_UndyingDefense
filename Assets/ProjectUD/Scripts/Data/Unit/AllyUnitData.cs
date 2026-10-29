using UnityEngine;

[CreateAssetMenu(fileName = "AllyUnitData", menuName = "ProjectUD/AllyUnitData")]
public class AllyUnitData : UnitData
{
    [Header("■ Ally Unit")]
    [SerializeField] private float cost;
    [SerializeField] private UnitData[] upgradeUnits;

    public float Cost => cost;
    public UnitData[] UpgradeUnits => upgradeUnits;
}
