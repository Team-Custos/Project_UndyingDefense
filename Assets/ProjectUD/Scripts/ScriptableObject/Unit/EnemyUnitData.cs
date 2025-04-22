using UnityEngine;
using TargetingType = EnemyUnit.TargetingType;
using AIStance = EnemyUnit.AIStance;

[CreateAssetMenu(fileName = "EnemyUnitData", menuName = "ProjectUD/EnemyUnitData")]
public class EnemyUnitData : UnitData
{
    [SerializeField] private float gold;

    public TargetingType targetingType;
    public AIStance aiStance;

    public float Gold => gold;
}
