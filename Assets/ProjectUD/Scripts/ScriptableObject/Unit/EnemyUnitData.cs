using UnityEngine;

[CreateAssetMenu(fileName = "EnemyUnitData", menuName = "ProjectUD/EnemyUnitData")]
public class EnemyUnitData : UnitData
{
    [SerializeField] private float gold;

    public float Gold => gold;
}
